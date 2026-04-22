---
name: timeline-sync
description: >
  Merges Claude.AI timeline-extract JSON with git log to update the
  "How It Was Built" timeline section in docs/index.html. Creates a
  branch, runs QA per PRODUCTION_WORKFLOW.md, and opens a PR on
  operator approval. Repeat-safe: diffs before committing, no PR if
  nothing changed. Scope: granum-assignment repo only. (SG-flagged)
---

## PURPOSE

`timeline-sync` is a project-scoped skill for the granum-assignment repo. It merges two inputs -- a JSON block produced by the Claude.AI `/timeline-extract` command and the live git log of this repo -- into a milestone timeline that is rendered as the "How It Was Built" tab section inside `docs/index.html`. The skill validates the input schema, fills git gaps, groups and deduplicates milestones, generates HTML using only the existing design tokens in `docs/index.html`, diffs against current content, and follows `PRODUCTION_WORKFLOW.md` gate sequence to branch, QA, and open a PR on operator approval. The skill never pushes to main, never merges PRs, and never creates a PR if the content diff is empty.

## INPUT CONTRACT

The operator pastes a JSON block matching this schema exactly. Validate on ingestion. If any field is missing or malformed, stop and report the exact field that failed.

```json
{
  "schema_version": "1.1",
  "extracted_at": "ISO8601",
  "extracted_by": "claude-ai-timeline-extract",
  "project": "granum-assignment",
  "operator": "CarmenReed",
  "repo": "https://github.com/CarmenReed/granum-assignment",
  "live_url": "https://CarmenReed.github.io/granum-assignment",
  "live_declared_at": "ISO8601",
  "live_commit": "string",
  "sources_read": {
    "spec_file": "boolean",
    "conversation_context": "boolean",
    "git_log": "boolean"
  },
  "thought_process_summary": ["string"],
  "milestones": [
    {
      "id": "string",
      "phase": "PHASE_0 through PHASE_6",
      "phase_label": "string",
      "title": "string",
      "date_display": "string",
      "date_iso": "ISO8601 or null",
      "body": "string",
      "evidence": "string",
      "confidence": "GREEN | YELLOW | RED",
      "sources": ["A | B | git"],
      "source_notes": {
        "spec": "string or null",
        "conversation": "string or null",
        "git": "string or null"
      },
      "tags": ["string"],
      "git_commit": "hash or null",
      "needs_cc": "boolean"
    }
  ],
  "gaps": [
    {
      "description": "string",
      "suggested_cc_command": "string or null"
    }
  ],
  "cc_instructions": "string"
}
```

`sources_read` keys and `sources` codes:
- `spec_file` / code `"A"` = the Granum assignment spec
- `conversation_context` / code `"B"` = the prior Claude.AI conversation
- `git_log` / code `"git"` = the repo git log

`source_notes` is pure display narrative. Each value is rendered verbatim inside the milestone. Never gate workflow logic on its values.

Validation rules:
- `schema_version` must equal `"1.1"`.
- `extracted_by` must equal `"claude-ai-timeline-extract"`.
- `sources_read` must be an object with boolean keys `spec_file`, `conversation_context`, `git_log`.
- `thought_process_summary` must be an array of strings (array may be empty).
- Every milestone must include `id`, `phase`, `phase_label`, `title`, `date_display`, `date_iso`, `body`, `evidence`, `confidence`, `sources`, `source_notes`, `tags`, `git_commit`, `needs_cc`.
- `confidence` must be `GREEN`, `YELLOW`, or `RED`.
- Each milestone's `sources` must be a non-empty array of known codes drawn from `{"A", "B", "git"}`.
- Each milestone's `source_notes` must be an object with keys `spec`, `conversation`, `git`; each value is a string or null. Rendered verbatim; not used for gating.
- On any failure: report the failing field path and stop. Do not proceed to Phase B.

## INVOCATION SEQUENCE

### Phase A -- Ingest and validate
1. Operator pastes the JSON into chat.
2. Parse and validate against the INPUT CONTRACT above.
3. On failure: report the exact field path and halt.
4. On success: confirm ingestion with the milestone count and a one-line breakdown of confidence levels (N GREEN / N YELLOW / N RED).

### Phase B -- Fill git gaps
From the repo root, run:
```powershell
git log --format="%H|%ai|%s" -- | Out-File -Encoding utf8 git_log_raw.txt
```
Then:
- For every milestone where `needs_cc` is true or `git_commit` is not null, match the commit hash or locate the closest commit by keyword match on the commit subject.
- If a matching commit is found, fill `date_iso` when null and upgrade `confidence` from YELLOW to GREEN.
- A git commit is expected if any of: `git_commit` is non-null, `needs_cc` is `false`, OR `sources` contains `"git"`. For such milestones, if no matching commit is located, flag as RED and record the gap.
- `source_notes` is pure display narrative; do not gate Phase B on its values.
- Never invent a commit hash. If a commit cannot be located, leave it RED.

### Phase C -- Group and deduplicate
- Group milestones by phase. Within a phase, sort ascending by `date_iso`.
- If two milestones share the same date and phase, merge them unless their titles are meaningfully different.
- Never merge a GREEN milestone with a RED milestone.
- Cap the rendered output at 12 milestones. If more exist after dedup, keep the highest-confidence, most-distinct-titled set.

### Phase D -- Generate HTML section
- Read `docs/index.html` before generating output.
- Use only the CSS variable names already defined in `docs/index.html`. Do not introduce new variable names.
- Generate HTML only for the "How It Was Built" tab section.
- Emit a collapsible preamble before the milestone list containing `thought_process_summary`. Use a native `<details>` with a `<summary>` labelled `How this timeline was assembled`. Render each string in the `thought_process_summary` array as its own `<p>` inside the `<details>` block, using `ink-soft` color at 13px. Reuse existing design tokens only; do not add new CSS. If the array is empty, still emit the `<details>` element with an empty body -- do not suppress the section.
- Per-milestone structure:
  - phase label: mono, coral
  - title: serif, navy, 16px bold
  - `date_display`: mono, muted, 11px
  - `body`: 13px, ink-soft
  - `source_notes`: render verbatim as 12px muted text. Emit one `<p>` per populated key in order `spec`, `conversation`, `git`, prefixed with the key name in bold (e.g. `<strong>conversation:</strong> ...`). Skip keys whose value is null.
  - evidence chip: green for GREEN, gold for YELLOW, muted for RED
  - tags: small pills, coral-lt background
- RED confidence milestones render with a dashed border and the label: `EVIDENCE PENDING -- not yet verified by git log`.
- Use `--` (double hyphen) everywhere. Never emit an em dash (U+2014).

### Phase E -- Diff against current content
- Read the current "How It Was Built" section from `docs/index.html`.
- Compare against the newly generated section.
- If only whitespace or formatting differs: report `No milestone changes detected. No PR needed.` and stop.
- If meaningful content changed: list the added, updated, and removed milestones and wait for the operator to say `proceed` before moving to Phase F.

### Phase F -- Branch, edit, QA, PR
Follow `PRODUCTION_WORKFLOW.md` gate sequence exactly.
- Branch name: `docs/timeline-update-[YYYY-MM-DD]`.
- Edit only the "How It Was Built" tab section in `docs/index.html`. Do not touch any other section, any JavaScript, or the Enhance or History tab content.
- Run the full QA checklist from `PRODUCTION_WORKFLOW.md`. Write the QA log. Present it to the operator. Wait for `approved` before creating the PR.
- PR title: `docs: timeline update [YYYY-MM-DD]`.
- PR body must include: milestone count, confidence breakdown (N GREEN / N YELLOW / N RED), git commits cited as evidence, and the operator approval timestamp.

### Phase G -- Append to APPROVAL_LOG.md
Append an entry in the same format as `PRODUCTION_WORKFLOW.md` specifies.

## RULES

- Never push to main directly.
- Never merge a PR.
- Never invent git evidence for a RED confidence milestone.
- Never skip the PRODUCTION_WORKFLOW.md QA gate.
- Never output em dashes (U+2014); use `--` instead.
- Never create a PR if the content diff is empty.
- Never modify sections of `docs/index.html` other than the "How It Was Built" tab.
- Never introduce new CSS variable names; reuse the existing design tokens in `docs/index.html`.
- Always validate the input JSON against the INPUT CONTRACT before any other action.
- Always wait for explicit operator `proceed` after the Phase E diff and explicit `approved` after the Phase F QA log before advancing.

## REPEAT RUN BEHAVIOR

- If a branch named `docs/timeline-update-*` already exists:
  - If it was merged, proceed normally with a new dated branch.
  - If it is unmerged, warn the operator and ask whether to overwrite or abort before continuing.
- If the incoming JSON has the same `extracted_at` value as a prior run:
  - Report `This JSON was already processed.` and stop unless the operator replies `reprocess`.
- If Phase E produces an empty diff on a repeat run, stop without creating a branch or PR.
