# ENHANCEMENT-3: A/B Prompt Testing Harness

## The customer problem

The prompt is a load-bearing piece of the system, but it is the only piece that ships without code review's normal scaffolding. A code change goes through pull request, tests, and CI before it touches production. A prompt change today goes through "I edited the file and the eyeball test looked fine."

That gap matters because prompt regressions are silent. A prompt that produces 5% worse output across 200 historical inputs does not crash the build, does not fail a unit test, and does not fire an alert. It looks like business as usual until a customer notices their notes started reading worse last Tuesday.

The same governance discipline that gets applied to code (tests, diffs, evidence) should apply to the prompt. An A/B prompt testing harness runs a candidate prompt and the production prompt against the same fixture set, produces side-by-side outputs, and surfaces the diff for review. The change does not deploy until the diff is acceptable.

## The proposed solution

Add a prompt test harness that takes two prompt variants (control and candidate) and a fixture file of historical inputs, runs both against the LLM, and produces a side-by-side comparison artifact. The harness is tooling for the prompt author, not a runtime path on the inference endpoint.

## Why this earns its complexity

The current architecture treats the prompt as configuration and trusts the engineer's judgment that a change is safe. That is the same posture the codebase had toward database migrations before formal migration tooling existed. The failure mode is the same: invisible drift, surface area for regressions that no test covers, and a postmortem that ends with "we should have had a process." The harness is the prompt-layer equivalent of a migration test.

## Architecture sketch

- New record: `PromptVariant(string Id, string Name, string FilePath)`
- New record: `PromptComparisonResult` -- contains the input, control output, candidate output, and a diff summary
- New interface: `IPromptTestHarness` with `RunComparisonAsync(PromptVariant control, PromptVariant candidate, string fixturePath, CancellationToken ct)`
- New stub class: `StubPromptTestHarness` -- `RunComparisonAsync` throws `NotImplementedException`
- Fixture file: `granum-v2/specs/03-fixtures/sample-historical-inputs.json` (3 example inputs, no PII, hand-written for the demo)

## Stub scope (what's in this branch)

Committed:
- Record `PromptVariant`
- Record `PromptComparisonResult`
- Interface `IPromptTestHarness`
- `StubPromptTestHarness` throwing `NotImplementedException`
- Fixture file with 3 sample inputs

Not committed (deliberately):
- The actual harness implementation
- The diff algorithm (line-diff vs token-diff vs semantic-diff is a real choice)
- The CLI or test runner that invokes the harness
- Any wiring into CI

## Open questions

1. Does the harness run locally (CLI) or as a CI gate? Both are valid; they have different latency and cost profiles.
2. What is "an acceptable diff"? A human review every time, an automatic threshold, or a hybrid?
3. Where does the fixture set live and how does it grow? Hand-curated, sampled from the interactions log, or both?
4. Does the harness support more than two variants (A/B/C/D) or strictly pairwise? Pairwise is simpler; multi-way is more useful when iterating.

## Greppable markers

- `granum-v2/proposed-code/Models/PromptVariant.cs` -- `// ENHANCEMENT-3` header
- `granum-v2/proposed-code/Models/PromptComparisonResult.cs` -- `// ENHANCEMENT-3` header
- `granum-v2/proposed-code/Services/IPromptTestHarness.cs` -- `// ENHANCEMENT-3` header
- `granum-v2/proposed-code/Services/StubPromptTestHarness.cs` -- `// ENHANCEMENT-3` header and method-level marker
- `granum-v2/specs/03-fixtures/sample-historical-inputs.json` -- fixture for live demo

Find them: `grep -rn "ENHANCEMENT-3" granum-v2/`

## Footnotes

No regulatory or demographic claims are made in this spec; the framing is governance discipline applied at the model layer rather than only at the code layer.
