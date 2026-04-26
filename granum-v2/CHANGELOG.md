# Changelog

## v2.0 (2026-04-26)

Initial release of the v2 enhancement showcase folder.

### Added

- Five portfolio-anchored enhancement specs ([specs/](specs/))
  - English / Spanish two-sided language model
  - PHC application record mode for SingleOps Tree Inventory
  - Zapier trigger on guardrail hit
  - Greenius training trigger on guardrail hit
  - Prompt A/B testing harness (conditional)
- Static contract previews for every enhancement ([proposed-code/](proposed-code/))
- One applied patch capturing the Zapier-trigger wiring point against `src/Api/Services/EnhancementService.cs` ([proposed-code/patches/03-EnhancementService.patch](proposed-code/patches/03-EnhancementService.patch))
- Portfolio research artifact, the source of truth behind every customer-pain claim ([GRANUM_PORTFOLIO_FACTS.md](GRANUM_PORTFOLIO_FACTS.md))
- Per-enhancement walk-through map ([ENHANCEMENT_DEMO_MAP.md](ENHANCEMENT_DEMO_MAP.md))

### Notes

- The take-home assignment under `../src/` is intentionally untouched. Net file change there: zero.
- Every customer-pain claim carries a footnote with an explicit epistemic-status tag (regulatory fact, verified Granum product fact, public statistical pattern, industry pattern recognition, or unvalidated assumption).
- Stubs throw `NotImplementedException` (except the compliance detection stub, which returns `None` deliberately). None of the proposed code is wired to runtime.
