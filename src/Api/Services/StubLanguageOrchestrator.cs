// ENHANCEMENT-1: Two-sided language model
// Spec: enhancements/01-two-sided-language.md
// Status: STUB ONLY. Not wired to runtime.
// Demo: this file exists so Monday's deep-dive can show the
//       architectural shape without committing implementation
//       time before validation.

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
        // (English -> en-US output; French -> fr-CA for Quebec or
        // generic French for non-Quebec Canadian operators).
        throw new NotImplementedException("ENHANCEMENT-1 stub");
    }
}
