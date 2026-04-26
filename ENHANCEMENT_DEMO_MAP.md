# Monday Demo: Enhancement Branch Walk-through

## Branch

`claude/granum-assignment-v2`

Six commits since `main`: one docs commit (the five specs) and five feature commits (one stub per enhancement). One demo-map commit on top of that.

Greppable everywhere: `grep -rn "ENHANCEMENT-[1-5]" src/ enhancements/`

## How to demo each enhancement

### ENHANCEMENT-1: Two-Sided Language Model

- **Intro line:** "LMN serves Canadian operators where French is non-negotiable under Bill 101 and Bill 96. U.S. landscape crews are heavily Hispanic per BLS. The current pipeline has one language axis. This stub splits crew-input language detection from operator-output language preference."
- **Files to open (in order):**
  - [enhancements/01-two-sided-language.md](enhancements/01-two-sided-language.md) -- spec, customer problem, footnotes
  - [src/Api/Models/OperatorLanguagePreference.cs](src/Api/Models/OperatorLanguagePreference.cs) -- the enum
  - [src/Api/Models/CrewLanguageDetection.cs](src/Api/Models/CrewLanguageDetection.cs) -- the detection record
  - [src/Api/Services/ILanguageOrchestrator.cs](src/Api/Services/ILanguageOrchestrator.cs) -- contract
  - [src/Api/Services/StubLanguageOrchestrator.cs:23](src/Api/Services/StubLanguageOrchestrator.cs:23) -- stubbed methods
- **Greppable:** `grep -rn "ENHANCEMENT-1" src/ enhancements/`
- **Stub vs real:** Contract is committed (enum, record, interface, stub class). Wiring is deferred (no DI registration, no change to `EnhancementService`, no persistence layer for operator preference, no prompt template changes).
- **Pivot question:** "If we built this for real, would operator language live on a tenant record, a user record, or an API key? That's the first decision and it shapes everything downstream."

### ENHANCEMENT-2: Compliance Flagging

- **Intro line:** "Pesticide and fertilizer applications hit EPA Worker Protection Standard and Health Canada PCPA record requirements. The current pipeline runs one prompt for everything; the moment a sentence enters a regulated reporting path, that single prompt is the wrong tool. This stub adds the detection contract."
- **Files to open (in order):**
  - [enhancements/02-compliance-flagging.md](enhancements/02-compliance-flagging.md) -- spec with EPA WPS / Health Canada PCPA footnotes
  - [src/Api/Models/RegulatedDomain.cs](src/Api/Models/RegulatedDomain.cs) -- the enum
  - [src/Api/Services/IComplianceFlaggingService.cs](src/Api/Services/IComplianceFlaggingService.cs) -- contract
  - [src/Api/Services/StubComplianceFlaggingService.cs:25](src/Api/Services/StubComplianceFlaggingService.cs:25) -- detection returns None, enforcement throws
- **Greppable:** `grep -rn "ENHANCEMENT-2" src/ enhancements/`
- **Stub vs real:** Contract is committed. Detection logic, domain-specific prompt templates, jurisdiction handling, and DI wiring are all deferred. Stub detection deliberately returns `None` so the stub is safe to register without changing runtime behavior.
- **Pivot question:** "Is detection an LLM classifier (high accuracy, latency cost) or a regex bootstrap (low cost, false negatives)? My instinct is regex first as a fast pre-filter, then LLM. What's your call?"

### ENHANCEMENT-3: A/B Prompt Testing Harness

- **Intro line:** "Code changes go through PR, tests, and CI. Prompt changes today go through 'I edited the file and it looked fine.' That gap is silent regression risk. This stub is the same governance discipline applied to the prompt itself."
- **Files to open (in order):**
  - [enhancements/03-prompt-ab-testing.md](enhancements/03-prompt-ab-testing.md) -- spec
  - [enhancements/03-fixtures/sample-historical-inputs.json](enhancements/03-fixtures/sample-historical-inputs.json) -- a real artifact, three hand-written inputs, no PII
  - [src/Api/Models/PromptVariant.cs](src/Api/Models/PromptVariant.cs) -- the variant record
  - [src/Api/Models/PromptComparisonResult.cs](src/Api/Models/PromptComparisonResult.cs) -- per-input control vs candidate
  - [src/Api/Services/IPromptTestHarness.cs](src/Api/Services/IPromptTestHarness.cs) -- contract
  - [src/Api/Services/StubPromptTestHarness.cs:27](src/Api/Services/StubPromptTestHarness.cs:27) -- stubbed `RunComparisonAsync`
- **Greppable:** `grep -rn "ENHANCEMENT-3" src/ enhancements/`
- **Stub vs real:** Contract is committed. The fixture file is a real artifact you can open and read. The diff algorithm choice (line-diff, token-diff, semantic-diff) is deliberately deferred -- it's a real choice and a fixture file is a better demo than a half-baked diff.
- **Pivot question:** "Would you wire this as a CLI tool the prompt author runs locally, or as a CI gate that blocks deploy? Different tradeoffs, both valid."

### ENHANCEMENT-4: PII Expansion

- **Intro line:** "The existing PII guard catches email and U.S. phone. Those are the high-volume categories. The categories that are individually rarer and individually more damaging when they leak are SSN, address, credit card, DOB. This commit adds the patterns. It deliberately does NOT wire them, because each one has a collision risk that needs its own test."
- **Files to open (in order):**
  - [enhancements/04-pii-expansion.md](enhancements/04-pii-expansion.md) -- spec
  - [src/Api/Services/PiiGuardService.cs:7](src/Api/Services/PiiGuardService.cs:7) -- header block explaining wiring is deferred and why
  - [src/Api/Services/PiiGuardService.cs:26](src/Api/Services/PiiGuardService.cs:26) -- the four new compiled regex fields
  - [enhancements/04-test-cases.md](enhancements/04-test-cases.md) -- the test cases that must land alongside wiring, including the load-bearing `Ssn_DoesNotCollideWithPhone` collision test
- **Greppable:** `grep -rn "ENHANCEMENT-4" src/ enhancements/`
- **Stub vs real:** Patterns are committed and ready to wire. `Check()` and `Redact()` bodies are unchanged. Wiring is a 3-line addition per pattern (4 lines for credit card because of Luhn). The pattern definitions ship with their collision-risk notes inline.
- **Pivot question:** "Want me to wire one of these live right now? SSN is the safest -- the hyphenated form has no collision with phone. Credit card needs a Luhn check, that's the more interesting one to walk through together."

### ENHANCEMENT-5: Webhook Out on Guardrail Hit

- **Intro line:** "Granum's value compounds when products in the portfolio can react to each other. Today, when the gate flags PII or low confidence, the rejection lives inside one service. LMN dispatch, SingleOps jobs, Greenius scheduling all stay blind. This stub is the integration surface."
- **Files to open (in order):**
  - [enhancements/05-webhook-out.md](enhancements/05-webhook-out.md) -- spec
  - [src/Api/Models/GuardrailEvent.cs](src/Api/Models/GuardrailEvent.cs) -- the event payload
  - [src/Api/Services/IWebhookEmitter.cs](src/Api/Services/IWebhookEmitter.cs) -- contract
  - [src/Api/Services/StubWebhookEmitter.cs:22](src/Api/Services/StubWebhookEmitter.cs:22) -- stubbed `EmitAsync`
  - [src/Api/Services/EnhancementService.cs:118](src/Api/Services/EnhancementService.cs:118) -- the single 3-line comment marker at the PII rejection branch identifying the wiring point. NO code change.
- **Greppable:** `grep -rn "ENHANCEMENT-5" src/ enhancements/`
- **Stub vs real:** Contract is committed. The wiring point is annotated with a comment marker, but the call into the emitter is deliberately not added -- HMAC signing, retry/backoff, per-tenant URL config, and async delivery channel are all real decisions that come before code.
- **Pivot question:** "If we wired this, would you put the emit on a background channel right away, or would you take the latency hit on the request path until usage proves it's a problem?"

## What this demo proves

- I think about products from multiple architectural axes simultaneously: language, regulation, governance, PII, and integration are five different cuts through the same pipeline.
- Stubs are deliberate. Earn-your-complexity at the meta level: don't implement until validated. Each spec carries open questions that are the conversation, not the conclusion.
- I can hand off any one of these to a junior engineer tomorrow because the spec is written, the contract is shaped, the wiring point is identified, and the test cases (where applicable) are listed.

## What NOT to oversell

- These are stubs, not implementations. `StubLanguageOrchestrator.DetectCrewLanguageAsync` throws. So does `EnforceRegulatedPhrasingAsync`, `RunComparisonAsync`, and `EmitAsync`. The stub for compliance returns `None` so it's safe to register, and the PII expansion patterns are defined but not wired into `Check`/`Redact`. None of these enhancements ship runtime behavior.
- I did NOT evaluate alternatives for any of them. The architectures sketched in the specs are one reasonable shape each, not a comparative analysis.
- The customer-pain framing is informed reasoning grounded in public data and industry patterns (Bill 101/96, EPA WPS, Health Canada PCPA, BLS occupational data, SSA SSN format, Granum's stated multi-product strategy). It is not validated customer interviews. Every spec carries footnotes that mark each claim's epistemic status (regulatory fact / public statistical pattern / industry pattern recognition / unvalidated assumption).
