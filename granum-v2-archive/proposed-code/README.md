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
patches/     -- the two existing-file modifications captured as .patch files
```

## How the patches relate to existing assignment files

Two of the five enhancements (PII expansion and webhook-out) need to touch existing `src/Api/Services/` files when implemented for real:

- [patches/04-PiiGuardService.patch](patches/04-PiiGuardService.patch) -- adds four new `private static readonly Regex` fields (SSN, U.S./Canadian address, credit card, DOB) plus a header comment block. `Check()` and `Redact()` bodies are NOT touched by the patch.
- [patches/05-EnhancementService.patch](patches/05-EnhancementService.patch) -- adds a single 3-line comment marker at the PII rejection branch identifying where an `IWebhookEmitter` would be invoked. No code change.

Both patches were generated with `git diff main -- <path>` against the live assignment files and were build-validated end-to-end (`dotnet build` clean, `dotnet test` 22/22 green) before extraction. They apply cleanly with `git apply` against the current `main`-state assignment files.

## Mapping back to the specs

Each contract in this folder is paired with a spec in [../specs/](../specs/). The spec is the load-bearing artifact; the code here is the shape the spec describes.

| Enhancement | Spec | Contract files |
| --- | --- | --- |
| 1. Two-sided language model | [../specs/01-two-sided-language.md](../specs/01-two-sided-language.md) | `Models/OperatorLanguagePreference.cs`, `Models/CrewLanguageDetection.cs`, `Services/ILanguageOrchestrator.cs`, `Services/StubLanguageOrchestrator.cs` |
| 2. Compliance flagging | [../specs/02-compliance-flagging.md](../specs/02-compliance-flagging.md) | `Models/RegulatedDomain.cs`, `Services/IComplianceFlaggingService.cs`, `Services/StubComplianceFlaggingService.cs` |
| 3. A/B prompt testing | [../specs/03-prompt-ab-testing.md](../specs/03-prompt-ab-testing.md) | `Models/PromptVariant.cs`, `Models/PromptComparisonResult.cs`, `Services/IPromptTestHarness.cs`, `Services/StubPromptTestHarness.cs` |
| 4. PII expansion | [../specs/04-pii-expansion.md](../specs/04-pii-expansion.md), [../specs/04-test-cases.md](../specs/04-test-cases.md) | `patches/04-PiiGuardService.patch` |
| 5. Webhook out | [../specs/05-webhook-out.md](../specs/05-webhook-out.md) | `Models/GuardrailEvent.cs`, `Services/IWebhookEmitter.cs`, `Services/StubWebhookEmitter.cs`, `patches/05-EnhancementService.patch` |
