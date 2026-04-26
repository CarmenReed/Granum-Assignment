# ENHANCEMENT-4: PII Expansion

## The customer problem

The current PII guard catches email and U.S. phone numbers. That covers the two highest-volume PII categories in landscape services free-text, but it does not cover the categories that are individually rarer and individually more damaging when they leak.[^1] Crew notes occasionally reference a customer's date of birth (gate code memory aids, "Mrs. Smith's 80th birthday"), addresses (when the operator's job number is missing or misread), and -- in the bookkeeping subset of users -- the occasional credit card or SSN where a crew member dictated something a back-office worker should have handled.

The point is not that these categories appear often. The point is that when they appear, the existing guard waves them through, and the LLM happily echoes them into the cleaned output and the interactions log. Every additional category we cover narrows that exposure.

## The proposed solution

Add four new compiled regex patterns to the existing `PiiGuardService`: SSN, U.S./Canadian addresses, credit card numbers (Luhn-validated), and date-of-birth. The patterns sit alongside the existing email and phone patterns and follow the same shape (compiled, IgnoreCase where relevant, single source of truth).

## Why this earns its complexity

This one earns its place by *not* changing the architecture at all. The pluggable-regex pattern is already there; the failure mode it prevents (PII leakage) is already understood; the contract for how `Check` and `Redact` work is already in place. Adding categories is the cheapest possible high-value follow-on. The reason it is staged as a stub-and-then-wire is that each pattern has its own collision risk: an SSN pattern that happens to match U.S. phone fragments would silently degrade the existing phone detection. Wiring is a separate, careful step.

## Architecture sketch

- Modify: `src/Api/Services/PiiGuardService.cs`
- Add four `private static readonly Regex` fields: SSN, AddressUsCa, CreditCard, Dob
- Each field has an XML doc comment describing the pattern and an `// ENHANCEMENT-4` marker
- Existing `Check` and `Redact` methods stay unchanged in this branch
- Wiring (NOT in this branch): three-line additions per pattern in `Check` (if-match return) and `Redact` (Replace chain), plus a test per pattern

## Stub scope (what's in this branch)

Committed:
- Four new compiled regex fields with XML doc comments
- A header comment block explaining the wiring is deferred and why
- An `enhancements/04-test-cases.md` file documenting the test cases that would be added when the patterns wire in

Not committed (deliberately):
- Any changes to `Check()` or `Redact()` method bodies
- Any new test methods
- Any change to `EnhancementService` (the PII rejection branch unchanged)

## Open questions

1. SSN format `XXX-XX-XXXX` is the SSA standard,[^2] but bare 9-digit input also appears in field notes. Do we cover bare 9-digit too, knowing it overlaps with phone fragments?
2. Credit card detection: Luhn check filters most random-digit false positives. Do we still flag a Luhn-passing 16-digit string as PII even when no separator is present?
3. DOB: `MM/DD/YYYY` is U.S.-default; `DD/MM/YYYY` is Canada/UK-default. The patterns must coexist without one stealing matches from the other.
4. Address detection has the worst signal-to-noise ratio. A street address pattern is easy to write and easy to over-match (street-name false positives in normal sentences). What is the acceptable false-positive rate?

## Greppable markers

- `src/Api/Services/PiiGuardService.cs` -- `// ENHANCEMENT-4` header block and four field-level markers
- `enhancements/04-test-cases.md` -- documented test cases for wiring

Find them: `grep -rn "ENHANCEMENT-4" src/ enhancements/`

## Footnotes

[^1]: PII category prevalence in landscape-services free-text records (high-volume email/phone, low-volume SSN/credit-card/DOB) is industry pattern recognition, not validated against Granum's internal interaction logs. **Industry pattern recognition. Not validated against Granum's internal customer data.**

[^2]: The Social Security Administration's Social Security Number format is three-digit area, two-digit group, four-digit serial, conventionally rendered with hyphens (`XXX-XX-XXXX`). **Regulatory fact (SSA standard format).**
