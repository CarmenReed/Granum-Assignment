// ENHANCEMENT-1: English / Spanish two-sided language model
// Spec: granum-v2/specs/01-en-es-two-sided-language.md
// Status: STUB ONLY. Not wired to runtime. Not part of build.

using Api.Models;

namespace Api.Services;

/// <summary>
/// Stub implementation of <see cref="ILanguageOrchestrator"/>.
/// Both methods throw to make it impossible to silently consume
/// the contract without wiring a real implementation first.
/// </summary>
public class StubLanguageOrchestrator : ILanguageOrchestrator
{
    public Task<CrewLanguageDetection> DetectCrewLanguageAsync(
        string crewInput,
        CancellationToken ct = default)
    {
        // ENHANCEMENT-1: real implementation would call a language ID
        // service (LLM classifier or fastText-style model) and return
        // the detected BCP 47 tag with a confidence score.
        throw new NotImplementedException("ENHANCEMENT-1 stub");
    }

    public Task<string> TranslateToOperatorLanguageAsync(
        string cleanedText,
        string sourceLanguageCode,
        OperatorLanguagePreference targetLanguage,
        CancellationToken ct = default)
    {
        // ENHANCEMENT-1: real implementation would prompt the LLM with
        // a translation system message keyed on operator preference
        // (English -> en-US output; Spanish -> es-US for the U.S. field
        // workforce that LMN Crew app already markets to).
        throw new NotImplementedException("ENHANCEMENT-1 stub");
    }
}
