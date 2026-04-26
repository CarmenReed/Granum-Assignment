// ENHANCEMENT-1: Two-sided language model
// Spec: granum-v2/specs/01-two-sided-language.md
// Status: STUB ONLY. Not wired to runtime.
// Demo: this file exists so Monday's deep-dive can show the
//       architectural shape without committing implementation
//       time before validation.

using Api.Models;

namespace Api.Services;

/// <summary>
/// Orchestrates the two-sided language pipeline: detects the crew's
/// input language and translates the cleaned output into the
/// operator's selected language. Splits the input-language axis
/// from the output-language axis so a Quebec operator's French
/// output and a Hispanic crew's Spanish input are not collapsed
/// into the same channel.
/// </summary>
public interface ILanguageOrchestrator
{
    /// <summary>
    /// Detects the language the crew member wrote a note in.
    /// </summary>
    /// <param name="crewInput">The raw note written by the crew member.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Detected language and a confidence score.</returns>
    Task<CrewLanguageDetection> DetectCrewLanguageAsync(
        string crewInput,
        CancellationToken ct = default);

    /// <summary>
    /// Translates a cleaned note into the operator's selected language.
    /// </summary>
    /// <param name="cleanedText">The cleaned text in the source language.</param>
    /// <param name="sourceLanguageCode">BCP 47 source language tag.</param>
    /// <param name="targetLanguage">Operator's selected output language.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The cleaned text in the operator's selected language.</returns>
    Task<string> TranslateToOperatorLanguageAsync(
        string cleanedText,
        string sourceLanguageCode,
        OperatorLanguagePreference targetLanguage,
        CancellationToken ct = default);
}
