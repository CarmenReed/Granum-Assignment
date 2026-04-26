// ENHANCEMENT-5: Prompt A/B testing harness (CONDITIONAL)
// Spec: granum-v2/specs/05-prompt-ab-testing-harness.md
// Status: STUB ONLY. Not wired to runtime. Not part of build.
// Demo: this file exists so Monday's deep-dive can show the
//       architectural shape. Conditional on Granum shipping
//       LLM-driven features in production (Phase 1 found zero
//       AI/ML claims across 34 pages).

namespace Api.Models;

/// <summary>
/// One row of an A/B prompt comparison: a single fixture input run
/// through both the control and candidate variants, with both
/// outputs captured side-by-side so the prompt author can review
/// the diff before deploying the candidate.
/// </summary>
/// <param name="Input">The fixture input shared by both runs.</param>
/// <param name="ControlOutput">Output produced by the control prompt.</param>
/// <param name="CandidateOutput">Output produced by the candidate prompt.</param>
/// <param name="DiffSummary">Short human-readable summary of the diff.</param>
public record PromptComparisonResult(
    string Input,
    string ControlOutput,
    string CandidateOutput,
    string DiffSummary);
