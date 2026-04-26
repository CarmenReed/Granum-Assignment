// ENHANCEMENT-1: English / Spanish two-sided language model
// Spec: granum-v2/specs/01-en-es-two-sided-language.md
// Status: STUB ONLY. Not wired to runtime. Not part of build.

namespace Api.Models;

/// <summary>
/// Result of detecting the language a crew member wrote a note in.
/// DetectedLanguageCode is a BCP 47 tag (e.g., "en", "es").
/// Confidence is 0.0..1.0; the orchestrator may downgrade or fail
/// loud below a configurable threshold.
/// </summary>
/// <param name="DetectedLanguageCode">BCP 47 language tag for the detected crew language.</param>
/// <param name="Confidence">Confidence score, 0.0 to 1.0.</param>
public record CrewLanguageDetection(
    string DetectedLanguageCode,
    double Confidence);
