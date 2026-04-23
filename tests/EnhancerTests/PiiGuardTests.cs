using Api.Services;

namespace EnhancerTests;

public class PiiGuardTests
{
    private readonly PiiGuardService _pii = new();

    [Fact]
    public void Email_IsDetected()
    {
        var result = _pii.Check("Please reach out to tech.lead@example.com for follow-up.");
        Assert.True(result.IsFlagged);
        Assert.Equal("email detected", result.Reason);
    }

    [Theory]
    [InlineData("(555) 555-5555")]
    [InlineData("555-555-5555")]
    [InlineData("555.555.5555")]
    [InlineData("5555555555")]
    [InlineData("+1 555 555 5555")]
    [InlineData("1-555-555-5555")]
    public void UsPhone_IsDetected(string phone)
    {
        var result = _pii.Check($"Customer can be reached at {phone} during business hours.");
        Assert.True(result.IsFlagged);
        Assert.Equal("phone detected", result.Reason);
    }

    [Theory]
    [InlineData("Technician arrived at 2pm. Installed 3 sprinkler heads on zone 4.")]
    [InlineData("Replaced hose, confirmed pressure at 45 psi, cleaned up and left site.")]
    [InlineData("")]
    [InlineData("   ")]
    public void CleanInput_Passes(string input)
    {
        var result = _pii.Check(input);
        Assert.False(result.IsFlagged);
        Assert.Null(result.Reason);
    }

    [Fact]
    public void Redact_ReplacesEmail()
    {
        var redacted = _pii.Redact("Follow up with tech.lead@example.com next week.");
        Assert.Equal("Follow up with [REDACTED-EMAIL] next week.", redacted);
    }

    [Fact]
    public void Redact_ReplacesPhone()
    {
        var redacted = _pii.Redact("Call customer at 555-123-4567 tomorrow.");
        Assert.Equal("Call customer at [REDACTED-PHONE] tomorrow.", redacted);
    }

    [Fact]
    public void Redact_ReplacesMultipleAndMixed()
    {
        var redacted = _pii.Redact("Email ops@acme.com or call (555) 987-6543.");
        Assert.Equal("Email [REDACTED-EMAIL] or call [REDACTED-PHONE].", redacted);
    }

    [Fact]
    public void Redact_LeavesCleanInputUnchanged()
    {
        const string clean = "Replaced valve on zone 3, pressure reads 42 psi.";
        Assert.Equal(clean, _pii.Redact(clean));
    }
}
