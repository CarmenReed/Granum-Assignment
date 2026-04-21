using System.Net;
using System.Net.Http.Json;
using Api.Data;
using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace EnhancerTests;

public class HistoryEndpointTests : IDisposable
{
    private readonly string _dbPath;
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public HistoryEndpointTests()
    {
        _dbPath = Path.Combine(Path.GetTempPath(), $"granum-test-{Guid.NewGuid():N}.db");
        Environment.SetEnvironmentVariable("ANTHROPIC_API_KEY", "test-key-not-used");

        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(b => b.ConfigureServices(services =>
            {
                var dbDescriptor = services.Single(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                services.Remove(dbDescriptor);
                services.AddDbContext<AppDbContext>(o => o.UseSqlite($"Data Source={_dbPath}"));

                var llmDescriptor = services.Single(d => d.ServiceType == typeof(ILlmService));
                services.Remove(llmDescriptor);
                services.AddScoped<ILlmService>(_ => new Mock<ILlmService>().Object);
            }));

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task FirstPage_ReturnsCorrectCount()
    {
        await SeedAsync(12);

        var resp = await _client.GetAsync("/history?page=1&pageSize=5");

        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        var body = await resp.Content.ReadFromJsonAsync<HistoryResponse>();
        Assert.NotNull(body);
        Assert.Equal(12, body!.TotalCount);
        Assert.Equal(5, body.PageSize);
        Assert.Equal(1, body.Page);
        Assert.Equal(3, body.TotalPages);
        Assert.Equal(5, body.Items.Count);
    }

    [Fact]
    public async Task SecondPage_OffsetIsCorrect()
    {
        await SeedAsync(12);

        var page1 = await (await _client.GetAsync("/history?page=1&pageSize=5"))
            .Content.ReadFromJsonAsync<HistoryResponse>();
        var page2 = await (await _client.GetAsync("/history?page=2&pageSize=5"))
            .Content.ReadFromJsonAsync<HistoryResponse>();

        Assert.NotNull(page1);
        Assert.NotNull(page2);
        Assert.Equal(5, page2!.Items.Count);
        var page1Ids = page1!.Items.Select(i => i.Id).ToHashSet();
        Assert.All(page2.Items, item => Assert.DoesNotContain(item.Id, page1Ids));
    }

    [Fact]
    public async Task EmptyDatabase_ReturnsEmptyResult()
    {
        var resp = await _client.GetAsync("/history");

        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        var body = await resp.Content.ReadFromJsonAsync<HistoryResponse>();
        Assert.NotNull(body);
        Assert.Empty(body!.Items);
        Assert.Equal(0, body.TotalCount);
        Assert.Equal(0, body.TotalPages);
    }

    private async Task SeedAsync(int count)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.EnsureCreatedAsync();
        for (var i = 0; i < count; i++)
        {
            db.Interactions.Add(new InteractionLog
            {
                Id = Guid.NewGuid().ToString(),
                RawNote = $"note {i}",
                EnhancedText = $"enhanced {i}",
                Model = "test-model",
                PromptTokens = 10,
                CompletionTokens = 20,
                TotalTokens = 30,
                LatencyMs = 100 + i,
                Outcome = InteractionOutcome.Success,
                ErrorDetail = null,
                Timestamp = DateTime.UtcNow.AddMinutes(-i)
            });
        }
        await db.SaveChangesAsync();
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
        SqliteConnection.ClearAllPools();
        try { if (File.Exists(_dbPath)) File.Delete(_dbPath); }
        catch (IOException) { }
    }
}
