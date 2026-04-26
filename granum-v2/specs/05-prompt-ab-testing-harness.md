# ENHANCEMENT-5: Prompt A/B Testing Harness (Conditional)

## The customer problem

This enhancement is conditional: it solves a problem Granum has if and only if Granum ships LLM-driven features in production. Phase 1 portfolio research crawled 34 Granum pages and found zero AI / ML claims anywhere across the portfolio.[^1] The marketing posture is consistent: "Automated Scheduling" inside LMN, "Automated Client Follow-ups" inside SingleOps, "Algorithmic routing" on SingleOps Premier, "data-driven PHC recommendations" on SingleOps Tree Inventory. Workflow automation, algorithms, data-driven. Never AI, never machine learning, never LLM. The conservative marketing posture across the portfolio may reflect deliberate marketing restraint while engineering ships LLM features, or it may reflect that LLM features are not yet in production. The marketing surface alone does not distinguish between the two.

If Granum is currently shipping or planning to ship LLM features, then prompt changes today likely go through "I edited the file and the eyeball test looked fine." That gap is the same governance gap code went through before formal test harnesses existed. A code change goes through pull request, tests, and CI before it touches production. A prompt change today goes through one engineer's judgment that it reads better.

That gap matters because prompt regressions are silent. A prompt that produces 5% worse output across 200 historical inputs does not crash the build, does not fail a unit test, and does not fire an alert. It looks like business as usual until a customer notices their notes started reading worse last Tuesday. The same governance discipline that gets applied to code (tests, diffs, evidence) should apply to the prompt, but only if there is a prompt to govern.

A prompt A/B testing harness runs a candidate prompt and the production prompt against the same fixture set, produces side-by-side outputs, and surfaces the diff for review. The change does not deploy until the diff is acceptable.

## The proposed solution

Add a prompt test harness that takes two prompt variants (control and candidate) and a fixture file of historical inputs, runs both against the LLM, and produces a side-by-side comparison artifact. The harness is tooling for the prompt author, not a runtime path on the inference endpoint. Conditional on Granum's LLM posture: if there is no production prompt to govern, this enhancement is solving a problem that does not exist yet.

## Why this earns its complexity

If the conditional holds, the failure mode this prevents is invisible: prompt regressions land without being caught, customers notice degraded output, and the postmortem ends with "we should have had a process." The harness is the prompt-layer equivalent of a migration test. If the conditional does not hold, the harness is dead code. That is why this spec is the last of the five: it is a real architectural axis but its priority depends on a fact that the public Granum marketing surface does not establish.[^1]

## Architecture sketch

- New record: `PromptVariant(string Id, string Name, string FilePath)`
- New record: `PromptComparisonResult` -- contains the input, control output, candidate output, and a diff summary
- New interface: `IPromptTestHarness` with `RunComparisonAsync(PromptVariant control, PromptVariant candidate, string fixturePath, CancellationToken ct)`
- New stub class: `StubPromptTestHarness` -- `RunComparisonAsync` throws `NotImplementedException`
- Fixture file: `granum-v2/specs/05-fixtures/sample-historical-inputs.json` (3 example inputs, no PII, hand-written for the demo)

## Stub scope (what's in this branch)

Committed:
- Record `PromptVariant`
- Record `PromptComparisonResult`
- Interface `IPromptTestHarness`
- `StubPromptTestHarness` throwing `NotImplementedException`
- Fixture file with 3 hand-written sample inputs, no PII

Not committed (deliberately):
- The actual harness implementation
- The diff algorithm (line-diff vs token-diff vs semantic-diff is a real choice)
- The CLI or test runner that invokes the harness
- Any wiring into CI

## Open questions

1. **Is Granum shipping LLM-driven features today, or planning to in the near term? This enhancement is conditional on the answer. If yes, the harness is high-leverage governance. If no or not yet, the harness is solving a problem that doesn't exist yet.** This is the first question because the rest of the enhancement is gated by it.
2. Does the harness run locally (CLI) or as a CI gate? Both are valid; they have different latency and cost profiles.
3. What is "an acceptable diff"? A human review every time, an automatic threshold, or a hybrid?
4. Where does the fixture set live and how does it grow? Hand-curated, sampled from the interactions log, or both? If Granum's bilingual posture[^2] holds, the fixture set should include parallel English and Spanish samples since prompt regressions can be language-specific.

## Greppable markers

- `granum-v2/proposed-code/Models/PromptVariant.cs` -- `// ENHANCEMENT-5` header
- `granum-v2/proposed-code/Models/PromptComparisonResult.cs` -- `// ENHANCEMENT-5` header
- `granum-v2/proposed-code/Services/IPromptTestHarness.cs` -- `// ENHANCEMENT-5` header
- `granum-v2/proposed-code/Services/StubPromptTestHarness.cs` -- `// ENHANCEMENT-5` header and method-level marker
- `granum-v2/specs/05-fixtures/sample-historical-inputs.json` -- fixture for live demo

Find them: `grep -rn "ENHANCEMENT-5" granum-v2/`

## Footnotes

[^1]: Phase 1 portfolio research crawled 34 Granum pages and found zero AI / ML claims. Marketing posture is consistent across the portfolio: "automated," "algorithmic," "data-driven," never "AI." This may reflect deliberate marketing restraint while engineering ships LLM features, or it may reflect that LLM features are not yet in production. The marketing surface alone does not distinguish between the two. **Verified absence on marketing surface (source: GRANUM_PORTFOLIO_FACTS.md honest confidence assessment).**

[^2]: LMN Crew app and Greenius are documented as bilingual English / Spanish across multiple Granum pages, which means a future prompt-testing harness should plan for parallel-language fixture sets. **Verified Granum product fact (source: granum.com/lmn/lmn-crew/ and granum.com/greenius/).**
