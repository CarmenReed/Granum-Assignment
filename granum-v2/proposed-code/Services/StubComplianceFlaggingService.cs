// ENHANCEMENT-2: PHC application record mode for SingleOps Tree Inventory
// Spec: granum-v2/specs/02-phc-tree-inventory-compliance.md
// Status: STUB ONLY. Not wired to runtime. Not part of build.
// Demo: this file exists so Monday's deep-dive can show the
//       architectural shape without committing implementation
//       time before validation.

using Api.Models;

namespace Api.Services;

/// <summary>
/// Stub implementation of <see cref="IComplianceFlaggingService"/>.
/// Detection returns None so the stub is safe to register without
/// changing runtime behavior. Enforcement throws to make it
/// impossible to silently consume the contract without a real
/// implementation.
/// </summary>
public class StubComplianceFlaggingService : IComplianceFlaggingService
{
    public Task<RegulatedDomain> DetectRegulatedContextAsync(
        string input,
        CancellationToken ct = default)
    {
        // ENHANCEMENT-2: real implementation would run an LLM
        // classifier (or a regex bootstrap as a fast pre-filter)
        // to detect pesticide/fertilizer/chemical-storage contexts.
        // Wiring target: SingleOps Tree Inventory PHC-treatment entries.
        return Task.FromResult(RegulatedDomain.None);
    }

    public Task<string> EnforceRegulatedPhrasingAsync(
        string input,
        RegulatedDomain domain,
        CancellationToken ct = default)
    {
        // ENHANCEMENT-2: real implementation would route the input
        // through a domain-specific prompt template that enforces
        // EPA WPS (40 CFR Part 170) or Health Canada PCPA-required
        // record fields, depending on operator jurisdiction.
        throw new NotImplementedException("ENHANCEMENT-2 stub");
    }
}
