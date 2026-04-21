namespace Api.Services;

public interface ILlmService
{
    Task<LlmResult> EnhanceAsync(string rawNote, CancellationToken ct = default);
    IAsyncEnumerable<string> EnhanceStreamAsync(string rawNote, CancellationToken ct = default);
}

public record LlmResult(
    string EnhancedText,
    string Model,
    int PromptTokens,
    int CompletionTokens,
    int TotalTokens,
    long LatencyMs);
