// ENHANCEMENT-5: Prompt A/B testing harness (CONDITIONAL)
// Spec: granum-v2/specs/05-prompt-ab-testing-harness.md
// Status: STUB ONLY. Not wired to runtime. Not part of build.
// v2.0 release · 2026-04-26

using Api.Models;

namespace Api.Services;

/// <summary>
/// Runs two prompt variants against a fixture file of historical
/// inputs and produces a side-by-side comparison artifact for human
/// review. Tooling for the prompt author; not a runtime path on
/// the inference endpoint. Conditional on Granum running an LLM in
/// production today.
/// </summary>
public interface IPromptTestHarness
{
    /// <summary>
    /// Runs the control and candidate variants against every input
    /// in the fixture file and returns one comparison row per input.
    /// </summary>
    /// <param name="control">The currently-deployed prompt.</param>
    /// <param name="candidate">The candidate prompt under review.</param>
    /// <param name="fixturePath">Path to a JSON fixture of historical inputs.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>One <see cref="PromptComparisonResult"/> per fixture input.</returns>
    Task<IReadOnlyList<PromptComparisonResult>> RunComparisonAsync(
        PromptVariant control,
        PromptVariant candidate,
        string fixturePath,
        CancellationToken ct = default);
}
