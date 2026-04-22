# UI_CHANGE_TEMPLATE.md
# Granum Assignment -- UI Change Script Template
# Copy this file, fill in the CHANGE DESCRIPTION section, hand to CC.
# CC must read PRODUCTION_WORKFLOW.md before executing any change in this template.

---

## PRE-READ REQUIREMENT

Before touching any file, CC must confirm it has read PRODUCTION_WORKFLOW.md
in this session. If it has not, read it now. The gate sequence in that file
is non-negotiable and governs this entire script.

---

## CHANGE DESCRIPTION
[OPERATOR FILLS THIS IN BEFORE HANDING TO CC]

Branch name   : ui/[short-description]
Change        : [one sentence -- what are we changing visually]
Files affected: docs/index.html only (unless specified otherwise)
Approved by   : Carmen Reed
Baseline      : f8fa2ed

---

## SCOPE BOUNDARY -- READ BEFORE TOUCHING ANYTHING

This is a UI-only change. The following are LOCKED and must not be touched:

LOCKED JAVASCRIPT:
- `const API_BASE_URL` -- do not modify the value or the variable name
- Any `fetch(` call -- do not modify URL, method, headers, or body
- Any `EventSource(` or SSE handler -- do not modify
- Any function that processes API response data
- Pagination logic (page, pageSize, totalPages calculations)
- Character counter logic
- Error display logic (the text shown on 400, 422, 500)

LOCKED HTML STRUCTURE:
- Tab structure and tab switching logic
- Form controls (textarea, submit button, stream button)
- History table column order
- Pagination controls

PERMITTED CHANGES:
- CSS rules (add, modify, or remove)
- Class names on elements (if the JS does not reference them for logic)
- Static text content (labels, headings, placeholder text)
- Inline SVG or image content
- HTML structure of the header and footer only

IF IN DOUBT: do not change it. Flag it for operator review instead.

---

## STEP 1: CREATE BRANCH

```powershell
git checkout main
git pull origin main
git checkout -b ui/[short-description]
```

Confirm branch was created before proceeding.

---

## STEP 2: MAKE THE CHANGE

[OPERATOR DESCRIBES THE SPECIFIC CHANGE HERE]

After making the change, verify:
- No fetch() calls were modified (grep check):
```powershell
git diff -- docs/index.html | Select-String "fetch\(" 
```
Expected: any fetch lines shown are context only (prefixed with space), not additions (prefixed with +).

- No API_BASE_URL was modified:
```powershell
git diff -- docs/index.html | Select-String "API_BASE_URL"
```
Expected: no additions (no lines starting with +) containing API_BASE_URL.

If either check shows additions, STOP. Do not proceed. Report to operator.

---

## STEP 3: RUN LOCAL QA

```powershell
cd docs
python -m http.server 8080
```

Open http://localhost:8080 and complete every check in PRODUCTION_WORKFLOW.md.
Do not mark any check as PASS without actually verifying it in the browser.

---

## STEP 4: WRITE QA LOG

Write the QA log using the format in PRODUCTION_WORKFLOW.md.
Present it in the chat window. Do not proceed until operator responds.

---

## STEP 5: WAIT FOR OPERATOR APPROVAL

Do not create the PR. Do not push to main.
Wait for operator to say "approved" or "rejected."

---

## STEP 6: ON APPROVAL -- CREATE PR

```powershell
git add docs/index.html
git commit -m "[commit message matching branch purpose]"
git push origin ui/[short-description]
gh pr create --base main --head ui/[short-description] --title "[title]" --body "[body from PRODUCTION_WORKFLOW.md PR FORMAT]"
```

Report the PR URL to the operator.
Do not merge.

---

## STEP 7: LOG THE APPROVAL

Append one line to APPROVAL_LOG.md using the format in PRODUCTION_WORKFLOW.md.

```powershell
git add APPROVAL_LOG.md
git commit -m "docs: log approval for ui/[short-description]"
git push origin ui/[short-description]
```

Then wait for the operator to merge the PR in GitHub.

---

## STEP 8: CONFIRM AFTER MERGE

After the operator confirms the PR is merged, verify the live site:
https://CarmenReed.github.io/granum-assignment

Report back with: live URL confirmed, or flag any issue seen.
