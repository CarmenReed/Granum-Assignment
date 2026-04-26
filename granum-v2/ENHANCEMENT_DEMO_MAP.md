# Enhancement Branch Walk-through

## Branch

`granum-assignment-v2`

This folder is the portfolio-anchored rebuild after a Phase 1 research pass that crawled 34 Granum pages. The Phase 1 research output is alongside this file at [GRANUM_PORTFOLIO_FACTS.md](GRANUM_PORTFOLIO_FACTS.md).

Greppable everywhere: `grep -rn "ENHANCEMENT-[1-5]" granum-v2/`

## Each enhancement

### ENHANCEMENT-1: English / Spanish Two-Sided Language Model

- **Summary:** "LMN Crew app is already marketed as bilingual English / Spanish across six Granum pages. The current pipeline has one language axis. This stub splits crew-input language detection from operator-output language preference using the language pair Granum already markets."
- **Files:**
  - [specs/01-en-es-two-sided-language.md](specs/01-en-es-two-sided-language.md) -- spec, customer problem, footnotes
  - [proposed-code/Models/OperatorLanguagePreference.cs](proposed-code/Models/OperatorLanguagePreference.cs) -- enum (English, Spanish)
  - [proposed-code/Models/CrewLanguageDetection.cs](proposed-code/Models/CrewLanguageDetection.cs) -- BCP 47 record
  - [proposed-code/Services/ILanguageOrchestrator.cs](proposed-code/Services/ILanguageOrchestrator.cs) -- contract
  - [proposed-code/Services/StubLanguageOrchestrator.cs](proposed-code/Services/StubLanguageOrchestrator.cs) -- stubbed methods
- **Greppable:** `grep -rn "ENHANCEMENT-1" granum-v2/`
- **Status:** Contract is shaped. Wiring is deferred (DI, persistence, prompt templates).
- **Open question:** "If we built this for real, would operator language live on a tenant record, a user record, or an API key? Each Granum product still has its own login portal post-merger, so this question has more layers than it might at first."

### ENHANCEMENT-2: PHC Application Record Mode for SingleOps Tree Inventory

- **Summary:** "SingleOps Tree Inventory already captures PHC treatments. PHC work is pesticide application, which carries record-keeping obligations under EPA Worker Protection Standard and Health Canada PCPA. The marketing surface captures the field shape but not the compliance-defensible record. This stub adds the detection contract for a regulated cleanup mode."
- **Files:**
  - [specs/02-phc-tree-inventory-compliance.md](specs/02-phc-tree-inventory-compliance.md) -- spec with Tree Inventory citations and EPA WPS / Health Canada PCPA footnotes
  - [proposed-code/Models/RegulatedDomain.cs](proposed-code/Models/RegulatedDomain.cs) -- the enum
  - [proposed-code/Services/IComplianceFlaggingService.cs](proposed-code/Services/IComplianceFlaggingService.cs) -- contract
  - [proposed-code/Services/StubComplianceFlaggingService.cs](proposed-code/Services/StubComplianceFlaggingService.cs) -- detection returns None, enforcement throws
- **Greppable:** `grep -rn "ENHANCEMENT-2" granum-v2/`
- **Status:** Contract is shaped. Detection logic, domain-specific prompt templates, jurisdiction handling, and DI wiring into Tree Inventory are deferred.
- **Open question:** "Is detection an LLM classifier or a regex bootstrap first? My instinct is regex first as a fast pre-filter, then LLM."

### ENHANCEMENT-3: Zapier Trigger on Guardrail Hit

- **Summary:** "LMN Professional and Enterprise tiers list Zapier with 'Over 6,000 apps.' Phase 1 found no native webhook documentation anywhere. This stub exposes guardrail events as a Zapier instant trigger, reusing the integration posture Granum already endorses."
- **Files:**
  - [specs/03-zapier-trigger-on-guardrail-hit.md](specs/03-zapier-trigger-on-guardrail-hit.md) -- spec with Zapier-on-LMN footnote
  - [proposed-code/Models/GuardrailEvent.cs](proposed-code/Models/GuardrailEvent.cs) -- the event payload
  - [proposed-code/Services/IZapierTriggerEmitter.cs](proposed-code/Services/IZapierTriggerEmitter.cs) -- contract
  - [proposed-code/Services/StubZapierTriggerEmitter.cs](proposed-code/Services/StubZapierTriggerEmitter.cs) -- stubbed `EmitAsync`
  - [proposed-code/patches/03-EnhancementService.patch](proposed-code/patches/03-EnhancementService.patch) -- 3-line wiring-point marker, captured as unified diff. NO code change in `src/`.
- **Greppable:** `grep -rn "ENHANCEMENT-3" granum-v2/`
- **Status:** Contract is shaped. The patch was build-validated against live assignment files (22/22 tests green at extraction).
- **Open question:** "Authentication: API-key (simpler, fits the existing LMN posture) or OAuth (richer permissions, more setup)?"

### ENHANCEMENT-4: Greenius Training Trigger on Guardrail Hit

- **Summary:** "Greenius training is the only documented inter-product integration in the Granum portfolio: it is surfaced inside the LMN Crew app. When the gate flags a category, the existing Greenius integration is the natural teaching moment. This stub auto-assigns the matching course to the crew member, closing the learning loop inside the surface they already use."
- **Files:**
  - [specs/04-greenius-training-trigger.md](specs/04-greenius-training-trigger.md) -- spec citing Greenius's 150+ courses and the LMN-Crew-app integration
  - [proposed-code/Models/GuardrailCategory.cs](proposed-code/Models/GuardrailCategory.cs) -- the category enum
  - [proposed-code/Services/IGreeniusTrainingAssigner.cs](proposed-code/Services/IGreeniusTrainingAssigner.cs) -- contract
  - [proposed-code/Services/StubGreeniusTrainingAssigner.cs](proposed-code/Services/StubGreeniusTrainingAssigner.cs) -- stubbed `AssignAsync`
- **Greppable:** `grep -rn "ENHANCEMENT-4" granum-v2/`
- **Status:** Contract is shaped. The Greenius course-assignment surface itself is an open question (no public API documented).
- **Open question:** "Does Greenius expose a course-assignment API today? If yes, this is a one-week wire-up. If not, the fallback is an in-app notification."

### ENHANCEMENT-5: Prompt A/B Testing Harness (Conditional)

- **Summary:** "Conditional. Phase 1 found zero AI / ML claims across 34 Granum pages. Marketing posture is consistent: 'automated,' 'algorithmic,' 'data-driven,' never 'AI.' If Granum is shipping LLM features today, this harness is high-leverage governance. If not yet, it is solving a problem that doesn't exist yet. The first open question on the spec is exactly that."
- **Files:**
  - [specs/05-prompt-ab-testing-harness.md](specs/05-prompt-ab-testing-harness.md) -- spec with the conditional framing
  - [specs/05-fixtures/sample-historical-inputs.json](specs/05-fixtures/sample-historical-inputs.json) -- 3 hand-written sample inputs, no PII
  - [proposed-code/Models/PromptVariant.cs](proposed-code/Models/PromptVariant.cs)
  - [proposed-code/Models/PromptComparisonResult.cs](proposed-code/Models/PromptComparisonResult.cs)
  - [proposed-code/Services/IPromptTestHarness.cs](proposed-code/Services/IPromptTestHarness.cs)
  - [proposed-code/Services/StubPromptTestHarness.cs](proposed-code/Services/StubPromptTestHarness.cs)
- **Greppable:** `grep -rn "ENHANCEMENT-5" granum-v2/`
- **Status:** Contract is shaped. Fixture is real. Diff algorithm choice is deferred on purpose.
- **Open question:** "Is Granum shipping LLM-driven features today, or planning to in the near term? The harness is conditional on the answer."

## What this branch shows

- I think about products from multiple architectural axes simultaneously: language, regulation, integration, training, and governance.
- I do read-only research before writing specs. An earlier draft made assumptions (French/Quebec for Enhancement-1, fictional "LMN dispatch" / "Greenius scheduling" consumers) that the Phase 1 research surfaced as wrong. The current build is the rewrite on top of verified Granum product facts.
- Stubs are deliberate. Earn-your-complexity at the meta level: don't implement before validating which axis matters first.
- The deliverable stays pristine. `src/` is byte-for-byte the original take-home assignment.

## Honest constraints

- These are static contracts and patches, not live wired-up code. Every stub method throws `NotImplementedException` (except the compliance detection stub, which returns `None` deliberately). Enhancement-3's wiring lives as a `.patch` file, not a live source-tree change.
- I did NOT evaluate alternatives. The architectures are one reasonable shape each, not a comparative analysis.
- Customer-pain framing is grounded in Granum's public marketing surface (per `GRANUM_PORTFOLIO_FACTS.md`), not validated customer research. Help-center pages were not crawled.
- Enhancement-5 is conditional on whether Granum ships LLM features today. Treat it as an open architectural question, not a recommendation.
