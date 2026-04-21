namespace Api.Models;

public record HistoryItem(
    string Id,
    string RawNote,
    string? EnhancedText,
    string? Model,
    int? PromptTokens,
    int? CompletionTokens,
    int? TotalTokens,
    long LatencyMs,
    string Outcome,
    string? ErrorDetail,
    DateTime Timestamp);

public record HistoryResponse(
    IReadOnlyList<HistoryItem> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);
