<!-- v2.0 release · 2026-04-26 -->
# ENHANCEMENT-2: PHC Application Record Mode for SingleOps Tree Inventory

## The customer problem

SingleOps Tree Inventory is marketed as a structured record of trees on a property, including "PHC treatments" and "PHC recommendations" alongside species, health, risk history, and "data-driven PHC recommendations."[^1] Plant Health Care (PHC) work for tree care companies includes pesticide application: insecticides, fungicides, growth regulators, soil drenches. In the U.S., pesticide applications carry record-keeping obligations under the EPA Worker Protection Standard (40 CFR Part 170), which sets specific application-record requirements covering product, EPA registration number, application date and time, location, applicator, and re-entry interval.[^2] In Canada, Health Canada administers the Pest Control Products Act, which governs sale and use of pest control products including the records that licensed applicators must keep.[^3]

SingleOps Tree Inventory captures the field shape of these treatments (what species, what treatment, when, by whom) but the marketing surface does not describe the captured records as compliance-defensible.[^1] When a tree-care crew member writes "applied imidacloprid to the oak by the driveway," the cleanup pass today routes that note through the same general-purpose template every other note uses. That collapses two very different jobs (write a clean job log; produce a defensible regulatory record) into one undifferentiated cleanup. The failure mode is silent regulatory drift: nothing in the API surface signals that a sentence touched a regulated domain, so nothing downstream can treat it differently.

A compliance flagger detects regulated contexts (pesticide application, fertilizer application, chemical storage) inside the input and routes those sentences through a stricter cleanup pass that enforces the regulator's required record fields. The wiring point is the existing SingleOps Tree Inventory PHC entry path: when an inventory entry includes a PHC treatment, run the regulated-template cleanup instead of the general one.

## The proposed solution

Add a detection step that classifies the input's domain. When a regulated domain is detected, a second cleanup pass with a domain-specific prompt enforces the required record fields (applicator, product, EPA registration, date, location, re-entry interval). The output keeps the regulated phrasing intact rather than flattening it into casual language. Tree Inventory's existing PHC-treatment surface is the most concrete wiring target on the Granum portfolio.[^1]

## Why this earns its complexity

SingleOps already captures PHC work as structured data. The cleanup pass already lives one layer above that capture. The current architecture has one prompt and one expectation: "make this note read better." For PHC notes, that is the wrong contract. The compliance flagger lets the existing surface graduate from "good job log" to "defensible regulatory record" without rewriting the data model. Without it, SingleOps' PHC capture remains a marketing feature but not a compliance feature, and the Phase 1 portfolio research found no compliance / regulatory feature surface anywhere across 34 Granum pages crawled.[^4]

## Architecture sketch

- New enum: `RegulatedDomain { None, PesticideApplication, FertilizerApplication, ChemicalStorage }`
- New interface: `IComplianceFlaggingService` with `DetectRegulatedContextAsync` and `EnforceRegulatedPhrasingAsync`
- New stub class: `StubComplianceFlaggingService` implementing the interface; detection returns `None`, enforcement throws `NotImplementedException`
- Wiring (NOT in this branch): detection runs before the LLM call; if non-None, a domain-specific prompt template is selected; the output is annotated with the detected domain in the response so downstream consumers (Tree Inventory, audit reports) can route it

## Stub scope (what's in this branch)

Committed:
- Enum `RegulatedDomain`
- Interface `IComplianceFlaggingService`
- `StubComplianceFlaggingService` with `DetectRegulatedContextAsync` returning `RegulatedDomain.None` and `EnforceRegulatedPhrasingAsync` throwing `NotImplementedException`

Not committed (deliberately):
- Any DI registration
- Any change to `EnhancementService`
- The domain-specific prompt templates (one per regulated domain)
- The detection logic itself (an LLM-classifier or a regex-bootstrap is a research call before implementation)
- Any wiring into SingleOps Tree Inventory's PHC entry path

## Open questions

1. Is detection an LLM classifier (high accuracy, latency cost, more spend) or a keyword/regex bootstrap (low cost, false negatives on rare wording)? Likely both, in sequence.
2. Does the response shape grow to expose the detected domain back to SingleOps Tree Inventory, or does the operator only see it on audit?
3. SingleOps customers operate in multiple U.S. states and Canadian provinces. State-level pesticide applicator record-keeping requirements vary, and a New York operator hits NY DEC requirements while a B.C. operator hits B.C.'s IPMA. Domain detection is the easy part; jurisdiction handling is the harder follow-on, and SingleOps' marketing surface does not signal a jurisdiction layer today.
4. When detection fires but enforcement fails or low-confidence, do we block the response, return a "flagged" response, or pass through with an audit log?

## Greppable markers

- `granum-v2/proposed-code/Models/RegulatedDomain.cs` -- `// ENHANCEMENT-2` header
- `granum-v2/proposed-code/Services/IComplianceFlaggingService.cs` -- `// ENHANCEMENT-2` header
- `granum-v2/proposed-code/Services/StubComplianceFlaggingService.cs` -- `// ENHANCEMENT-2` header and method-level markers

Find them: `grep -rn "ENHANCEMENT-2" granum-v2/`

## Footnotes

[^1]: SingleOps Tree Inventory captures PHC treatments and PHC recommendations as a marketed product feature, including "detailed profiles, species, health, risk history" and "data-driven PHC recommendations." **Verified Granum product fact (source: granum.com/singleops/tree-inventory/).**

[^2]: U.S. EPA Worker Protection Standard, codified at 40 CFR Part 170, sets application-record requirements for agricultural pesticide use. Specific fields and retention periods are defined in subparts B and C. **Regulatory fact.**

[^3]: Health Canada administers the federal Pest Control Products Act (PCPA) and its regulations, which govern sale and use of pest control products in Canada and include record-keeping obligations for licensed users. **Regulatory fact.**

[^4]: Phase 1 portfolio research crawled 34 Granum pages and found zero compliance / regulatory feature surface. No mention of EPA, pesticide records, applicator licensing, OSHA, ISA certification, SOC 2, HIPAA, GDPR, audit trails, or PCPA. The closest is "Pesticides" as a Greenius Tailgate Talk topic (training content, not record-keeping). **Verified absence on marketing surface (source: GRANUM_PORTFOLIO_FACTS.md cross-product themes).**
