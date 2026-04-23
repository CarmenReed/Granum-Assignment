using Api.Data;
using Api.Models;
using Api.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace EnhancerTests;

public class EnhancementServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly AppDbContext _db;
    private readonly InteractionRepository _repo;
    private readonly PiiGuardService _pii = new();
    private readonly Mock<ILlmService> _llm = new();
    private readonly EnhancementService _service;

    public EnhancementServiceTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;
        _db = new AppDbContext(opts);
        _db.Database.EnsureCreated();
        _repo = new InteractionRepository(_db);
        _service = new EnhancementService(_pii, _llm.Object, _repo);
    }

    [Fact]
    public async Task SuccessfulEnhancement_ReturnsSuccessAndLogsInteraction()
    {
        _llm.Setup(x => x.EnhanceAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LlmResult("Polished output.", "claude-haiku-test", 42, 58, 100, 321));

        var result = await _service.EnhanceAsync("Mowed lawn, trimmed hedges, cleaned up.", CancellationToken.None);

        var success = Assert.IsType<EnhancementResult.Success>(result);
        Assert.Equal("Polished output.", success.EnhancedText);
        Assert.Equal(321, success.LatencyMs);

        var log = Assert.Single(_db.Interactions);
        Assert.Equal(InteractionOutcome.Success, log.Outcome);
        Assert.Equal("Polished output.", log.EnhancedText);
        Assert.Equal(100, log.TotalTokens);
        Assert.Null(log.ErrorDetail);
    }

    [Fact]
    public async Task LlmException_ReturnsLlmErrorAndLogsFailure()
    {
        _llm.Setup(x => x.EnhanceAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("upstream 502"));

        var result = await _service.EnhanceAsync("Technician replaced valve.", CancellationToken.None);

        Assert.IsType<EnhancementResult.LlmError>(result);
        var log = Assert.Single(_db.Interactions);
        Assert.Equal(InteractionOutcome.LlmFailure, log.Outcome);
        Assert.Contains("upstream 502", log.ErrorDetail);
        Assert.Null(log.EnhancedText);
    }

    [Fact]
    public async Task LlmTimeout_ReturnsLlmErrorAndLogsTimeout()
    {
        _llm.Setup(x => x.EnhanceAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var result = await _service.EnhanceAsync("Site walk complete.", CancellationToken.None);

        Assert.IsType<EnhancementResult.LlmError>(result);
        var log = Assert.Single(_db.Interactions);
        Assert.Equal(InteractionOutcome.LlmFailure, log.Outcome);
        Assert.Equal("LLM call timed out.", log.ErrorDetail);
    }

    [Fact]
    public async Task PiiRejection_PersistsRedactedRawNote_NotOriginal()
    {
        const string raw = "call customer at 555-123-4567 to reschedule";

        var result = await _service.EnhanceAsync(raw, CancellationToken.None);

        Assert.IsType<EnhancementResult.PiiError>(result);
        var log = Assert.Single(_db.Interactions);
        Assert.Equal(InteractionOutcome.PiiRejected, log.Outcome);
        Assert.DoesNotContain("555-123-4567", log.RawNote);
        Assert.Contains("[REDACTED-PHONE]", log.RawNote);
        _llm.Verify(x => x.EnhanceAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    public void Dispose()
    {
        _db.Dispose();
        _connection.Dispose();
    }
}
