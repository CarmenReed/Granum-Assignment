// ENHANCEMENT-3: Zapier trigger on guardrail hit
// Spec: granum-v2/specs/03-zapier-trigger-on-guardrail-hit.md
// Status: STUB ONLY. Not wired to runtime. Not part of build.
// v2.0 release · 2026-04-26

namespace Api.Models;

/// <summary>
/// Outbound event payload emitted to Zapier as an "instant trigger"
/// when the inference gate flags a request. Carries no raw input
/// and no PII: the input is identified by a hash so downstream
/// Zapier consumers can correlate without re-storing the original
/// text. Designed to fit Zapier's existing trigger surface on LMN
/// Professional+, the integration lane Granum already markets.
/// </summary>
/// <param name="EventType">Stable event-type tag (e.g., "pii_rejected", "low_confidence", "compliance_failure").</param>
/// <param name="Timestamp">UTC time the gate fired.</param>
/// <param name="InputHash">Hash of the original input (algorithm chosen at wiring time; SHA-256 is the working assumption).</param>
/// <param name="RejectionReason">Short reason string from the gate (e.g., "email detected", "phone detected").</param>
/// <param name="CrewId">Optional crew-member identifier when the request supplies one.</param>
public record GuardrailEvent(
    string EventType,
    DateTimeOffset Timestamp,
    string InputHash,
    string RejectionReason,
    string? CrewId);
