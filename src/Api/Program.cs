using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Api.Data;
using Api.Middleware;
using Api.Models;
using Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var dbPath = Environment.GetEnvironmentVariable("DATABASE_PATH") ?? "interactions.db";
builder.Services.AddDbContext<AppDbContext>(opts => opts.UseSqlite($"Data Source={dbPath}"));
builder.Services.AddScoped<InteractionRepository>();

builder.Services.AddHttpClient<ILlmService, AnthropicLlmService>(client =>
{
    client.BaseAddress = new Uri("https://api.anthropic.com");
    client.Timeout = Timeout.InfiniteTimeSpan;
});

builder.Services.AddSingleton<PiiGuardService>();
builder.Services.AddScoped<EnhancementService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.UseMiddleware<ErrorHandlingMiddleware>();

app.MapGet("/", () => "Granum Assignment API");

app.MapGet("/history", async (InteractionRepository repo, int? page, int? pageSize, CancellationToken ct) =>
{
    var p = page is null or < 1 ? 1 : page.Value;
    var size = pageSize switch
    {
        null or < 1 => 10,
        > 50 => 50,
        _ => pageSize.Value
    };

    var (items, total) = await repo.GetPagedAsync(p, size, ct);
    var mapped = items.Select(x => new HistoryItem(
        x.Id, x.RawNote, x.EnhancedText, x.Model, x.PromptTokens, x.CompletionTokens,
        x.TotalTokens, x.LatencyMs, x.Outcome, x.ErrorDetail, x.Timestamp)).ToList();
    var totalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)size);
    return Results.Ok(new HistoryResponse(mapped, p, size, total, totalPages));
});

app.MapGet("/enhance/stream", async (HttpContext ctx, string? note, EnhancementService service,
    ILlmService llm, CancellationToken ct) =>
{
    var preflight = await service.PreflightAsync(note, ct);
    if (preflight is EnhancementResult.ValidationError v)
    {
        ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
        await ctx.Response.WriteAsJsonAsync(new { error = v.Message }, ct);
        return;
    }
    if (preflight is EnhancementResult.PiiError p)
    {
        ctx.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
        await ctx.Response.WriteAsJsonAsync(new { error = $"Input contains PII: {p.Reason}." }, ct);
        return;
    }

    ctx.Response.Headers["Content-Type"] = "text/event-stream";
    ctx.Response.Headers["Cache-Control"] = "no-cache";
    ctx.Response.Headers["X-Accel-Buffering"] = "no";
    await ctx.Response.Body.FlushAsync(ct);

    var accumulated = new StringBuilder();
    var sw = Stopwatch.StartNew();
    var model = Environment.GetEnvironmentVariable("ANTHROPIC_MODEL") ?? "claude-haiku-4-5-20251001";
    var clean = note!;

    try
    {
        await foreach (var chunk in llm.EnhanceStreamAsync(clean, ct))
        {
            accumulated.Append(chunk);
            var line = $"data: {JsonSerializer.Serialize(new { chunk })}\n\n";
            await ctx.Response.WriteAsync(line, ct);
            await ctx.Response.Body.FlushAsync(ct);
        }
    }
    catch (OperationCanceledException) when (ctx.RequestAborted.IsCancellationRequested)
    {
        return;
    }
    catch (Exception ex)
    {
        sw.Stop();
        await service.LogStreamLlmFailureAsync(clean, ex.Message, sw.ElapsedMilliseconds, CancellationToken.None);
        return;
    }

    sw.Stop();
    await service.LogStreamSuccessAsync(clean, accumulated.ToString(), model, sw.ElapsedMilliseconds, ct);

    var final = JsonSerializer.Serialize(new { done = true, model, latencyMs = sw.ElapsedMilliseconds });
    await ctx.Response.WriteAsync($"data: {final}\n\n", ct);
    await ctx.Response.Body.FlushAsync(ct);
});

app.MapPost("/enhance", async (EnhanceRequest request, EnhancementService service, CancellationToken ct) =>
{
    var result = await service.EnhanceAsync(request?.RawNote, ct);
    return result switch
    {
        EnhancementResult.Success s => Results.Ok(new EnhanceResponse(
            s.Id, s.EnhancedText, s.Model, s.PromptTokens, s.CompletionTokens,
            s.TotalTokens, s.LatencyMs, s.Timestamp)),
        EnhancementResult.ValidationError v => Results.BadRequest(new { error = v.Message }),
        EnhancementResult.PiiError p => Results.Json(new { error = $"Input contains PII: {p.Reason}." },
            statusCode: StatusCodes.Status422UnprocessableEntity),
        EnhancementResult.LlmError l => Results.Json(new { error = l.PublicMessage },
            statusCode: StatusCodes.Status500InternalServerError),
        _ => Results.StatusCode(StatusCodes.Status500InternalServerError)
    };
});

app.Run();

public partial class Program { }
