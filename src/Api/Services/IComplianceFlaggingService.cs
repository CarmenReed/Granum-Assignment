// ENHANCEMENT-2: Compliance flagging (regulated phrasing)
// Spec: enhancements/02-compliance-flagging.md
// Status: STUB ONLY. Not wired to runtime.
// Demo: this file exists so Monday's deep-dive can show the
//       architectural shape without committing implementation
//       time before validation.

using Api.Models;

namespace Api.Services;

/// <summary>
/// Detects whether free-text input touches a regulated domain
/// (pesticide application, fertilizer application, chemical storage)
/// and, when so, enforces the regulator's required phrasing on the
/// cleaning pass. Splits "make this read better" from "make this
/// survive an audit."
/// </summary>
public interface IComplianceFlaggingService
{
    /// <summary>
    /// Classifies the input's regulated domain, if any.
    /// </summary>
    /// <param name="input">The raw note from the crew.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The detected regulated domain, or None.</returns>
    Task<RegulatedDomain> DetectRegulatedContextAsync(
        string input,
        CancellationToken ct = default);

    /// <summary>
    /// Re-cleans input under a domain-specific prompt that enforces
    /// the regulator's required record fields and phrasing.
    /// </summary>
    /// <param name="input">The raw note from the crew.</param>
    /// <param name="domain">The detected regulated domain (must not be None).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Cleaned text in regulator-acceptable phrasing.</returns>
    Task<string> EnforceRegulatedPhrasingAsync(
        string input,
        RegulatedDomain domain,
        CancellationToken ct = default);
}
