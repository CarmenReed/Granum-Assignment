# ENHANCEMENT-2: Compliance Flagging (Regulated Phrasing)

## The customer problem

Pesticide and fertilizer applications carry record-keeping obligations that the rest of a landscape job's notes do not. In the U.S., the EPA Worker Protection Standard (40 CFR Part 170) sets specific application-record requirements covering product, EPA registration number, application date and time, location, applicator, and re-entry interval.[^1] In Canada, Health Canada administers the Pest Control Products Act, which governs the sale and use of pest control products including the records that licensed applicators must keep.[^2] Many U.S. states layer additional applicator record requirements on top of the federal floor.[^3]

When a crew member writes "sprayed weeds along the fence line," that phrasing is fine for an internal job log and dangerous for a regulatory audit. The cleaning pass today has no concept of which sentences enter a regulated reporting path. Every sentence gets the same general-purpose cleanup. That works until it does not, and the failure mode is "we passed an audit until we didn't."

A compliance flagger detects regulated contexts (pesticide, fertilizer, chemical storage) inside the input and routes those sentences through a stricter cleaning pass that enforces the regulated-phrasing fields the law requires.

## The proposed solution

Add a detection step that classifies the input's domain. When a regulated domain is detected (pesticide, fertilizer, chemical storage), a second cleaning pass with a domain-specific prompt enforces the required record fields. The output keeps the regulated phrasing intact rather than flattening it into casual language.

## Why this earns its complexity

The current architecture has one prompt and one expectation: "make this note read better." That collapses two very different jobs (write a clean job log; write a defensible regulatory record) into one undifferentiated cleaning pass. The failure mode is silent regulatory drift: nothing in the API surface signals that a sentence touched a regulated domain, so nothing downstream can treat it differently. Splitting detection from cleaning makes the regulated path observable, testable, and auditable.

## Architecture sketch

- New enum: `RegulatedDomain { None, PesticideApplication, FertilizerApplication, ChemicalStorage }`
- New interface: `IComplianceFlaggingService` with `DetectRegulatedContextAsync` and `EnforceRegulatedPhrasingAsync`
- New stub class: `StubComplianceFlaggingService` implementing the interface; detection returns `None`, enforcement throws `NotImplementedException`
- Wiring (NOT in this branch): detection runs before the LLM call; if non-None, a domain-specific prompt template is selected; the output is annotated with the detected domain in the response

## Stub scope (what's in this branch)

Committed:
- Enum `RegulatedDomain`
- Interface `IComplianceFlaggingService`
- `StubComplianceFlaggingService` with `DetectRegulatedContextAsync` returning `RegulatedDomain.None` and `EnforceRegulatedPhrasingAsync` throwing `NotImplementedException`

Not committed (deliberately):
- Any DI registration
- Any change to `EnhancementService`
- The domain-specific prompt templates
- The detection logic itself (an LLM-classifier or a regex-bootstrap is a research call before implementation)

## Open questions

1. Is detection an LLM classifier (high accuracy, latency cost, more spend) or a keyword/regex bootstrap (low cost, false negatives on rare wording)? Likely both, in sequence.
2. Does the response shape grow to expose the detected domain, or does the operator only see it on audit?
3. What jurisdictions does the operator's account live in? A pesticide note from a New York operator hits NY DEC requirements; a BC operator hits BC's IPMA. Domain detection is the easy part; jurisdiction handling is the harder follow-on.
4. When detection fires but enforcement fails or low-confidence, do we block the response, return a "flagged" response, or pass through with an audit log?

## Greppable markers

- `src/Api/Models/RegulatedDomain.cs` -- `// ENHANCEMENT-2` header
- `src/Api/Services/IComplianceFlaggingService.cs` -- `// ENHANCEMENT-2` header
- `src/Api/Services/StubComplianceFlaggingService.cs` -- `// ENHANCEMENT-2` header and method-level markers

Find them: `grep -rn "ENHANCEMENT-2" src/ enhancements/`

## Footnotes

[^1]: U.S. EPA Worker Protection Standard, codified at 40 CFR Part 170, sets application-record requirements for agricultural pesticide use. Specific fields and retention periods are defined in subparts B and C. **Regulatory fact.**

[^2]: Health Canada administers the federal Pest Control Products Act (PCPA) and its regulations, which govern sale and use of pest control products in Canada and include record-keeping obligations for licensed users. **Regulatory fact.**

[^3]: U.S. state-level pesticide applicator record-keeping requirements vary; many states (e.g., New York under 6 NYCRR Part 325, California under the Department of Pesticide Regulation) impose record requirements that meet or exceed the EPA federal floor. The specific state list a Granum customer must comply with depends on the operator's licensed jurisdictions. **Regulatory fact. Specific applicability per customer not validated against Granum's internal customer data.**
