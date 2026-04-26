# ENHANCEMENT-1: Two-Sided Language Model

## The customer problem

Granum customers operate across North America with a workforce profile that is rarely monolingual. LMN (Landscape Management Network) is headquartered in Ontario and supports Canadian operators, where French is non-negotiable for the Quebec market under Bill 101 and Bill 96.[^1] On the U.S. side, the landscape services industry is heavily Hispanic-staffed per Bureau of Labor Statistics data on grounds maintenance occupations,[^2] which means crew-side input frequently arrives in Spanish even when the operator manages the business in English.

Today the enhancement endpoint has one language axis: whatever the LLM is prompted to produce. That collapses the operator's reporting language and the crew's input language into the same channel, which forces either (a) the operator to rewrite crew notes by hand, or (b) the crew to write in a language they don't own. Both outcomes are friction Granum can absorb at the platform layer.

A two-sided language model splits the input language from the output language. Crew can write in any supported language; the operator selects the output language once at the account level (English or French at launch).[^3] The cleaning pass produces output in the operator's selected language regardless of input language.

## The proposed solution

Add a per-operator language preference (English or French at launch) and a per-request crew-language detection step. The orchestrator reads the operator's preference, detects the crew's input language, and prompts the LLM to produce the cleaned output in the operator's language. No manual translation step on either side.

## Why this earns its complexity

The current architecture's failure mode here is silent: a French-speaking operator in Quebec sees English output, an English-speaking operator sees Spanish input echoed back, and there is no surface in the API to distinguish "the model misread the note" from "the model wrote in the wrong language." A two-sided language pipeline makes the language contract explicit and testable. Without it, every multi-language failure mode looks like a generic prompt-quality problem.

## Architecture sketch

- New enum: `OperatorLanguagePreference { English, French }`
- New record: `CrewLanguageDetection(string DetectedLanguageCode, double Confidence)`
- New interface: `ILanguageOrchestrator` with `DetectCrewLanguageAsync` and `TranslateToOperatorLanguageAsync`
- New stub class: `StubLanguageOrchestrator` implementing the interface, both methods throwing `NotImplementedException`
- Wiring (NOT in this branch): operator preference would be persisted on an account/tenant record; the orchestrator would be invoked from `EnhancementService.EnhanceAsync` between PII guard and LLM call

## Stub scope (what's in this branch)

Committed:
- Enum `OperatorLanguagePreference`
- Record `CrewLanguageDetection`
- Interface `ILanguageOrchestrator`
- `StubLanguageOrchestrator` with both methods throwing `NotImplementedException`

Not committed (deliberately):
- DI registration in `Program.cs`
- Any change to `EnhancementService`
- Any persistence layer for operator language preference
- Any prompt template changes
- Any test coverage beyond confirming the project still builds

The stub's purpose is to make the contract visible. Wiring it is a separate decision that depends on Granum's authentication/tenant model.

## Open questions

1. Is operator language preference per-tenant, per-user, or per-API-key? Affects where it gets persisted.
2. Do we expose detected crew language back to the caller (audit trail), or keep it internal to the orchestrator?
3. For crew languages outside the supported set (e.g., Portuguese, Vietnamese), do we fail loud, fall back to English, or let the LLM attempt it with a confidence flag?
4. Does a Quebec operator's French output need to be specifically Quebec French (`fr-CA`), or is generic French acceptable for landscape-services vocabulary?

## Greppable markers

- `granum-v2/proposed-code/Models/OperatorLanguagePreference.cs` -- `// ENHANCEMENT-1` header
- `granum-v2/proposed-code/Models/CrewLanguageDetection.cs` -- `// ENHANCEMENT-1` header
- `granum-v2/proposed-code/Services/ILanguageOrchestrator.cs` -- `// ENHANCEMENT-1` header
- `granum-v2/proposed-code/Services/StubLanguageOrchestrator.cs` -- `// ENHANCEMENT-1` header and method-level markers

Find them: `grep -rn "ENHANCEMENT-1" granum-v2/`

## Footnotes

[^1]: Quebec's Charter of the French Language (Bill 101, 1977) and Bill 96 (2022) require French as the primary language of business communication, including written records and customer-facing materials, for organizations operating in Quebec. **Regulatory fact.**

[^2]: U.S. Bureau of Labor Statistics Occupational Employment and Wage Statistics for "Landscaping and Groundskeeping Workers" (SOC 37-3011) reports a substantially higher Hispanic/Latino workforce share than the U.S. workforce average. **Public statistical pattern. Not validated against Granum's internal customer data.**

[^3]: LMN is headquartered in Ontario, Canada, and its public marketing materials reference North American landscape operators including Canadian provinces. The framing of "Canadian customer base requires French support" is industry pattern recognition based on LMN's stated geographic footprint, not validated with Granum or LMN's internal customer breakdown. **Industry pattern recognition. Not validated against Granum's internal customer data.**
