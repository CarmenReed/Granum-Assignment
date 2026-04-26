// ENHANCEMENT-2: PHC application record mode for SingleOps Tree Inventory
// Spec: granum-v2/specs/02-phc-tree-inventory-compliance.md
// Status: STUB ONLY. Not wired to runtime. Not part of build.
// Demo: this file exists so Monday's deep-dive can show the
//       architectural shape without committing implementation
//       time before validation.

namespace Api.Models;

/// <summary>
/// Domains in which a note triggers regulated record-keeping
/// requirements. None means general-purpose cleanup is acceptable;
/// any other value routes the cleanup pass through a domain-specific
/// prompt that enforces the regulator's required phrasing. Wiring
/// target on the Granum portfolio: SingleOps Tree Inventory's
/// existing PHC-treatment capture surface.
/// </summary>
public enum RegulatedDomain
{
    None,
    PesticideApplication,
    FertilizerApplication,
    ChemicalStorage
}
