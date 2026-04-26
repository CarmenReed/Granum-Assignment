# Granum v2: Five Proposed Enhancements (Showcase, Not Compiled)

This folder is a **separate showcase** of five product upgrades I would build next on top of the take-home assignment. The assignment itself (under [../src/](../src/)) is intentionally untouched -- pristine deliverable, no scope creep.

What's here:
- [specs/](specs/) -- one Markdown spec per enhancement, with footnoted regulatory/demographic claims and explicit epistemic-status tags
- [proposed-code/](proposed-code/) -- static `.cs` files showing the contract shapes (interfaces, records, enums, stub classes), plus `.patch` files for the two existing-file modifications
- This README -- the walk-through map for the Monday pair-code session

What's deliberately NOT here:
- Compiled stubs in `src/`. Anything in `proposed-code/` is a preview; the build path stays clean. See [proposed-code/README.md](proposed-code/README.md).
- Wiring (DI registration, prompt template changes, persistence). The point is the shape and the open questions, not the implementation.

Greppable everywhere: `grep -rn "ENHANCEMENT-[1-5]" granum-v2/`

## How to demo each enhancement

### ENHANCEMENT-1: Two-Sided Language Model

- **Intro line:** "LMN serves Canadian operators where French is non-negotiable under Bill 101 and Bill 96. U.S. landscape crews are heavily Hispanic per BLS. The current pipeline has one language axis. This stub splits crew-input language detection from operator-output language preference."
- **Files to open (in order):**
  - [specs/01-two-sided-language.md](specs/01-two-sided-language.md) -- spec, customer problem, footnotes
  - [proposed-code/Models/OperatorLanguagePreference.cs](proposed-code/Models/OperatorLanguagePreference.cs) -- the enum
  - [proposed-code/Models/CrewLanguageDetection.cs](proposed-code/Models/CrewLanguageDetection.cs) -- the detection record
  - [proposed-code/Services/ILanguageOrchestrator.cs](proposed-code/Services/ILanguageOrchestrator.cs) -- contract
  - [proposed-code/Services/StubLanguageOrchestrator.cs:23](proposed-code/Services/StubLanguageOrchestrator.cs:23) -- stubbed methods
- **Greppable:** `grep -rn "ENHANCEMENT-1" granum-v2/`
- **Stub vs real:** Contract is shaped (enum, record, interface, stub class). Wiring is deferred (no DI registration, no change to `EnhancementService`, no persistence layer for operator preference, no prompt template changes).
- **Pivot question:** "If we built this for real, would operator language live on a tenant record, a user record, or an API key? That's the first decision and it shapes everything downstream."

### ENHANCEMENT-2: Compliance Flagging

- **Intro line:** "Pesticide and fertilizer applications hit EPA Worker Protection Standard and Health Canada PCPA record requirements. The current pipeline runs one prompt for everything; the moment a sentence enters a regulated reporting path, that single prompt is the wrong tool. This stub adds the detection contract."
- **Files to open (in order):**
  - [specs/02-compliance-flagging.md](specs/02-compliance-flagging.md) -- spec with EPA WPS / Health Canada PCPA footnotes
  - [proposed-code/Models/RegulatedDomain.cs](proposed-code/Models/RegulatedDomain.cs) -- the enum
  - [proposed-code/Services/IComplianceFlaggingService.cs](proposed-code/Services/IComplianceFlaggingService.cs) -- contract
  - [proposed-code/Services/StubComplianceFlaggingService.cs:25](proposed-code/Services/StubComplianceFlaggingService.cs:25) -- detection returns None, enforcement throws
- **Greppable:** `grep -rn "ENHANCEMENT-2" granum-v2/`
- **Stub vs real:** Contract is shaped. Detection logic, domain-specific prompt templates, jurisdiction handling, and DI wiring are all deferred. Stub detection deliberately returns `None` so the stub is safe to register without changing runtime behavior.
- **Pivot question:** "Is detection an LLM classifier (high accuracy, latency cost) or a regex bootstrap (low cost, false negatives)? My instinct is regex first as a fast pre-filter, then LLM. What's your call?"

### ENHANCEMENT-3: A/B Prompt Testing Harness

- **Intro line:** "Code changes go through PR, tests, and CI. Prompt changes today go through 'I edited the file and it looked fine.' That gap is silent regression risk. This stub is the same governance discipline applied to the prompt itself."
- **Files to open (in order):**
  - [specs/03-prompt-ab-testing.md](specs/03-prompt-ab-testing.md) -- spec
  - [specs/03-fixtures/sample-historical-inputs.json](specs/03-fixtures/sample-historical-inputs.json) -- a real artifact, three hand-written inputs, no PII
  - [proposed-code/Models/PromptVariant.cs](proposed-code/Models/PromptVariant.cs) -- the variant record
  - [proposed-code/Models/PromptComparisonResult.cs](proposed-code/Models/PromptComparisonResult.cs) -- per-input control vs candidate
  - [proposed-code/Services/IPromptTestHarness.cs](proposed-code/Services/IPromptTestHarness.cs) -- contract
  - [proposed-code/Services/StubPromptTestHarness.cs:27](proposed-code/Services/StubPromptTestHarness.cs:27) -- stubbed `RunComparisonAsync`
- **Greppable:** `grep -rn "ENHANCEMENT-3" granum-v2/`
- **Stub vs real:** Contract is shaped. The fixture file is a real artifact you can open and read. The diff algorithm choice (line-diff, token-diff, semantic-diff) is deliberately deferred -- it's a real choice and a fixture file is a better demo than a half-baked diff.
- **Pivot question:** "Would you wire this as a CLI tool the prompt author runs locally, or as a CI gate that blocks deploy? Different tradeoffs, both valid."

### ENHANCEMENT-4: PII Expansion

- **Intro line:** "The existing PII guard catches email and U.S. phone. Those are the high-volume categories. The categories that are individually rarer and individually more damaging when they leak are SSN, address, credit card, DOB. The patch adds the patterns; wiring `Check()`/`Redact()` is a 3-line change per pattern that I deliberately staged as a separate step because each pattern has a collision risk that needs its own test."
- **Files to open (in order):**
  - [specs/04-pii-expansion.md](specs/04-pii-expansion.md) -- spec
  - [proposed-code/patches/04-PiiGuardService.patch](proposed-code/patches/04-PiiGuardService.patch) -- header block + 4 new compiled regex fields, captured as a unified diff
  - [specs/04-test-cases.md](specs/04-test-cases.md) -- test cases that must land alongside wiring, including the load-bearing `Ssn_DoesNotCollideWithPhone` collision test
- **Greppable:** `grep -rn "ENHANCEMENT-4" granum-v2/`
- **Stub vs real:** Patterns are defined and ready to wire. The patch was build-validated against the live assignment files (`dotnet build` clean, `dotnet test` 22/22 green) before being extracted as a `.patch`. Wiring is a 3-line addition per pattern (4 lines for credit card because of Luhn).
- **Pivot question:** "Want me to apply this patch and wire one of them live right now? SSN is the safest -- the hyphenated form has no collision with phone. Credit card needs a Luhn check, that's the more interesting one to walk through together."

### ENHANCEMENT-5: Webhook Out on Guardrail Hit

- **Intro line:** "Granum's value compounds when products in the portfolio can react to each other. Today, when the gate flags PII or low confidence, the rejection lives inside one service. LMN dispatch, SingleOps jobs, Greenius scheduling all stay blind. This stub is the integration surface."
- **Files to open (in order):**
  - [specs/05-webhook-out.md](specs/05-webhook-out.md) -- spec
  - [proposed-code/Models/GuardrailEvent.cs](proposed-code/Models/GuardrailEvent.cs) -- the event payload
  - [proposed-code/Services/IWebhookEmitter.cs](proposed-code/Services/IWebhookEmitter.cs) -- contract
  - [proposed-code/Services/StubWebhookEmitter.cs:22](proposed-code/Services/StubWebhookEmitter.cs:22) -- stubbed `EmitAsync`
  - [proposed-code/patches/05-EnhancementService.patch](proposed-code/patches/05-EnhancementService.patch) -- single 3-line comment marker at the PII rejection branch identifying the wiring point, captured as a unified diff. NO code change.
- **Greppable:** `grep -rn "ENHANCEMENT-5" granum-v2/`
- **Stub vs real:** Contract is shaped. The wiring point is annotated with a comment marker (in the patch), but the call into the emitter is deliberately not added -- HMAC signing, retry/backoff, per-tenant URL config, and async delivery channel are all real decisions that come before code.
- **Pivot question:** "If we wired this, would you put the emit on a background channel right away, or would you take the latency hit on the request path until usage proves it's a problem?"

## What this demo proves

- I think about products from multiple architectural axes simultaneously: language, regulation, governance, PII, and integration are five different cuts through the same pipeline.
- Stubs are deliberate. Earn-your-complexity at the meta level: don't implement until validated. Each spec carries open questions that are the conversation, not the conclusion.
- The deliverable stays pristine. The assignment under [../src/](../src/) is exactly what was submitted; this folder is a separate "what next" pitch, not a modification of the deliverable.
- I can hand off any one of these to a junior engineer tomorrow because the spec is written, the contract is shaped, the wiring point is identified, and the test cases (where applicable) are listed.

## What NOT to oversell

- These are static contracts and patches, not live wired-up code. Every stub method throws `NotImplementedException` (except the compliance detection stub, which returns `None` deliberately). The PII expansion lives as a `.patch` file, not as a live source-tree change. None of these enhancements ship runtime behavior on this branch.
- I did NOT evaluate alternatives for any of them. The architectures sketched in the specs are one reasonable shape each, not a comparative analysis.
- The customer-pain framing is informed reasoning grounded in public data and industry patterns (Bill 101/96, EPA WPS, Health Canada PCPA, BLS occupational data, SSA SSN format, Granum's stated multi-product strategy). It is not validated customer interviews. Every spec carries footnotes that mark each claim's epistemic status (regulatory fact / public statistical pattern / industry pattern recognition / unvalidated assumption).
