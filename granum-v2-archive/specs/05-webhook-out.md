# ENHANCEMENT-5: Webhook Out on Guardrail Hit

## The customer problem

Granum's strategic surface is a multi-product portfolio (LMN, SingleOps, Greenius and adjacent products),[^1] and the value of an inference gate firing inside the cleaning service compounds when other products in the portfolio can react to it. Today, when the gate flags PII, low confidence, or a compliance failure, the rejection lives inside one service. The dispatch tool does not know. The job-tracking tool does not know. The scheduling tool does not know. Each product handles its own slice and the others get told nothing.

A webhook emitter on guardrail hit is the integration surface the rest of the Granum ecosystem would consume. When the gate fires, an outbound HTTP POST to a configured URL carries a structured event (event type, timestamp, hashed input, rejection reason, optional crew identifier). LMN dispatch can route a coaching nudge to the crew lead; SingleOps can flag the job for review; Greenius can adjust scheduling cadence. The cleaning service stops being a black box and becomes a real-time signal source.

## The proposed solution

Add an outbound webhook emitter that fires when the inference gate rejects a request. The event payload is a structured record (no raw input, no PII -- a hash and a reason). The emitter is fire-and-forget from the request path's perspective; failures to deliver do not block the user response.

## Why this earns its complexity

The current architecture's failure mode is isolation. A guardrail hit is information, and that information has zero distribution today. Adding a webhook does not expand the gate's responsibility; it lets the rest of the platform consume what the gate already knows. Without it, every cross-product integration has to reach back into the interactions log on a polling cadence, which is exactly the architecture pattern Granum's multi-product strategy should not be paying for.

## Architecture sketch

- New record: `GuardrailEvent(string EventType, DateTime Timestamp, string InputHash, string RejectionReason, string? CrewId)`
- New interface: `IWebhookEmitter` with `EmitAsync(GuardrailEvent evt, CancellationToken ct)`
- New stub class: `StubWebhookEmitter` -- `EmitAsync` throws `NotImplementedException`
- Wiring point identified in `EnhancementService.cs` at the PII rejection branch (around line 116) with a single-line `// ENHANCEMENT-5` comment marker
- Wiring (NOT in this branch): DI registration, retry/backoff policy, signing, configurable URL per tenant

## Stub scope (what's in this branch)

Committed:
- Record `GuardrailEvent`
- Interface `IWebhookEmitter`
- `StubWebhookEmitter` throwing `NotImplementedException`
- One single-line comment marker inside `EnhancementService.cs` at the PII rejection branch identifying the wiring point

Not committed (deliberately):
- The actual emit (HttpClient, signing, retry)
- DI registration in `Program.cs`
- The configuration shape (per-tenant URL, shared secret, event filter)
- Any test coverage beyond confirming the project still builds
- Any code change inside `EnhanceAsync` other than the marker comment

## Open questions

1. Is the emit synchronous (blocks the response on success/failure) or asynchronous (fire-and-forget on a background channel)? Async is the right answer; the question is which channel.
2. What does "secure" look like for the outbound webhook? HMAC signing per delivery, mutual TLS, an allowlist of destination hosts -- pick one before wiring.
3. Are guardrail events also persisted (interactions log already has `Outcome = PiiRejected`) or is the webhook the only sink? Both is fine; the question is whether the webhook reads from the log or from the request path.
4. What is the failure mode when the destination is down? Local queue, drop on floor, retry with backoff -- each is a different operational story.

## Greppable markers

- `granum-v2/proposed-code/Models/GuardrailEvent.cs` -- `// ENHANCEMENT-5` header
- `granum-v2/proposed-code/Services/IWebhookEmitter.cs` -- `// ENHANCEMENT-5` header
- `granum-v2/proposed-code/Services/StubWebhookEmitter.cs` -- `// ENHANCEMENT-5` header and method-level marker
- `granum-v2/proposed-code/patches/05-EnhancementService.patch` -- single 3-line `// ENHANCEMENT-5` wiring-point marker at the PII rejection branch, captured as a unified diff against the live `src/Api/Services/EnhancementService.cs` (no code change)

Find them: `grep -rn "ENHANCEMENT-5" granum-v2/`

## Footnotes

[^1]: Granum's multi-product portfolio (LMN dispatch, SingleOps job tracking, Greenius scheduling) is referenced in Granum's public communications and is the basis for treating cross-product webhook integration as a near-term integration target. The specific naming of LMN dispatch, SingleOps jobs, and Greenius scheduling as webhook consumers is inferred from Granum's stated multi-product strategy and not validated with Granum engineering. **Inferred integration target based on Granum's stated multi-product strategy. Not validated with Granum engineering.**
