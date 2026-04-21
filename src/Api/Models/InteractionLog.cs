namespace Api.Models;

public class InteractionLog
{
    public string Id { get; set; } = "";
    public string RawNote { get; set; } = "";
    public string? EnhancedText { get; set; }
    public string? Model { get; set; }
    public int? PromptTokens { get; set; }
    public int? CompletionTokens { get; set; }
    public int? TotalTokens { get; set; }
    public long LatencyMs { get; set; }
    public string Outcome { get; set; } = "";
    public string? ErrorDetail { get; set; }
    public DateTime Timestamp { get; set; }
}

public static class InteractionOutcome
{
    public const string Success = "success";
    public const string LlmFailure = "llm_failure";
    public const string ValidationFailure = "validation_failure";
    public const string PiiRejected = "pii_rejected";
}
