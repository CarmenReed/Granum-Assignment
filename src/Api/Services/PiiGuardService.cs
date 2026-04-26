using System.Text.RegularExpressions;

namespace Api.Services;

public record PiiCheckResult(bool IsFlagged, string? Reason);

// ENHANCEMENT-4: PII expansion (SSN, address, credit card, DOB)
// Patterns defined below as private readonly fields. NOT wired
// into Check() or Redact() in this commit. Wiring them is a
// 3-line change per pattern, demonstrated live during Monday's
// deep-dive when Jonathan asks "now make it block X."
// Spec: enhancements/04-pii-expansion.md
// Test cases: enhancements/04-test-cases.md

public class PiiGuardService
{
    private static readonly Regex Email = new(
        @"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex UsPhone = new(
        @"(\+?1[-.\s]?)?(\(\d{3}\)|\d{3})[-.\s]?\d{3}[-.\s]?\d{4}",
        RegexOptions.Compiled);

    /// <summary>
    /// ENHANCEMENT-4: SSN in SSA-standard hyphenated form (XXX-XX-XXXX).
    /// Bare 9-digit SSN intentionally excluded from this pattern;
    /// overlap with phone fragments is the load-bearing risk and
    /// would need a separate collision-test before wiring.
    /// </summary>
    private static readonly Regex Ssn = new(
        @"\b\d{3}-\d{2}-\d{4}\b",
        RegexOptions.Compiled);

    /// <summary>
    /// ENHANCEMENT-4: U.S. or Canadian street address. Number,
    /// street name, common suffix (St/Ave/Rd/Blvd/Ln/Dr/Ct/Way), with
    /// an optional Canadian postal code (A1A 1A1) afterward. Highest
    /// false-positive risk of the four; acceptable rate is an open
    /// question on the spec.
    /// </summary>
    private static readonly Regex AddressUsCa = new(
        @"\b\d{1,6}\s+[A-Za-z][A-Za-z0-9.\s'-]{1,40}\s+(Street|St|Avenue|Ave|Road|Rd|Boulevard|Blvd|Lane|Ln|Drive|Dr|Court|Ct|Way|Place|Pl)\b(\s+[A-Z]\d[A-Z]\s?\d[A-Z]\d)?",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    /// <summary>
    /// ENHANCEMENT-4: Credit card 13-19 digits with optional spaces
    /// or hyphens. Luhn validation is intentionally NOT performed in
    /// the regex; wiring this pattern into Check/Redact must include
    /// a Luhn check on the captured digits to avoid flagging arbitrary
    /// long numeric strings.
    /// </summary>
    private static readonly Regex CreditCard = new(
        @"\b(?:\d[ -]?){13,19}\b",
        RegexOptions.Compiled);

    /// <summary>
    /// ENHANCEMENT-4: Date of birth in MM/DD/YYYY or DD/MM/YYYY.
    /// The two formats overlap by design (a U.S. operator's 03/04/1972
    /// and a Canadian operator's 03/04/1972 collide); resolution is
    /// per-tenant locale at wiring time, not in this pattern.
    /// </summary>
    private static readonly Regex Dob = new(
        @"\b(0?[1-9]|1[0-2])[/.-](0?[1-9]|[12]\d|3[01])[/.-](19|20)\d{2}\b",
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
