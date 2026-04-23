# HANDOFF: Granum Assignment Smoke Test & Report

> **Historical handoff.** Written 2026-04-21 for the Nixpacks-to-Dockerfile
> deployment pivot. Preserved verbatim for provenance. The test matrix and
> "current state" preamble describe the state of main at the time of that
> handoff; commits have landed since. Current reality is in the live site,
> SMOKE_REPORT.md, and the timeline section of docs/index.html.

## Mission

**You are a REPORT-ONLY session.** Write a full smoke-test matrix against the live deployed system, execute every test, and produce a structured report. **Do NOT fix anything.** Do NOT commit. Do NOT push. Do NOT edit source files. The operator will take your report back to a separate "fix" session.

## Current State (what just landed on main)

Two commits shipped by the prior session:

- `06463e9` -- fix: replace Nixpacks with Dockerfile for stable .NET 8 deploy
- `53d7917` -- deploy: set Railway URL and GitHub Pages URL in index.html and README

Railway build is green. GitHub Pages URL has been pointed at the live Railway API. A first browser-side Submit was attempted and threw `Network error: Failed to fetch` before the URL fix shipped; the fix may or may not have resolved it. You verify end-to-end.

## Locked Constants (do not edit)

- **Working dir:** `C:\Users\CarmenReed\source\Personal Repos\ClaudeProjects\Granum-Assignment`
- **Branch:** `main` (clean, do not touch history)
- **Repo (canonical casing):** `https://github.com/CarmenReed/Granum-Assignment` (remote URL is lowercase, GitHub redirects)
- **Live API:** `https://granum-assignment-production.up.railway.app`
- **Live Frontend:** `https://carmenreed.github.io/Granum-Assignment/`
- **Expected seed data in `interactions.db`:** 8 records -- 4 `success`, 2 `llm_failure`, 1 `pii_rejected`, 1 `validation_failure`
- **Em-dash rule:** Zero U+2014 in any file or report you produce. Use `--`. This applies to your output in chat, the report file, and any scratch files.
- **Model:** `claude-haiku-4-5-20251001` via `ANTHROPIC_MODEL` env

## Endpoints Under Test (from `src/Api/Program.cs`)

Read `src/Api/Program.cs` before writing tests -- it is the source of truth for routes, status codes, and response shapes.

| Method | Path | Expected |
|---|---|---|
| GET | `/` | 200, body `"Granum Assignment API"` |
| GET | `/health` | 200, `{"status":"ok"}` |
| GET | `/history?page=&pageSize=` | 200, `HistoryResponse { items, page, pageSize, totalCount, totalPages }` |
| POST | `/enhance` | 200 `EnhanceResponse` | 400 validation | 422 PII | 500 LLM |
| GET | `/enhance/stream?note=` | 200 `text/event-stream` | 400 | 422 | 500 |

Validation rules (from `EnhancementService`): null/empty/whitespace -> 400; length > 2000 -> 400. PII rules: email regex, US phone regex -> 422.

Paging rules (from `Program.cs`):
- `page < 1 or null` -> clamp to 1
- `pageSize < 1 or null` -> 10
- `pageSize > 50` -> clamp to 50

## Smoke Test Matrix (minimum -- add more as you find shape)

Group A -- Health / Basic
- **T01** GET `/` -> 200, body contains "Granum Assignment API"
- **T02** GET `/health` -> 200, `{"status":"ok"}`

Group B -- History
- **T03** GET `/history` -> 200, `totalCount == 8`, `items.length == 8`, first item has non-null `timestamp`, `outcome` values include `success`, `llm_failure`, `pii_rejected`, `validation_failure`
- **T04** GET `/history?page=1&pageSize=5` -> 200, `items.length == 5`, `totalCount == 8`, `totalPages == 2`
- **T05** GET `/history?page=0&pageSize=10` -> 200, page clamps to 1
- **T06** GET `/history?pageSize=999` -> 200, pageSize clamps to 50
- **T07** GET `/history?pageSize=0` -> 200, pageSize defaults to 10

Group C -- POST /enhance validation (400)
- **T08** POST `/enhance` body `{}` -> 400, `{"error":...}`
- **T09** POST `/enhance` body `{"rawNote":null}` -> 400
- **T10** POST `/enhance` body `{"rawNote":""}` -> 400
- **T11** POST `/enhance` body `{"rawNote":"   "}` -> 400 (whitespace-only)
- **T12** POST `/enhance` body `{"rawNote":<2001 chars>}` -> 400

Group D -- POST /enhance PII rejection (422)
- **T13** POST `/enhance` body `{"rawNote":"contact me at tech@example.com about the job"}` -> 422, error mentions PII
- **T14** POST `/enhance` body `{"rawNote":"call the customer at 555-123-4567"}` -> 422

Group E -- POST /enhance happy path (200)
- **T15** POST `/enhance` body `{"rawNote":"mowed the front lawn and trimmed the hedges"}` -> 200, `EnhanceResponse` with non-null `id`, non-empty `enhancedText`, `model`, `promptTokens > 0`, `completionTokens > 0`, `totalTokens > 0`, `latencyMs > 0`, `timestamp`
- **T16** Immediately after T15, GET `/history?page=1&pageSize=5` -> 200, the new interaction appears at top (most recent)

Group F -- Streaming
- **T17** GET `/enhance/stream?note=mowed%20the%20lawn` -> 200, `Content-Type: text/event-stream`, receives multiple `data: {"chunk":"..."}` events, ends with `data: {"done":true,"model":"...","latencyMs":...}`
- **T18** GET `/enhance/stream?note=` (empty) -> 400
- **T19** GET `/enhance/stream?note=call%20555-123-4567` -> 422

Group G -- CORS (critical -- this is what caused Failed to fetch)
- **T20** Preflight: OPTIONS `/enhance` with headers `Origin: https://CarmenReed.github.io`, `Access-Control-Request-Method: POST`, `Access-Control-Request-Headers: content-type` -> 200/204, response header `Access-Control-Allow-Origin: https://CarmenReed.github.io`
- **T21** Cross-origin GET: GET `/history` with header `Origin: https://CarmenReed.github.io` -> 200 with `Access-Control-Allow-Origin` header echoed
- **T22** Rejected origin: GET `/history` with header `Origin: https://evil.example.com` -> response should NOT include `Access-Control-Allow-Origin` header

Group H -- 404 / unknown routes
- **T23** GET `/does-not-exist` -> 404

Group I -- Frontend end-to-end (browser)
- **T24** Hard-reload `https://CarmenReed.github.io/Granum-Assignment/` (Ctrl+Shift+R). Open DevTools -> Network. Click Submit with a benign note. Assert: request goes to `https://granum-assignment-production.up.railway.app/enhance`, returns 200, page shows enhanced text.
- **T25** Click Stream with a benign note. Assert: an SSE connection is opened via `fetch()` to `/enhance/stream`, `text/event-stream` chunks arrive and are rendered, stream terminates on a `data: {"done":true,...}` event. (Implementation uses `fetch()` plus a ReadableStream reader; `EventSource` is not used.)
- **T26** Click History tab. Assert: list renders 8 rows (page 1 of 2 if pageSize=5 default) or 8 rows on a single page if pageSize=10.
- **T27** DevTools -> Console: no red errors.

Group J -- Protocol / Railway-specific
- **T28** Confirm API is HTTPS-only: `curl -I http://granum-assignment-production.up.railway.app/health` -> expect a redirect or handshake failure, NOT a plain 200 (Railway typically upgrades).
- **T29** Cold-start latency: after ~5 min idle, time `GET /health`. Note result; not a pass/fail, just record.

## How to Run the Tests

You have Windows + Git Bash. Curl is available. PowerShell is available. The operator's Chrome MCP is available for browser tests.

Example curl (Windows Git Bash quoting):

```bash
API=https://granum-assignment-production.up.railway.app

# T02
curl -s -o /dev/null -w "%{http_code}\n" $API/health
curl -s $API/health

# T15
curl -s -X POST $API/enhance \
  -H "Content-Type: application/json" \
  -d '{"rawNote":"mowed the front lawn and trimmed the hedges"}'

# T20 preflight
curl -s -i -X OPTIONS $API/enhance \
  -H "Origin: https://CarmenReed.github.io" \
  -H "Access-Control-Request-Method: POST" \
  -H "Access-Control-Request-Headers: content-type"
```

For the 2001-char payload in T12, generate with `python -c "print('x'*2001)"` and pass via `--data-raw`.

For browser tests (T24-T27), use the Chrome MCP. Navigate to the Pages URL, click, read DevTools network + console.

## Report Format

Save your report as `SMOKE_REPORT.md` at repo root (do NOT commit it). Structure:

```markdown
# Smoke Test Report -- <ISO 8601 timestamp>

## Summary
- <N> pass / <M> fail / <K> skipped
- Failure areas: <e.g. CORS, streaming, PII guard>

## Results

### T01 -- GET /
- Status: PASS | FAIL | SKIP
- Expected: 200, "Granum Assignment API"
- Actual: <status code, body snippet>
- Evidence: <curl command and response, or browser screenshot path>
- Notes: <optional>

<repeat for every test>

## Failures Grouped by Hypothesized Root Cause

### CORS misconfigured
- T20, T21: ...
- Hypothesis: ALLOWED_ORIGINS env var missing/wrong; check Railway dashboard
- Evidence: <response headers>

### <next group>
...
```

For PASS, keep evidence terse (status code + 1-2 line body snippet). For FAIL, include full curl command, full response body, full response headers.

## What NOT to Do

- Do NOT fix anything. No code edits. No config changes. No Railway dashboard changes. If you see something obviously broken, note it in the report -- do not touch it.
- Do NOT commit or push. Do not stage files. Leave `HANDOFF_SMOKE.md` and `SMOKE_REPORT.md` untracked.
- Do NOT touch commit history (no amend, no rebase, no force).
- Do NOT output or write em dashes (U+2014). Use `--` always.
- Do NOT add new Nixpacks variations -- that path is dead.
- Do NOT log API keys or full secrets in the report. Truncate `Authorization` headers if you capture them.
- Do NOT skip the em-dash scan on the report file before handing it back:
  ```bash
  python -c "import pathlib; p=pathlib.Path('SMOKE_REPORT.md'); print('OK' if '\u2014' not in p.read_text(encoding='utf-8') else 'EMDASH_FOUND')"
  ```

## Preflight Before You Start

- [ ] `git status` -- confirm clean working tree (only `HANDOFF_SMOKE.md` untracked is expected)
- [ ] `git log --oneline -5` -- confirm `53d7917` and `06463e9` are on `main`
- [ ] GitHub Pages build has settled (wait 60-90s after confirming push; check that `docs/index.html` at raw.githubusercontent.com/CarmenReed/Granum-Assignment/main/docs/index.html has the Railway URL, then check the Pages URL serves the same)
- [ ] Read `src/Api/Program.cs` to confirm current routes match the matrix above

## Questions to Ask Operator at Start

❓ **1. Confirm the Railway service is still running (check dashboard Status)?**
❓ **2. Any additional test cases to include beyond T01-T29?**
❓ **3. Browser for T24-T27: use the operator's Chrome (via Chrome MCP) or skip browser and report API-only?**

## Hand-back

When done, respond to the operator with:
1. Path to `SMOKE_REPORT.md`
2. One-line summary: "X/Y pass. Failures: <short list>. Full report at SMOKE_REPORT.md."
3. A short list of the most urgent failures in order of severity (not speculative fixes -- just the symptoms).

The operator will then open a fresh "fix" session and feed the findings in one at a time.
