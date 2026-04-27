// ENHANCEMENT-1: English / Spanish two-sided language model
// Spec: granum-v2/specs/01-en-es-two-sided-language.md
// Status: STUB ONLY. Not wired to runtime. Not part of build.
// v2.0 release · 2026-04-26

namespace Api.Models;

/// <summary>
/// Operator-selected output language for cleaned notes.
/// English and Spanish at launch, matching LMN Crew app's
/// marketed bilingual surface (6 corroborating Granum pages).
/// Persisted per-tenant or per-account in real implementation.
/// </summary>
public enum OperatorLanguagePreference
{
    English,
    Spanish
}
