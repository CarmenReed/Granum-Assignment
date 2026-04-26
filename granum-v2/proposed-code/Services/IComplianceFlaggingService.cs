// ENHANCEMENT-2: PHC application record mode for SingleOps Tree Inventory
// Spec: granum-v2/specs/02-phc-tree-inventory-compliance.md
// Status: STUB ONLY. Not wired to runtime. Not part of build.

using Api.Models;

namespace Api.Services;

/// <summary>
/// Detects whether free-text input touches a regulated domain
/// (pesticide application, fertilizer application, chemical storage)
/// and, when so, enforces the regulator's required phrasing on the
/// cleanup pass. Splits "make this read better" from "make this
/// survive an audit." Designed to wire into SingleOps Tree
/// Inventory's existing PHC-treatment capture path.
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
