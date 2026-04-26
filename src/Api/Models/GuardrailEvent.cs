// ENHANCEMENT-5: Webhook out on guardrail hit
// Spec: enhancements/05-webhook-out.md
// Status: STUB ONLY. Not wired to runtime.
// Demo: this file exists so Monday's deep-dive can show the
//       architectural shape without committing implementation
//       time before validation.

namespace Api.Models;

/// <summary>
/// Outbound event payload emitted to a configured webhook URL when
/// the inference gate flags a request. Carries no raw input and no
/// PII -- the input is identified by a hash so downstream consumers
/// can correlate without re-storing the original text.
/// </summary>
/// <param name="EventType">Stable event-type tag (e.g., "pii_rejected", "low_confidence", "compliance_failure").</param>
/// <param name="Timestamp">UTC time the gate fired.</param>
/// <param name="InputHash">Hash of the original input (algorithm chosen at wiring time; SHA-256 is the working assumption).</param>
/// <param name="RejectionReason">Short reason string from the gate (e.g., "email detected", "phone detected").</param>
/// <param name="CrewId">Optional crew-member identifier when the request supplies one.</param>
public record GuardrailEvent(
    string EventType,
    DateTime Timestamp,
    string InputHash,
    string RejectionReason,
    string? CrewId);
