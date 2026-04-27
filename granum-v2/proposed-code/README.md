<!-- v2.0 release · 2026-04-26 -->
# Proposed Code (Preview Only, Not Compiled)

This folder is a **static preview** of code that would land in the `Api` project if any of the five enhancements in [../specs/](../specs/) get implemented. None of the files here participate in the build. The assignment's `src/` tree is intentionally untouched.

Why static instead of compiled stubs:

- The deliverable for the take-home assignment lives under `src/`. Mixing experimental contracts into that tree muddies "this is what I shipped" vs "this is what I'd build next."
- These contracts are deliberately stub-only (every method throws `NotImplementedException` or returns the safest no-op). Shipping them in the build path adds DI surface, type clutter, and reviewer cognitive load with no runtime value.
- Keeping them as a preview lets the spec docs in [../specs/](../specs/) point at concrete file shapes without the reader having to verify whether anything is wired live.

## Layout

```
Models/      -- DTOs, enums, and records the contracts use
Services/    -- interface + stub-class pairs, one per enhancement
patches/     -- the one existing-file modification (E3) captured as a .patch file
```

## How the patch relates to existing assignment files

One of the five enhancements (Zapier trigger on guardrail hit, Enhancement-3) needs to touch an existing `src/Api/Services/` file when implemented for real:

- [patches/03-EnhancementService.patch](patches/03-EnhancementService.patch) -- adds a single 3-line comment marker at the PII rejection branch identifying where an `IZapierTriggerEmitter` would be invoked. No code change.

The patch was generated with `git diff main -- src/Api/Services/EnhancementService.cs` against the live assignment file and was build-validated end-to-end (`dotnet build` clean, `dotnet test` 22/22 green) before extraction. It applies cleanly with `git apply` against the current `main`-state assignment file.

## Mapping back to the specs

Each contract in this folder is paired with a spec in [../specs/](../specs/). The spec is the load-bearing artifact; the code here is the shape the spec describes.

| Enhancement | Spec | Contract files |
| --- | --- | --- |
| 1. English / Spanish two-sided language | [../specs/01-en-es-two-sided-language.md](../specs/01-en-es-two-sided-language.md) | `Models/OperatorLanguagePreference.cs`, `Models/CrewLanguageDetection.cs`, `Services/ILanguageOrchestrator.cs`, `Services/StubLanguageOrchestrator.cs` |
| 2. PHC compliance flagging | [../specs/02-phc-tree-inventory-compliance.md](../specs/02-phc-tree-inventory-compliance.md) | `Models/RegulatedDomain.cs`, `Services/IComplianceFlaggingService.cs`, `Services/StubComplianceFlaggingService.cs` |
| 3. Zapier trigger on guardrail hit | [../specs/03-zapier-trigger-on-guardrail-hit.md](../specs/03-zapier-trigger-on-guardrail-hit.md) | `Models/GuardrailEvent.cs`, `Services/IZapierTriggerEmitter.cs`, `Services/StubZapierTriggerEmitter.cs`, `patches/03-EnhancementService.patch` |
| 4. Greenius training trigger | [../specs/04-greenius-training-trigger.md](../specs/04-greenius-training-trigger.md) | `Models/GuardrailCategory.cs`, `Services/IGreeniusTrainingAssigner.cs`, `Services/StubGreeniusTrainingAssigner.cs` |
| 5. Prompt A/B testing harness (conditional) | [../specs/05-prompt-ab-testing-harness.md](../specs/05-prompt-ab-testing-harness.md) | `Models/PromptVariant.cs`, `Models/PromptComparisonResult.cs`, `Services/IPromptTestHarness.cs`, `Services/StubPromptTestHarness.cs` (fixture at `../specs/05-fixtures/sample-historical-inputs.json`) |
