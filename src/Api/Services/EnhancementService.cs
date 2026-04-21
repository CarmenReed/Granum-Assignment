using System.Diagnostics;
using Api.Data;
using Api.Models;

namespace Api.Services;

public abstract record EnhancementResult
{
    public sealed record Success(
        string Id,
        string EnhancedText,
        string Model,
        int PromptTokens,
        int CompletionTokens,
        int TotalTokens,
        long LatencyMs,
        DateTime Timestamp) : EnhancementResult;

    public sealed record ValidationError(string Message) : EnhancementResult;
    public sealed record PiiError(string Reason) : EnhancementResult;
    public sealed record LlmError(string PublicMessage) : EnhancementResult;
}

public class EnhancementService
{
    public const int MaxNoteLength = 2000;

    private readonly PiiGuardService _pii;
    private readonly ILlmService _llm;
    private readonly InteractionRepository _repo;

    public EnhancementService(PiiGuardService pii, ILlmService llm, InteractionRepository repo)
    {
        _pii = pii;
        _llm = llm;
        _repo = repo;
    }

    public async Task<EnhancementResult> EnhanceAsync(string? rawNote, CancellationToken ct = default)
    {
        var safeInput = rawNote ?? string.Empty;

        if (string.IsNullOrWhiteSpace(safeInput))
        {
            const string msg = "rawNote is required.";
            await LogFailureAsync(safeInput, InteractionOutcome.ValidationFailure, msg, 0, ct);
            return new EnhancementResult.ValidationError(msg);
        }

        if (safeInput.Length > MaxNoteLength)
        {
            var msg = $"rawNote exceeds {MaxNoteLength} characters.";
            await LogFailureAsync(safeInput, InteractionOutcome.ValidationFailure, msg, 0, ct);
            return new EnhancementResult.ValidationError(msg);
        }

        var pii = _pii.Check(safeInput);
        if (pii.IsFlagged)
        {
            await LogFailureAsync(safeInput, InteractionOutcome.PiiRejected, pii.Reason, 0, ct);
            return new EnhancementResult.PiiError(pii.Reason!);
        }

        LlmResult llmResult;
        var sw = Stopwatch.StartNew();
        try
        {
            llmResult = await _llm.EnhanceAsync(safeInput, ct);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (OperationCanceledException)
        {
            sw.Stop();
            await LogFailureAsync(safeInput, InteractionOutcome.LlmFailure, "LLM call timed out.", sw.ElapsedMilliseconds, ct);
            return new EnhancementResult.LlmError("Enhancement temporarily unavailable.");
        }
        catch (Exception ex)
        {
            sw.Stop();
            await LogFailureAsync(safeInput, InteractionOutcome.LlmFailure, ex.Message, sw.ElapsedMilliseconds, ct);
            return new EnhancementResult.LlmError("Enhancement temporarily unavailable.");
        }

        var id = Guid.NewGuid().ToString();
        var timestamp = DateTime.UtcNow;
        var log = new InteractionLog
        {
            Id = id,
            RawNote = safeInput,
            EnhancedText = llmResult.EnhancedText,
            Model = llmResult.Model,
            PromptTokens = llmResult.PromptTokens,
            CompletionTokens = llmResult.CompletionTokens,
            TotalTokens = llmResult.TotalTokens,
            LatencyMs = llmResult.LatencyMs,
            Outcome = InteractionOutcome.Success,
            ErrorDetail = null,
            Timestamp = timestamp
        };
        await _repo.AddAsync(log, ct);

        return new EnhancementResult.Success(
            id,
            llmResult.EnhancedText,
            llmResult.Model,
            llmResult.PromptTokens,
            llmResult.CompletionTokens,
            llmResult.TotalTokens,
            llmResult.LatencyMs,
            timestamp);
    }

    private Task LogFailureAsync(string rawNote, string outcome, string? errorDetail, long latencyMs, CancellationToken ct)
    {
        var log = new InteractionLog
        {
            Id = Guid.NewGuid().ToString(),
            RawNote = rawNote,
            EnhancedText = null,
            Model = null,
            PromptTokens = null,
            CompletionTokens = null,
            TotalTokens = null,
            LatencyMs = latencyMs,
            Outcome = outcome,
            ErrorDetail = errorDetail,
            Timestamp = DateTime.UtcNow
        };
        return _repo.AddAsync(log, ct);
    }
}
