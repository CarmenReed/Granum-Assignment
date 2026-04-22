# PRODUCTION_WORKFLOW.md
# Granum Assignment -- Production Edit Protocol
# Effective: April 22, 2026 00:00 UTC (site declared LIVE)
# All commits to main after this timestamp are production edits.
# This file governs every change from this point forward.

---

## BASELINE DECLARATION

```
LIVE timestamp  : April 22, 2026 00:00 UTC
Repo            : https://github.com/CarmenReed/granum-assignment
Live URL        : https://CarmenReed.github.io/granum-assignment
Railway URL     : (confirmed working at LIVE declaration)
Baseline commit : f8fa2ed (feat: Granum branding and UI polish)
```

This baseline is the submission state. Every change after this point
is a production edit and must go through the full gate sequence below.
No direct pushes to main. No exceptions.

---

## GATE SEQUENCE FOR EVERY PRODUCTION EDIT

```
1. BRANCH       CC creates a feature branch, never works on main
2. CHANGE       CC makes the edit on the feature branch only
3. LOCAL QA     CC serves the frontend locally, runs smoke checks
4. QA LOG       CC writes a QA log entry -- operator reads it
5. HUMAN GATE   Operator explicitly approves with "approved" or rejects
6. PR           CC creates a GitHub PR on approval only
7. MERGE        Operator merges the PR manually in GitHub UI
8. VERIFY       Operator confirms live URL after merge
```

CC must not skip or reorder any step.
CC must not push to main directly under any circumstance.
CC must not create the PR until the operator says "approved."
CC must not merge the PR -- that action belongs to the operator only.

---

## BRANCH NAMING

```
ui/[short-description]      for visual changes
fix/[short-description]     for bug fixes
docs/[short-description]    for README or documentation changes
```

Example: `ui/outcome-badge-spacing`

---

## LOCAL QA PROTOCOL (STEP 3)

Before writing the QA log, CC must serve the frontend locally and verify:

```powershell
# Serve docs/ locally on port 8080
cd docs
python -m http.server 8080
```

Then open http://localhost:8080 and verify ALL of the following.
Every item must be checked. No item may be skipped.

### Functional smoke checks (must pass -- any failure blocks the PR)

- [ ] F1: Page loads without console errors
- [ ] F2: Enhance tab visible and active on load
- [ ] F3: Textarea accepts input and character counter updates
- [ ] F4: Submit button is present and clickable
- [ ] F5: Stream button is present and clickable
- [ ] F6: History tab is present and clickable
- [ ] F7: History tab loads without console errors
- [ ] F8: API_BASE_URL const is unchanged in the source
- [ ] F9: No fetch() calls were modified
- [ ] F10: No SSE event handler code was modified

### Visual checks (must pass -- any failure blocks the PR)

- [ ] V1: Granum logo renders in header
- [ ] V2: Tagline renders below logo
- [ ] V3: Active tab has #E8401C underline indicator
- [ ] V4: Light theme -- white background, no dark mode bleed
- [ ] V5: Layout is not broken at 375px width (mobile)
- [ ] V6: No em dashes visible anywhere on the page

### Em dash scan (must return zero results)

```powershell
Select-String -Path docs/index.html -Pattern ([char]0x2014) -SimpleMatch
```

Expected: no output. Any result blocks the PR.

---

## QA LOG FORMAT (STEP 4)

CC writes this log to `qa_log.md` at the repo root (never committed to main,
lives on the feature branch only until the PR is merged, then archived).

```
## QA Entry
Date        : [readable UTC, e.g. April 22, 2026 15:30 UTC]
Branch      : [branch name]
Change      : [one sentence describing what changed]
Commit      : [hash]

### Functional Checks
F1  [PASS/FAIL]
F2  [PASS/FAIL]
F3  [PASS/FAIL]
F4  [PASS/FAIL]
F5  [PASS/FAIL]
F6  [PASS/FAIL]
F7  [PASS/FAIL]
F8  [PASS/FAIL]
F9  [PASS/FAIL]
F10 [PASS/FAIL]

### Visual Checks
V1  [PASS/FAIL]
V2  [PASS/FAIL]
V3  [PASS/FAIL]
V4  [PASS/FAIL]
V5  [PASS/FAIL]
V6  [PASS/FAIL]

### Em Dash Scan
Result: [CLEAN / FOUND -- list lines]

### Overall
[READY FOR PR / BLOCKED -- reason]

### Notes
[Any observations, deviations, or things operator should look at]
```

CC presents this log in the chat window for the operator to read.
CC does not create the PR until the operator responds.

---

## HUMAN GATE (STEP 5)

After CC presents the QA log, the operator must respond with one of:

- "approved" -- CC proceeds to create the PR
- "approved: [note]" -- CC proceeds, logs the note in the PR description
- "rejected: [reason]" -- CC stops, does not create the PR, awaits instructions

CC must wait. CC must not interpret silence as approval.
CC must not proceed after a partial or ambiguous response.

---

## PR FORMAT (STEP 6)

CC creates the PR with this structure:

Title: same as commit message
Body:
```
## Change
[One sentence describing what changed]

## QA
All 16 smoke checks passed. Em dash scan clean.
QA timestamp: [readable UTC, e.g. April 22, 2026 15:30 UTC]

## Operator Approval
Approved by operator at [timestamp of approval message]

## Files Changed
[list]

## Baseline
This PR targets main at f8fa2ed (LIVE baseline, April 22, 2026).
```

---

## APPROVAL LOG

CC appends one line to `APPROVAL_LOG.md` at repo root each time
the operator approves a PR. This file IS committed to main.

Format:
```
[readable UTC] | [branch] | [PR number] | [operator approval note or "approved"]
```

Example:
```
April 22, 2026 10:14 UTC | ui/outcome-badge-spacing | PR #20 | approved
```

This is the audit trail. It must never be edited retroactively.

---

## WHAT CC MUST NEVER DO UNDER THIS PROTOCOL

- Push directly to main
- Merge a PR
- Skip any QA check
- Mark a check PASS without verifying it
- Create a PR without operator approval
- Proceed after operator says "rejected"
- Edit APPROVAL_LOG.md retroactively
- Alter any fetch(), API_BASE_URL, or SSE handler during a UI change
