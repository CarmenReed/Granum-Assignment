# ENHANCEMENT-4: Test Cases for PII Expansion Wiring

When the four new regex patterns in `PiiGuardService.cs` get wired into `Check()` and `Redact()`, these test cases need to land in the same commit. They are the contract that says "wiring is correct" for each new category.

The load-bearing test in this whole list is `Ssn_DoesNotCollideWithPhone`. The SSN pattern in this branch is the conservative hyphenated form to avoid this collision; if a future commit broadens it to bare 9-digit, this test must continue to pass.

## SSN

- `Ssn_HyphenatedDetected` -- `"123-45-6789"` returns `IsFlagged=true, Reason="ssn detected"`.
- `Ssn_HyphenatedRedacted` -- `"123-45-6789"` redacts to `"[REDACTED-SSN]"`.
- `Ssn_DoesNotCollideWithPhone` -- a U.S. phone number `"555-123-4567"` does NOT match the SSN pattern; the existing phone test must continue to fire on phone, not on SSN. **Load-bearing collision test.**
- `Ssn_RejectsBareNineDigits` -- `"123456789"` does NOT match (the pattern is hyphenated-only by design).
- `Ssn_IgnoresWordBoundary` -- `"contract id 123-45-6789-A"` matches; `"x123-45-6789x"` does not.

## U.S./Canadian Address

- `Address_UsStreetDetected` -- `"123 Main Street"` returns `IsFlagged=true, Reason="address detected"`.
- `Address_UsStreetRedacted` -- `"123 Main Street"` redacts to `"[REDACTED-ADDRESS]"`.
- `Address_CommonSuffixesDetected` -- runs the pattern across St, Ave, Rd, Blvd, Ln, Dr, Ct, Way, Place.
- `Address_CanadianPostalCodeIncluded` -- `"123 Main St K1A 0B1"` matches the full string including the postal code.
- `Address_CaseInsensitive` -- `"123 main street"`, `"123 MAIN ST"` both match.
- `Address_DoesNotMatchPlainStreetReference` -- `"sprayed weeds along the street"` does NOT match (no leading number).

## Credit Card

- `CreditCard_LuhnValidVisaDetected` -- a Luhn-valid Visa-format number is flagged.
- `CreditCard_LuhnValidWithSpaces` -- `"4111 1111 1111 1111"` is flagged.
- `CreditCard_LuhnValidWithHyphens` -- `"4111-1111-1111-1111"` is flagged.
- `CreditCard_LuhnInvalidNotFlagged` -- a 16-digit number that fails Luhn does NOT flag (this is the Luhn-on-wiring requirement; without it, false positives explode).
- `CreditCard_RedactedAsCreditCard` -- redaction outputs `"[REDACTED-CC]"`.
- `CreditCard_DoesNotCollideWithSsn` -- `"123-45-6789"` is shorter than 13 digits and does not match the credit card pattern.

## Date of Birth

- `Dob_UsFormatDetected` -- `"03/04/1972"` flags.
- `Dob_DashSeparatorDetected` -- `"03-04-1972"` flags.
- `Dob_DotSeparatorDetected` -- `"03.04.1972"` flags.
- `Dob_RedactedAsDob` -- redaction outputs `"[REDACTED-DOB]"`.
- `Dob_RejectsImpossibleMonth` -- `"13/04/1972"` does NOT match (month bounds enforced).
- `Dob_RejectsImpossibleDay` -- `"03/32/1972"` does NOT match (day bounds enforced).
- `Dob_LocaleAmbiguity` -- documents that `"03/04/1972"` is ambiguous between U.S. and Canadian operators; the per-tenant locale resolution is a wiring-time decision, not a regex decision.

## Cross-Category Collision Tests

- `Existing_EmailStillDetected` -- the four new patterns must not steal matches from the existing email pattern.
- `Existing_PhoneStillDetected` -- the four new patterns must not steal matches from the existing phone pattern.
- `Multiple_PiiInOneInput` -- input containing two or more PII categories returns flagged with the first-detected category's reason; redaction handles all of them.

## Wiring Pattern (for reference, not in this branch)

Each new pattern follows the same three-line additions:

In `Check()`:

    if (Ssn.IsMatch(input)) return new PiiCheckResult(true, "ssn detected");

In `Redact()`:

    redacted = Ssn.Replace(redacted, "[REDACTED-SSN]");

Plus a Luhn check inside `Check()` for the credit card path; that is the one-pattern exception.
