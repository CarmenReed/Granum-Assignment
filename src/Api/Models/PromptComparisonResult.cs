// ENHANCEMENT-3: A/B prompt testing harness
// Spec: enhancements/03-prompt-ab-testing.md
// Status: STUB ONLY. Not wired to runtime.
// Demo: this file exists so Monday's deep-dive can show the
//       architectural shape without committing implementation
//       time before validation.

namespace Api.Models;

/// <summary>
/// One row of an A/B prompt comparison: a single fixture input run
/// through both the control and candidate variants, with both
/// outputs captured side-by-side so the prompt author can review
/// the diff before deploying the candidate.
/// </summary>
/// <param name="Input">The fixture input shared by both runs.</param>
/// <param name="ControlVariantId">Stable id of the control variant.</param>
/// <param name="ControlOutput">Output produced by the control prompt.</param>
/// <param name="CandidateVariantId">Stable id of the candidate variant.</param>
/// <param name="CandidateOutput">Output produced by the candidate prompt.</param>
/// <param name="DiffSummary">Short human-readable summary of the diff.</param>
public record PromptComparisonResult(
    string Input,
    string ControlVariantId,
    string ControlOutput,
    string CandidateVariantId,
    string CandidateOutput,
    string DiffSummary);
