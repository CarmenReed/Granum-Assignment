// ENHANCEMENT-3: A/B prompt testing harness
// Spec: granum-v2/specs/03-prompt-ab-testing.md
// Status: STUB ONLY. Not wired to runtime.
// Demo: this file exists so Monday's deep-dive can show the
//       architectural shape without committing implementation
//       time before validation.

using Api.Models;

namespace Api.Services;

/// <summary>
/// Stub implementation of <see cref="IPromptTestHarness"/>. Throws
/// so callers cannot silently consume the contract without a real
/// implementation; the fixture file at
/// <c>granum-v2/specs/03-fixtures/sample-historical-inputs.json</c>
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
        // ENHANCEMENT-3: real implementation would load the fixture
        // file, invoke ILlmService once per input per variant, and
        // produce a diff summary per row. The diff algorithm choice
        // (line/token/semantic) is a deliberate open question.
        throw new NotImplementedException("ENHANCEMENT-3 stub");
    }
}
