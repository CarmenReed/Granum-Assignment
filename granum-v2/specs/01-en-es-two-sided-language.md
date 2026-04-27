<!-- v2.0 release · 2026-04-26 -->
# ENHANCEMENT-1: English / Spanish Two-Sided Language Model

## The customer problem

The LMN Crew app is already marketed as bilingual English / Spanish. Six independent Granum pages corroborate the language pair: LMN Starter ("bilingual support for field teams"), the LMN Crew product page ("Available in English and Spanish"), and four vertical-market pages (full-service landscaping, design-build, snow-ice, landscape maintenance) all describe the Crew app as "English/Spanish, offline capable."[^1] Greenius reinforces the pair: "Every course and interface is translated so your crews can train in their preferred language" with courses available in English and Spanish.[^2] The U.S. Bureau of Labor Statistics groundskeeping data (SOC 37-3011) shows a substantially higher Hispanic/Latino workforce share than the U.S. workforce average,[^3] which is consistent with Granum's bilingual marketing posture.

Today the cleanup endpoint has one language axis: whatever the LLM is prompted to produce. That collapses the operator's reporting language and the crew's input language into the same channel. An English-speaking operator running a Spanish-speaking crew gets either crew Spanish echoed into the cleaned output, or a generically-translated cleanup that loses the operator's native tone. Both outcomes are friction Granum can absorb at the platform layer, especially since LMN already markets the bilingual surface that this enhancement extends.

A two-sided language model splits the input language from the output language. Crew can write in any supported language; the operator selects the output language once at the account level (English or Spanish at launch, matching LMN's bilingual marketing). The cleanup pass produces output in the operator's selected language regardless of input language.

## The proposed solution

Add a per-operator language preference (English or Spanish at launch) and a per-request crew-language detection step. The orchestrator reads the operator's preference, detects the crew's input language, and prompts the LLM to produce the cleaned output in the operator's language. No manual translation step on either side. The pair matches LMN's existing bilingual marketing.[^1]

## Why this earns its complexity

LMN's bilingual surface is already a customer promise. The current cleanup architecture treats that promise as the model's problem: prompt it well enough and it figures out the language. The failure mode is silent language drift, where the model picks the wrong output language and there is no API surface to distinguish "the model misread the note" from "the model wrote in the wrong language." Splitting input language from output language makes the contract explicit and testable, so multi-language failure modes become observable rather than disguised as generic prompt-quality problems.

## Architecture sketch

- New enum: `OperatorLanguagePreference { English, Spanish }`
- New record: `CrewLanguageDetection(string DetectedLanguageCode, double Confidence)`
- New interface: `ILanguageOrchestrator` with `DetectCrewLanguageAsync` and `TranslateToOperatorLanguageAsync`
- New stub class: `StubLanguageOrchestrator` implementing the interface, both methods throwing `NotImplementedException`
- Wiring (NOT in this branch): operator preference would be persisted on an account/tenant record; the orchestrator would be invoked from `EnhancementService.EnhanceAsync` between PII guard and LLM call

## Stub scope (what's in this branch)

Committed:
- Enum `OperatorLanguagePreference` (English, Spanish)
- Record `CrewLanguageDetection`
- Interface `ILanguageOrchestrator`
- `StubLanguageOrchestrator` with both methods throwing `NotImplementedException`

Not committed (deliberately):
- DI registration in `Program.cs`
- Any change to `EnhancementService`
- Any persistence layer for operator language preference
- Any prompt template changes
- Any test coverage beyond confirming the project still builds

## Open questions

1. Is operator language preference per-tenant, per-user, or per-API-key? Each LMN, SingleOps, and Greenius product still has a separate login portal,[^4] which makes per-product configuration plausible but raises a unification question on top.
2. Do we expose detected crew language back to the caller (audit trail), or keep it internal to the orchestrator?
3. For crew languages outside the supported set (Portuguese, Vietnamese, Mandarin), do we fail loud, fall back to English, or let the LLM attempt it with a confidence flag?
4. The marketing surface treats Canada and the U.S. as one English-speaking North American market with Spanish for the field. If a Canadian customer requests French support inside the help-center surface (out of scope for this research pass), where does that change land in this contract?

## Greppable markers

- `granum-v2/proposed-code/Models/OperatorLanguagePreference.cs` -- `// ENHANCEMENT-1` header
- `granum-v2/proposed-code/Models/CrewLanguageDetection.cs` -- `// ENHANCEMENT-1` header
- `granum-v2/proposed-code/Services/ILanguageOrchestrator.cs` -- `// ENHANCEMENT-1` header
- `granum-v2/proposed-code/Services/StubLanguageOrchestrator.cs` -- `// ENHANCEMENT-1` header and method-level markers

Find them: `grep -rn "ENHANCEMENT-1" granum-v2/`

## Footnotes

[^1]: LMN's bilingual support is documented as English / Spanish across six Granum pages including LMN Starter ("bilingual support for field teams"), the LMN Crew app product page ("Available in English and Spanish"), the landscape-maintenance vertical, the full-service-landscaping vertical, the design-build vertical, and the snow-ice vertical. Source URLs cataloged in GRANUM_PORTFOLIO_FACTS.md fetch log: granum.com/lmn/starter/, granum.com/lmn/lmn-crew/, granum.com/landscape-maintenance/, granum.com/who-we-serve/full-service-landscaping/, granum.com/who-we-serve/landscaping-design-build/, granum.com/who-we-serve/snow-ice/. **Verified Granum product fact (source: granum.com/lmn/starter/ and 5 cross-references).**

[^2]: Greenius states "Every course and interface is translated so your crews can train in their preferred language" and "Courses are available in both English and Spanish," with per-category confirmation across equipment training, health and safety, and specialty training. **Verified Granum product fact (source: granum.com/greenius/ and granum.com/greenius/courses/).**

[^3]: U.S. Bureau of Labor Statistics Occupational Employment and Wage Statistics for "Landscaping and Groundskeeping Workers" (SOC 37-3011) reports a substantially higher Hispanic/Latino workforce share than the U.S. workforce average. **Public statistical pattern. Not validated against Granum's internal customer data.**

[^4]: Each Granum product still has its own login portal post-merger: my.golmn.com, app.singleops.com, new.gogreenius.com. The Granum brand was launched October 8, 2025 and is roughly six months old at the time of this spec. **Verified Granum product fact (source: granum.com/ and granum.com/news/, per GRANUM_PORTFOLIO_FACTS.md cross-product themes).**
