// ENHANCEMENT-2: Compliance flagging (regulated phrasing)
// Spec: granum-v2/specs/02-compliance-flagging.md
// Status: STUB ONLY. Not wired to runtime.
// Demo: this file exists so Monday's deep-dive can show the
//       architectural shape without committing implementation
//       time before validation.

namespace Api.Models;

/// <summary>
/// Domains in which a note triggers regulated record-keeping
/// requirements. None means general-purpose cleaning is acceptable;
/// any other value routes the cleaning pass through a domain-specific
/// prompt that enforces the regulator's required phrasing.
/// </summary>
public enum RegulatedDomain
{
    None,
    PesticideApplication,
    FertilizerApplication,
    ChemicalStorage
}
