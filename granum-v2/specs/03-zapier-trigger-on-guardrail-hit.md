<!-- v2.0 release · 2026-04-26 -->
# ENHANCEMENT-3: Zapier Trigger on Guardrail Hit

## The customer problem

LMN Professional and Enterprise tiers list Zapier as a marketed integration: "Over 6,000 apps with Zapier" with framing like "Connect LMN to the tools you already use, forms, messaging, accounting workflows, and automate repetitive steps."[^1] Zapier is the integration lane Granum already markets to the customer base. Phase 1 portfolio research crawled 34 Granum pages and found no native webhook or developer API surfaced anywhere.[^2] The supported integration vector for cross-system automation is Zapier, not a native webhook.

Today, when the inference gate flags PII, low confidence, or a compliance-cleanup miss, the rejection lives inside one service. The dispatch surface inside LMN does not know. SingleOps Crew Work Orders does not know. A customer who has already wired Zapier into their stack to push LMN events into Slack, Salesforce, an HR system, or a custom LangChain workflow has no path to consume guardrail events alongside their other LMN events. The cleanup service stays opaque to the integration lane Granum already endorses.

A Zapier trigger on guardrail hit is the integration shape that matches Granum's stated posture. Customers configure the trigger via Zapier's app catalog, point it at their existing zaps, and route flagged events into whatever downstream system they already use. The cleanup service stops being a black box and becomes a real-time signal source through the lane Granum already markets.

## The proposed solution

Expose guardrail events as a Zapier "instant trigger." When the inference gate fires (PII rejection, low-confidence flag, compliance-cleanup miss), an outbound HTTP call notifies Zapier with a structured event (event type, timestamp, hashed input, rejection reason, optional crew identifier). Customers wire the trigger into their existing zaps. No new native API surface; the integration matches the lane Granum already endorses on LMN Professional and Enterprise.[^1]

## Why this earns its complexity

The current architecture's failure mode is isolation. A guardrail hit is information, and that information has zero distribution today. A native webhook would impose a new integration surface on every customer (HMAC signing, retry/backoff, per-tenant URL config) that Granum does not currently market or document. A Zapier trigger reuses the integration surface that already ships on LMN Professional+, which means the cost of integration falls inside Zapier's existing zap-builder UI rather than on customer engineering teams. The architecture earns its complexity by extending an existing integration posture instead of inventing a new one.

## Architecture sketch

- New record: `GuardrailEvent(string EventType, DateTimeOffset Timestamp, string InputHash, string RejectionReason, string? CrewId)`
- New interface: `IZapierTriggerEmitter` with `EmitAsync(GuardrailEvent evt, CancellationToken ct)`
- New stub class: `StubZapierTriggerEmitter` -- `EmitAsync` throws `NotImplementedException`
- Wiring point identified in `EnhancementService.cs` at the PII rejection branch (around line 116) with a 3-line `// ENHANCEMENT-3` comment marker; captured as a unified diff at `granum-v2/proposed-code/patches/03-EnhancementService.patch`
- Wiring (NOT in this branch): Zapier App Directory listing, OAuth or API-key auth, Zapier's polling-vs-instant-trigger choice, retry/backoff, per-tenant tier-gating (Professional+ only, matching the existing Zapier tier policy[^1])

## Stub scope (what's in this branch)

Committed:
- Record `GuardrailEvent`
- Interface `IZapierTriggerEmitter`
- `StubZapierTriggerEmitter` throwing `NotImplementedException`
- Patch file at `granum-v2/proposed-code/patches/03-EnhancementService.patch` capturing the 3-line wiring-point comment marker against the live `src/Api/Services/EnhancementService.cs`. NO code change in `src/`.

Not committed (deliberately):
- The actual emit (HttpClient, signing, retry)
- DI registration in `Program.cs`
- The Zapier App Directory listing and trigger schema
- Any wiring into `EnhanceAsync` other than the marker comment
- Any tier-gating logic

## Open questions

1. Is the trigger an "instant trigger" (Zapier polls a REST endpoint) or a "polling trigger" (Zapier polls our endpoint on a schedule)? Instant is lower latency and Granum's existing Zapier integration may already use it; polling is simpler to ship.
2. Authentication: API-key (simpler, fits the existing LMN posture) or OAuth (richer permissions, more setup)?
3. Do we tier-gate the trigger to LMN Professional+ to match the existing Zapier tier policy,[^1] or expose it on every tier as a guardrail-specific feature?
4. Does the trigger filter by event type at the Granum side, or does Zapier's filter step handle that downstream? Filtering at Granum saves Zapier task quota; filtering at Zapier is more flexible.

## Greppable markers

- `granum-v2/proposed-code/Models/GuardrailEvent.cs` -- `// ENHANCEMENT-3` header
- `granum-v2/proposed-code/Services/IZapierTriggerEmitter.cs` -- `// ENHANCEMENT-3` header
- `granum-v2/proposed-code/Services/StubZapierTriggerEmitter.cs` -- `// ENHANCEMENT-3` header and method-level marker
- `granum-v2/proposed-code/patches/03-EnhancementService.patch` -- 3-line wiring-point marker captured as unified diff against live `src/Api/Services/EnhancementService.cs` (no code change in `src/`)

Find them: `grep -rn "ENHANCEMENT-3" granum-v2/`

## Footnotes

[^1]: LMN Professional and Enterprise tiers list Zapier as an integration with "Over 6,000 apps with Zapier" framing. The framing on the Professional product page reads "Connect LMN to the tools you already use, forms, messaging, accounting workflows, and automate repetitive steps." Zapier is not on the Starter tier. **Verified Granum product fact (source: granum.com/lmn/professional/ and granum.com/lmn/pricing/).**

[^2]: Phase 1 portfolio research crawled 34 Granum pages and found no native webhook, developer API, or REST endpoint documentation surfaced anywhere. The marketing posture treats Zapier as the integration vector for cross-system automation. The absence is on the marketing surface only; help-center pages (support.golmn.com, docs.singleops.com, support.gogreenius.com) were not crawled. **Verified absence on marketing surface (source: GRANUM_PORTFOLIO_FACTS.md fetch log).**
