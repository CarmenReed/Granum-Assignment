// ENHANCEMENT-5: Webhook out on guardrail hit
// Spec: enhancements/05-webhook-out.md
// Status: STUB ONLY. Not wired to runtime.
// Demo: this file exists so Monday's deep-dive can show the
//       architectural shape without committing implementation
//       time before validation.

using Api.Models;

namespace Api.Services;

/// <summary>
/// Emits a <see cref="GuardrailEvent"/> to a configured outbound
/// destination when the inference gate fires. Real implementation
/// is fire-and-forget from the request path's perspective; delivery
/// failures must not block the user response.
/// </summary>
public interface IWebhookEmitter
{
    /// <summary>
    /// Emits the event to the configured webhook destination.
    /// </summary>
    /// <param name="evt">The guardrail event.</param>
    /// <param name="ct">Cancellation token (best-effort; emit must not block the response path).</param>
    Task EmitAsync(GuardrailEvent evt, CancellationToken ct = default);
}
