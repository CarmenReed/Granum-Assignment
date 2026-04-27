// ENHANCEMENT-4: Greenius training trigger on guardrail hit
// Spec: granum-v2/specs/04-greenius-training-trigger.md
// Status: STUB ONLY. Not wired to runtime. Not part of build.
// v2.0 release · 2026-04-26

using Api.Models;

namespace Api.Services;

/// <summary>
/// Stub implementation of <see cref="IGreeniusTrainingAssigner"/>.
/// Throws so callers cannot silently consume the contract before the
/// open question (whether Greenius exposes a course-assignment API
/// or webhook) is answered. Real implementation depends on Greenius
/// surfacing an assignment endpoint.
/// </summary>
public class StubGreeniusTrainingAssigner : IGreeniusTrainingAssigner
{
    public Task AssignAsync(
        GuardrailCategory category,
        string crewId,
        CancellationToken ct = default)
    {
        // ENHANCEMENT-4: real implementation would look up the
        // category-to-course mapping (per-tenant config) and call
        // Greenius's assignment endpoint. If Greenius does not
        // expose a course-assignment API on the marketing surface,
        // the fallback is an in-app "suggested course" notification
        // inside the LMN Crew app inbox instead of a hard assignment.
        throw new NotImplementedException("ENHANCEMENT-4 stub");
    }
}
