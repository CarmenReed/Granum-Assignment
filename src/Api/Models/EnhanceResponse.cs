namespace Api.Models;

public record EnhanceResponse(
    string Id,
    string EnhancedText,
    string Model,
    int PromptTokens,
    int CompletionTokens,
    int TotalTokens,
    long LatencyMs,
    DateTime Timestamp);
