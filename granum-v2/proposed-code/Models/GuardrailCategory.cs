// ENHANCEMENT-4: Greenius training trigger on guardrail hit
// Spec: granum-v2/specs/04-greenius-training-trigger.md
// Status: STUB ONLY. Not wired to runtime. Not part of build.
// Demo: this file exists so Monday's deep-dive can show the
//       architectural shape without committing implementation
//       time before validation.

namespace Api.Models;

/// <summary>
/// High-level category of an inference-gate hit. Used to look up
/// the matching Greenius course for auto-assignment to the crew
/// member who triggered the flag. Mapping (category to course id)
/// is configurable so course content can change without code.
/// </summary>
public enum GuardrailCategory
{
    PiiLeak,
    ComplianceMissed,
    LowConfidence,
    Other
}
