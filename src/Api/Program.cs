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
