// ENHANCEMENT-5: Webhook out on guardrail hit
// Spec: enhancements/05-webhook-out.md
// Status: STUB ONLY. Not wired to runtime.
// Demo: this file exists so Monday's deep-dive can show the
//       architectural shape without committing implementation
//       time before validation.

using Api.Models;

namespace Api.Services;

/// <summary>
/// Stub implementation of <see cref="IWebhookEmitter"/>. Throws so
/// callers cannot silently consume the contract without wiring a
/// real HTTP client, signing scheme, retry/backoff policy, and
/// per-tenant destination configuration.
/// </summary>
public class StubWebhookEmitter : IWebhookEmitter
{
    public Task EmitAsync(GuardrailEvent evt, CancellationToken ct = default)
    {
        // ENHANCEMENT-5: real implementation would POST the event as
        // JSON to a per-tenant configured URL with HMAC signing and
        // a retry/backoff policy. Delivery happens off the request
        // path so a slow or down destination does not block the user.
        throw new NotImplementedException("ENHANCEMENT-5 stub");
    }
}
