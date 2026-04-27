// ENHANCEMENT-4: Greenius training trigger on guardrail hit
// Spec: granum-v2/specs/04-greenius-training-trigger.md
// Status: STUB ONLY. Not wired to runtime. Not part of build.
// v2.0 release · 2026-04-26

using Api.Models;

namespace Api.Services;

/// <summary>
/// Assigns the matching Greenius course to a crew member when the
/// inference gate fires with a categorized reason. Closes the
/// learning loop using the only documented inter-product integration
/// in the Granum portfolio: Greenius training surfaced inside the
/// LMN Crew app.
/// </summary>
public interface IGreeniusTrainingAssigner
{
    /// <summary>
    /// Assigns a Greenius course to a crew member based on the
    /// guardrail category that fired.
    /// </summary>
    /// <param name="category">The category of the guardrail hit.</param>
    /// <param name="crewId">The crew member who triggered the flag.</param>
    /// <param name="ct">Cancellation token (best-effort; assignment must not block the response path).</param>
    Task AssignAsync(
        GuardrailCategory category,
        string crewId,
        CancellationToken ct = default);
}
