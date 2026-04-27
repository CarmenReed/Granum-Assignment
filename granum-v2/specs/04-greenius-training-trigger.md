<!-- v2.0 release · 2026-04-26 -->
# ENHANCEMENT-4: Greenius Training Trigger on Guardrail Hit

## The customer problem

Greenius is Granum's employee training and development platform with 150+ pre-built courses, including "Tailgate Talks" structured at 15-minute granularity covering safety, equipment, soft skills, and business practices.[^1] Greenius is already integrated into the LMN Crew app: training is "surfaced inside the LMN Crew app" so crew members can take a course directly inside the workflow they already use.[^2] Greenius offers per-category content (equipment training, health and safety, specialty training) and pitches itself for "multiple crews, busy seasons, and bilingual workforces" with parallel English / Spanish course sets.[^3]

Today, when the inference gate flags a category (PII leaked into a note, compliance phrasing missed, low-confidence cleanup), the rejection is logged and the request fails closed. The crew member who triggered the flag continues working with no signal that they could close the gap with a 15-minute course that already exists. The training platform sits one click away inside the same app, but the gate and the training surface do not talk to each other.

A Greenius training trigger on guardrail hit closes that loop. When the gate flags a category, an auto-assignment of the matching Greenius course lands in the crew member's training queue. The crew lead sees the assignment on the next sync; the crew member completes the course at the next break. The integration surface for this already exists (Greenius inside the LMN Crew app[^2]); only the auto-assignment trigger is new.

## The proposed solution

Add a guardrail-category-to-course mapping and an auto-assignment service. When the inference gate fires with a categorized reason (PII leak, compliance miss, low confidence), the assigner looks up the matching Greenius course and pushes a course assignment to the crew member identified in the request. The matching itself is a configurable mapping (PII leak -> "Customer Data Handling" Tailgate Talk, compliance miss -> "Pesticide Recordkeeping" Tailgate Talk, etc.) so course IDs change without code changes.

## Why this earns its complexity

The current architecture has the gate signal but no way to consume it as a learning event. The cost of the gate firing is borne entirely by the request that failed; the next request from the same crew member has no benefit from the prior signal. Greenius is the system Granum already markets as the way crews get smarter, and it is already integrated into the surface (LMN Crew app) where the gate fires. Auto-assignment turns a one-time rejection into a learning loop without requiring a new training surface, a new mobile app, or a new content library. The complexity is justified because the integration surface already exists.[^2]

## Architecture sketch

- New enum: `GuardrailCategory { PiiLeak, ComplianceMissed, LowConfidence, Other }`
- New interface: `IGreeniusTrainingAssigner` with `AssignAsync(GuardrailCategory category, string crewId, CancellationToken ct)`
- New stub class: `StubGreeniusTrainingAssigner` -- `AssignAsync` throws `NotImplementedException`
- Wiring (NOT in this branch): the category-to-course mapping (a config file or a tenant-level setting), the assignment API call into Greenius itself, retry/backoff, per-tenant licensing checks (the customer must be on Greenius), and the crew-language detection from Enhancement-1 to prefer Spanish-language courses for Spanish-speaking crew members[^3]

## Stub scope (what's in this branch)

Committed:
- Enum `GuardrailCategory`
- Interface `IGreeniusTrainingAssigner`
- `StubGreeniusTrainingAssigner` throwing `NotImplementedException`

Not committed (deliberately):
- Any DI registration
- The category-to-course mapping
- The Greenius assignment API call (the public marketing surface does not document a course-assignment API; whether one exists is an open question)[^4]
- Any change to `EnhancementService`

## Open questions

1. Does Greenius expose a course-assignment API? The public marketing surface does not document one.[^4] If the answer is no, this enhancement requires either a new Greenius surface or a workflow that puts a "suggested course" notification in the LMN Crew app inbox instead of a hard assignment.
2. Where does the category-to-course mapping live? Per-tenant configuration is the most flexible; a global default with per-tenant override is the most pragmatic.
3. Does the assignment respect crew language? Greenius offers parallel English / Spanish course sets;[^3] pairing this with Enhancement-1's `CrewLanguageDetection` would route a Spanish-speaking crew member to the Spanish version of the assigned course.
4. What is the customer-licensing story? Greenius is a separate product. A customer on LMN without Greenius gets no benefit from this enhancement, which raises the question of whether the trigger fires at all for those tenants or whether it surfaces a Greenius upsell.

## Greppable markers

- `granum-v2/proposed-code/Models/GuardrailCategory.cs` -- `// ENHANCEMENT-4` header
- `granum-v2/proposed-code/Services/IGreeniusTrainingAssigner.cs` -- `// ENHANCEMENT-4` header
- `granum-v2/proposed-code/Services/StubGreeniusTrainingAssigner.cs` -- `// ENHANCEMENT-4` header and method-level marker

Find them: `grep -rn "ENHANCEMENT-4" granum-v2/`

## Footnotes

[^1]: Greenius offers 150+ pre-built online courses including Tailgate Talks at 15-minute granularity, with categories covering safety, equipment, soft skills, and business practices. **Verified Granum product fact (source: granum.com/greenius/ and granum.com/greenius/courses/).**

[^2]: Greenius training is surfaced inside the LMN Crew app: "Greenius training accessible directly from the Crew app." This is the only documented inter-product integration surface inside the Granum portfolio at this time. **Verified Granum product fact (source: granum.com/lmn/lmn-crew/).**

[^3]: Greenius states "Every course and interface is translated so your crews can train in their preferred language" with courses available in English and Spanish, designed for "bilingual workforces." **Verified Granum product fact (source: granum.com/greenius/).**

[^4]: The public marketing surface does not document a course-assignment API or webhook for Greenius. Whether one exists inside the help-center / developer portal is unknown; this spec marks the assignment surface as an open question rather than asserting it exists. **Unvalidated assumption. Spec marks the assignment surface as an open question.**
