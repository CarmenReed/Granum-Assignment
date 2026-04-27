<!-- v2.0 release · 2026-04-26 -->
# Granum v2: Five Portfolio-Anchored Enhancements (Showcase, Not Compiled)

This folder is a **separate showcase** of five product upgrades I would build next on top of the take-home assignment. The assignment itself (under [../src/](../src/)) is intentionally untouched -- pristine deliverable, no scope creep.

Every enhancement here is anchored to a verified Granum product or feature documented in [GRANUM_PORTFOLIO_FACTS.md](GRANUM_PORTFOLIO_FACTS.md) (the Phase 1 portfolio research that crawled 34 Granum pages).

What's here:
- [specs/](specs/) -- one Markdown spec per enhancement, with footnoted regulatory and Granum-product claims and explicit epistemic-status tags
- [proposed-code/](proposed-code/) -- static `.cs` files showing the contract shapes (interfaces, records, enums, stub classes), plus a `.patch` file for the one existing-file modification (Enhancement-3's Zapier wiring marker)
- This README -- the walk-through map for this folder

What's deliberately NOT here:
- Compiled stubs in `src/`. Anything in `proposed-code/` is a preview; the build path stays clean. See [proposed-code/README.md](proposed-code/README.md).
- Wiring (DI registration, prompt template changes, persistence). The point is the shape and the open questions, not the implementation.

Greppable everywhere: `grep -rn "ENHANCEMENT-[1-5]" granum-v2/`

## Each enhancement

### ENHANCEMENT-1: English / Spanish Two-Sided Language Model

- **Intro line:** "LMN Crew app is already marketed as bilingual English / Spanish across six Granum pages. Greenius offers parallel English / Spanish course sets. Today the cleanup endpoint has one language axis. This stub splits crew-input language detection from operator-output language preference using the language pair Granum already markets."
- **Files to open (in order):**
  - [specs/01-en-es-two-sided-language.md](specs/01-en-es-two-sided-language.md) -- spec, customer problem, footnotes citing the 6 corroborating Granum pages
  - [proposed-code/Models/OperatorLanguagePreference.cs](proposed-code/Models/OperatorLanguagePreference.cs) -- enum (English, Spanish)
  - [proposed-code/Models/CrewLanguageDetection.cs](proposed-code/Models/CrewLanguageDetection.cs) -- BCP 47 record
  - [proposed-code/Services/ILanguageOrchestrator.cs](proposed-code/Services/ILanguageOrchestrator.cs) -- contract
  - [proposed-code/Services/StubLanguageOrchestrator.cs](proposed-code/Services/StubLanguageOrchestrator.cs) -- stubbed methods
- **Greppable:** `grep -rn "ENHANCEMENT-1" granum-v2/`
- **Stub vs real:** Contract is shaped (enum, record, interface, stub class). Wiring is deferred (no DI registration, no change to `EnhancementService`, no persistence layer for operator preference, no prompt template changes).
- **Open question:** "If we built this for real, would operator language live on a tenant record, a user record, or an API key? Each Granum product still has its own login portal post-merger, so this question has more layers than it might at first."

### ENHANCEMENT-2: PHC Application Record Mode for SingleOps Tree Inventory

- **Intro line:** "SingleOps Tree Inventory already captures PHC treatments and PHC recommendations on the marketing surface. PHC work is pesticide application, which carries record-keeping obligations under EPA Worker Protection Standard (40 CFR Part 170) and Health Canada PCPA. SingleOps captures the field shape but not the compliance-defensible record. This stub adds the detection contract for a regulated-cleanup mode."
- **Files to open (in order):**
  - [specs/02-phc-tree-inventory-compliance.md](specs/02-phc-tree-inventory-compliance.md) -- spec with Tree Inventory citations and EPA WPS / Health Canada PCPA footnotes
  - [proposed-code/Models/RegulatedDomain.cs](proposed-code/Models/RegulatedDomain.cs) -- the enum
  - [proposed-code/Services/IComplianceFlaggingService.cs](proposed-code/Services/IComplianceFlaggingService.cs) -- contract
  - [proposed-code/Services/StubComplianceFlaggingService.cs](proposed-code/Services/StubComplianceFlaggingService.cs) -- detection returns None, enforcement throws
- **Greppable:** `grep -rn "ENHANCEMENT-2" granum-v2/`
- **Stub vs real:** Contract is shaped. Detection logic, domain-specific prompt templates, jurisdiction handling, and DI wiring into SingleOps Tree Inventory's PHC entry path are all deferred. Stub detection deliberately returns `None` so the stub is safe to register without changing runtime behavior.
- **Open question:** "Is detection an LLM classifier (high accuracy, latency cost) or a regex bootstrap (low cost, false negatives)? My instinct is regex first as a fast pre-filter, then LLM. What's your call?"

### ENHANCEMENT-3: Zapier Trigger on Guardrail Hit

- **Intro line:** "LMN Professional and Enterprise tiers list Zapier as a marketed integration with 'Over 6,000 apps.' That is the integration lane Granum already endorses, and the Phase 1 research found no native webhook documentation anywhere. This stub exposes guardrail events as a Zapier instant trigger, reusing the existing integration posture instead of inventing a new native webhook surface."
- **Files to open (in order):**
  - [specs/03-zapier-trigger-on-guardrail-hit.md](specs/03-zapier-trigger-on-guardrail-hit.md) -- spec with Zapier-on-LMN footnote
  - [proposed-code/Models/GuardrailEvent.cs](proposed-code/Models/GuardrailEvent.cs) -- the event payload
  - [proposed-code/Services/IZapierTriggerEmitter.cs](proposed-code/Services/IZapierTriggerEmitter.cs) -- contract
  - [proposed-code/Services/StubZapierTriggerEmitter.cs](proposed-code/Services/StubZapierTriggerEmitter.cs) -- stubbed `EmitAsync`
  - [proposed-code/patches/03-EnhancementService.patch](proposed-code/patches/03-EnhancementService.patch) -- single 3-line wiring-point marker at the PII rejection branch (no code change in `src/`)
- **Greppable:** `grep -rn "ENHANCEMENT-3" granum-v2/`
- **Stub vs real:** Contract is shaped. The wiring point is captured as a unified diff against the live `src/Api/Services/EnhancementService.cs`. The patch was build-validated (`dotnet build` clean, 22/22 tests green) before being extracted; `src/` is unchanged.
- **Open question:** "Authentication: API-key (simpler, fits the existing LMN posture) or OAuth (richer permissions, more setup)? And do we tier-gate this to LMN Professional+ to match the existing Zapier tier policy, or expose it on every tier as a guardrail-specific feature?"

### ENHANCEMENT-4: Greenius Training Trigger on Guardrail Hit

- **Intro line:** "Greenius is the only documented inter-product integration in the Granum portfolio: training is surfaced inside the LMN Crew app. When the inference gate flags a category (PII leak, compliance miss, low confidence), the existing Greenius integration is the natural teaching moment. This stub auto-assigns the matching Greenius course to the crew member, closing the learning loop inside the surface they already use."
- **Files to open (in order):**
  - [specs/04-greenius-training-trigger.md](specs/04-greenius-training-trigger.md) -- spec citing Greenius's 150+ courses and the LMN-Crew-app integration
  - [proposed-code/Models/GuardrailCategory.cs](proposed-code/Models/GuardrailCategory.cs) -- the category enum
  - [proposed-code/Services/IGreeniusTrainingAssigner.cs](proposed-code/Services/IGreeniusTrainingAssigner.cs) -- contract
  - [proposed-code/Services/StubGreeniusTrainingAssigner.cs](proposed-code/Services/StubGreeniusTrainingAssigner.cs) -- stubbed `AssignAsync`
- **Greppable:** `grep -rn "ENHANCEMENT-4" granum-v2/`
- **Stub vs real:** Contract is shaped. The assignment surface itself is an open question: the public marketing surface does not document a Greenius course-assignment API. If Granum has one in the help center, wiring is straightforward. If not, the fallback is a "suggested course" notification inside the LMN Crew app inbox.
- **Open question:** "Does Greenius expose a course-assignment API today? If yes, this is a one-week wire-up. If no, the fallback is an in-app notification, which is a different conversation."

### ENHANCEMENT-5: Prompt A/B Testing Harness (Conditional)

- **Intro line:** "This one is conditional. Phase 1 research found zero AI / ML claims across 34 Granum pages. Marketing posture is consistently 'automated' and 'algorithmic,' never 'AI.' If Granum is shipping LLM features today, this harness is high-leverage governance. If not yet, it is solving a problem that doesn't exist yet. The first open question on the spec is exactly that conditional."
- **Files to open (in order):**
  - [specs/05-prompt-ab-testing-harness.md](specs/05-prompt-ab-testing-harness.md) -- spec with the conditional framing
  - [specs/05-fixtures/sample-historical-inputs.json](specs/05-fixtures/sample-historical-inputs.json) -- 3 hand-written sample inputs, no PII
  - [proposed-code/Models/PromptVariant.cs](proposed-code/Models/PromptVariant.cs)
  - [proposed-code/Models/PromptComparisonResult.cs](proposed-code/Models/PromptComparisonResult.cs)
  - [proposed-code/Services/IPromptTestHarness.cs](proposed-code/Services/IPromptTestHarness.cs)
  - [proposed-code/Services/StubPromptTestHarness.cs](proposed-code/Services/StubPromptTestHarness.cs)
- **Greppable:** `grep -rn "ENHANCEMENT-5" granum-v2/`
- **Stub vs real:** Contract is shaped. The fixture file is a real artifact you can open. The diff algorithm choice (line-diff, token-diff, semantic-diff) is deliberately deferred because it is a real choice and a fixture file is a better demo than a half-baked diff.
- **Open question:** "Is Granum shipping LLM-driven features today, or planning to in the near term? The harness is conditional on the answer. If yes, the conversation is 'how do you review prompt changes today, and where would this slot in.' If no or not yet, the conversation is 'should this be the next thing on the AI roadmap.'"

## What this folder demonstrates

- I think about products from multiple architectural axes simultaneously: language, regulation, integration, training, and governance are five different cuts through the same pipeline, all anchored to verified Granum products from the Phase 1 portfolio research.
- I do read-only research before writing specs. An earlier draft of these enhancements made assumptions (French/Quebec for Enhancement-1, fictional "LMN dispatch" / "Greenius scheduling" consumers for the old Enhancement-5) that the research surfaced as wrong. The current build is the rewrite on top of verified Granum product facts.
- Stubs are deliberate. Earn-your-complexity at the meta level: don't implement before validating which axis matters first. Each spec carries open questions that are the conversation, not the conclusion.
- The deliverable stays pristine. The assignment under [../src/](../src/) is exactly what was submitted; this folder is a separate "what next" pitch, not a modification of the deliverable.
- Each one is hand-off-ready: the spec is written, the contract is shaped, the wiring point is identified, and the open questions are listed.

## Honest constraints

- These are static contracts and patches, not live wired-up code. Every stub method throws `NotImplementedException` (except the compliance detection stub, which returns `None` deliberately). The Zapier wiring lives as a `.patch` file, not as a live source-tree change. None of these enhancements ship runtime behavior on this branch.
- I did NOT evaluate alternatives for any of them. The architectures sketched in the specs are one reasonable shape each, not a comparative analysis.
- Customer-pain framing is grounded in Granum's public marketing surface (per [GRANUM_PORTFOLIO_FACTS.md](GRANUM_PORTFOLIO_FACTS.md)), not validated against Granum's internal customer data. Help-center pages (support.golmn.com, docs.singleops.com, support.gogreenius.com) were not crawled. Anything that depends on "the website does not say X" should be verified against the help center before being a load-bearing argument.
- Enhancement-5 (prompt A/B harness) is conditional on whether Granum ships LLM features today. The portfolio research did not establish that, and the marketing posture is conservative ("automated," "algorithmic," "data-driven," never "AI"). Treat it as an open architectural question, not a recommendation.
