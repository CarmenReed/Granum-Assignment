// ENHANCEMENT-3: Zapier trigger on guardrail hit
// Spec: granum-v2/specs/03-zapier-trigger-on-guardrail-hit.md
// Status: STUB ONLY. Not wired to runtime. Not part of build.
// v2.0 release · 2026-04-26

using Api.Models;

namespace Api.Services;

/// <summary>
/// Stub implementation of <see cref="IZapierTriggerEmitter"/>. Throws
/// so callers cannot silently consume the contract without wiring a
/// real HTTP client to Zapier's trigger endpoint, plus auth, plus
/// retry/backoff, plus per-tenant tier-gating to LMN Professional+.
/// </summary>
public class StubZapierTriggerEmitter : IZapierTriggerEmitter
{
    public Task EmitAsync(GuardrailEvent evt, CancellationToken ct = default)
    {
        // ENHANCEMENT-3: real implementation would POST the event as
        // JSON to Zapier's instant-trigger endpoint, authenticated per
        // the Zapier App Directory listing's auth scheme. Delivery
        // happens off the request path so a slow Zapier endpoint does
        // not block the user response.
        throw new NotImplementedException("ENHANCEMENT-3 stub");
    }
}
