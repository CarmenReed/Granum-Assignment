// ENHANCEMENT-5: Prompt A/B testing harness (CONDITIONAL)
// Spec: granum-v2/specs/05-prompt-ab-testing-harness.md
// Status: STUB ONLY. Not wired to runtime. Not part of build.
// Demo: this file exists so Monday's deep-dive can show the
//       architectural shape. Conditional on Granum shipping
//       LLM-driven features in production (Phase 1 found zero
//       AI/ML claims across 34 pages).

using Api.Models;

namespace Api.Services;

/// <summary>
/// Stub implementation of <see cref="IPromptTestHarness"/>. Throws
/// so callers cannot silently consume the contract without a real
/// implementation; the fixture file at
/// <c>granum-v2/specs/05-fixtures/sample-historical-inputs.json</c>
/// is a real artifact and exists independently of this stub.
/// </summary>
public class StubPromptTestHarness : IPromptTestHarness
{
    public Task<IReadOnlyList<PromptComparisonResult>> RunComparisonAsync(
        PromptVariant control,
        PromptVariant candidate,
        string fixturePath,
        CancellationToken ct = default)
    {
        // ENHANCEMENT-5: real implementation would load the fixture
        // file, invoke ILlmService once per input per variant, and
        // produce a diff summary per row. The diff algorithm choice
        // (line/token/semantic) is a deliberate open question.
        throw new NotImplementedException("ENHANCEMENT-5 stub");
    }
}
