using System.Text.RegularExpressions;

namespace Api.Services;

public record PiiCheckResult(bool IsFlagged, string? Reason);

public class PiiGuardService
{
    private static readonly Regex Email = new(
        @"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex UsPhone = new(
        @"(\+?1[-.\s]?)?(\(\d{3}\)|\d{3})[-.\s]?\d{3}[-.\s]?\d{4}",
        RegexOptions.Compiled);

    public PiiCheckResult Check(string input)
    {
        if (string.IsNullOrEmpty(input)) return new PiiCheckResult(false, null);
        if (Email.IsMatch(input)) return new PiiCheckResult(true, "email detected");
        if (UsPhone.IsMatch(input)) return new PiiCheckResult(true, "phone detected");
        return new PiiCheckResult(false, null);
    }

    public string Redact(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        var redacted = Email.Replace(input, "[REDACTED-EMAIL]");
        redacted = UsPhone.Replace(redacted, "[REDACTED-PHONE]");
        return redacted;
    }
}
