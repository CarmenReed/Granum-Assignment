// ENHANCEMENT-3: Zapier trigger on guardrail hit
// Spec: granum-v2/specs/03-zapier-trigger-on-guardrail-hit.md
// Status: STUB ONLY. Not wired to runtime. Not part of build.

using Api.Models;

namespace Api.Services;

/// <summary>
/// Emits a <see cref="GuardrailEvent"/> as a Zapier "instant trigger"
/// when the inference gate fires. Reuses Granum's existing Zapier
/// integration lane on LMN Professional+ ("Over 6,000 apps with
/// Zapier") instead of inventing a new native webhook surface.
/// Real implementation is fire-and-forget from the request path's
/// perspective; delivery failures must not block the user response.
/// </summary>
public interface IZapierTriggerEmitter
{
    /// <summary>
    /// Emits the event to Zapier's trigger endpoint for the customer's
    /// configured zap.
    /// </summary>
    /// <param name="evt">The guardrail event.</param>
    /// <param name="ct">Cancellation token (best-effort; emit must not block the response path).</param>
    Task EmitAsync(GuardrailEvent evt, CancellationToken ct = default);
}
