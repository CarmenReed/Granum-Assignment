// ENHANCEMENT-1: Two-sided language model
// Spec: enhancements/01-two-sided-language.md
// Status: STUB ONLY. Not wired to runtime.
// Demo: this file exists so Monday's deep-dive can show the
//       architectural shape without committing implementation
//       time before validation.

namespace Api.Models;

/// <summary>
/// Operator-selected output language for cleaned notes.
/// Persisted per-tenant or per-account in real implementation.
/// English and French at launch (US + Canadian market today).
/// </summary>
public enum OperatorLanguagePreference
{
    English,
    French
}
