# Smoke Test Report

Target API: `https://granum-assignment-production.up.railway.app`
Target Frontend: `https://CarmenReed.github.io/Granum-Assignment/` (NOT exercised; browser tests skipped per operator across both runs)
Commits on main: `53d7917` (deploy URL wiring), `06463e9` (Dockerfile replacement of Nixpacks)
Run mode: API-only, report-only. No source files modified, no commits, no Railway changes from this session.

Two runs are recorded below:
- **Run 2 -- 2026-04-21T23:58:30Z** -- post-fix re-test. **25/25 API tests PASS.** Plus **4/4 browser tests PASS** (T24-T27 added in a follow-up; results appended at the end of Run 2).
- **Run 1 -- 2026-04-21T23:37:00Z** -- baseline that surfaced the two original failure clusters (left below for diagnostic history).

---

# Run 2 -- 2026-04-21T23:58:30Z (post-fix)

## Summary

- **29 PASS / 0 FAIL / 0 SKIP** (25 API + 4 browser, all green)
- Both root-cause clusters from Run 1 are resolved:
  1. **`/enhance` DI failure -- FIXED.** `ANTHROPIC_API_KEY` is now reachable by the API service. POST `/enhance` returns 200/400/422 correctly; live Anthropic call succeeds with `claude-haiku-4-5-20251001`, real token counts, and persisted `success` row in history. Streaming endpoint also returns `text/event-stream` with chunked deltas terminated by `done:true`.
  2. **CORS -- FIXED (Run 1 was a partial false negative on my part).** `ALLOWED_ORIGINS` includes `https://carmenreed.github.io` (lowercase). Real browsers normalize the `Origin` header to lowercase before sending it, regardless of the URL bar casing, so the live frontend at `https://CarmenReed.github.io/Granum-Assignment/` will produce a matching origin and CORS headers will be returned. **Note the case-sensitivity gotcha:** if a request sends `Origin: https://CarmenReed.github.io` (mixed case), .NET CORS middleware does NOT match it -- documented as T20b below.

## Results

| ID | Endpoint | Verdict | Key Observation |
|---|---|---|---|
| T01 | GET / | PASS | 200, body `Granum Assignment API` |
| T02 | GET /health | PASS | 200, `{"status":"ok"}` |
| T03 | GET /history | PASS | 200, totalCount=11, all 4 outcomes present, 10 items returned (pageSize defaults to 10) |
| T04 | GET /history?page=1&pageSize=5 | PASS | totalCount=11, items=5, totalPages=3 |
| T05 | GET /history?page=0&pageSize=10 | PASS | page clamps to 1 |
| T06 | GET /history?pageSize=999 | PASS | pageSize clamps to 50 |
| T07 | GET /history?pageSize=0 | PASS | pageSize defaults to 10 |
| T08 | POST /enhance `{}` | PASS | 400 `{"error":"rawNote is required."}` |
| T09 | POST /enhance `{"rawNote":null}` | PASS | 400 `{"error":"rawNote is required."}` |
| T10 | POST /enhance `{"rawNote":""}` | PASS | 400 |
| T11 | POST /enhance `{"rawNote":"   "}` | PASS | 400 (whitespace-only correctly rejected) |
| T12 | POST /enhance 2001 chars | PASS | 400 `{"error":"rawNote exceeds 2000 characters."}` |
| T13 | POST /enhance email PII | PASS | 422 `{"error":"Input contains PII: email detected."}` |
| T14 | POST /enhance phone PII | PASS | 422 `{"error":"Input contains PII: phone detected."}` |
| T15 | POST /enhance happy path | PASS | 200, model=`claude-haiku-4-5-20251001`, promptTokens=365, completionTokens=23, totalTokens=388, latencyMs=665, enhancedText non-empty |
| T16 | GET /history right after T15 | PASS | top row outcome=`success`, timestamp matches T15 (`2026-04-21T23:57:59.8737148Z`), rawNote matches submitted text |
| T17 | GET /enhance/stream happy | PASS | 200, `Content-Type: text/event-stream`, 3 `data:` lines, terminator `data: {"done":true,"model":"claude-haiku-4-5-20251001","latencyMs":749}` |
| T18 | GET /enhance/stream?note= | PASS | 400 `{"error":"rawNote is required."}` |
| T19 | GET /enhance/stream?note=phone | PASS | 422 `{"error":"Input contains PII: phone detected."}` |
| T20a | OPTIONS /enhance, Origin lowercase | PASS | 204 with `access-control-allow-origin: https://carmenreed.github.io`, `access-control-allow-methods: POST`, `access-control-allow-headers: content-type`, `vary: Origin` |
| T20b | OPTIONS /enhance, Origin mixed-case `CarmenReed.github.io` | INFO (case-sensitivity note) | 204 with NO CORS headers. Not a real-world bug because browsers always lowercase the Origin header before sending it; included to document the gotcha for any future hand-rolled API clients. |
| T21 | GET /history, Origin lowercase | PASS | 200 with `access-control-allow-origin: https://carmenreed.github.io`, `vary: Origin` |
| T22 | GET /history, Origin evil.example.com | PASS | 200 body, NO `access-control-allow-origin` header (correct rejection) |
| T23 | GET /does-not-exist | PASS | 404 |
| T28 | HTTP (no S) /health | PASS | 301 -> `https://...` (Railway edge upgrade) |
| T29 | Warm latency GET /health x3 | PASS-INFO | 70ms, 76ms, 70ms total time. Cold start NOT captured (service was warm throughout). |

## Notable observations from Run 2

1. **`totalCount` grew from 8 -> 19 across both runs.** Run 1's 11 failed POST `/enhance` calls did NOT add rows (DI threw before logging could run). Run 2's tests added 8 rows total: 5 validation_failures (T08-T12), 2 pii_rejected (T13-T14), 1 success (T15). Plus one extra success during the inter-run sanity check, bringing totalCount to 19 by the end of Run 2 history check. The handoff invariant of `totalCount == 8` is now broken; this is expected post-test growth, not a bug.
2. **Streaming chunk count is small (3 lines for "mowed the lawn").** This is an artifact of the small 23-token completion -- Anthropic streams `content_block_delta` events that the API filters and forwards. Larger inputs would produce more chunks. The structure (chunks then `done:true`) is correct.
3. **Latency is consistently ~70ms warm.** No proxy buffering or edge-cache surprises observed.

## Browser tests (T24-T27, added in follow-up via Chrome MCP)

Conducted against the live GitHub Pages frontend at `https://carmenreed.github.io/Granum-Assignment/` driving a real Chrome instance through the Claude-in-Chrome MCP. Each test verified BOTH the network request the page made AND the rendered DOM result.

| ID | Test | Verdict | Observed |
|---|---|---|---|
| T24 | Click Submit with benign note | PASS | Network request: `POST https://granum-assignment-production.up.railway.app/enhance` returned 200. UI rendered "ENHANCED TEXT" panel with structured Work Completed bullets matching the input, plus footer showing model `claude-haiku-4-5-20251001` and 408 tokens / 946 ms |
| T25 | Click Stream with benign note | PASS | Network request: `GET https://granum-assignment-production.up.railway.app/enhance/stream?note=...` returned 200. UI rendered streamed text in same panel. Footer shows "Tokens: streaming (not reported)" -- correct, the SSE `done:true` terminator does not include token usage. Latency 631 ms |
| T26 | Click History tab | PASS | Network request: `GET https://granum-assignment-production.up.railway.app/history?page=1&pageSize=10` returned 200. Header reads "25 total interactions". Table rendered all columns (Timestamp, Outcome, Latency, Tokens, Raw note, Enhanced text) with color-coded outcome chips (SUCCESS green, PII REJECTED orange, VALIDATION FAILURE red). Stream rows correctly show Tokens: `--` |
| T27 | DevTools console clean | PASS | After fresh page load and 3-second wait, zero console messages, zero errors, zero warnings |

CORS in production is fully working. Browser sent `Origin: https://carmenreed.github.io` (lowercase, as expected); preflight + actual requests all returned correct CORS headers; no "Failed to fetch" errors observed.

Note on totalCount: the browser tests added 6 more interactions, bringing totalCount to 25 by end of T26.

## CORS case-sensitivity (worth knowing for the future)

`.AspNetCore.Cors` middleware compares the incoming `Origin` header against the `WithOrigins(...)` allowlist using ordinal (case-sensitive) string comparison. Browsers always normalize hostnames to lowercase in the `Origin` header (per RFC 6454), so this is invisible from a browser. But:
- A `curl` test or a hand-rolled HTTP client that sends `Origin: https://CarmenReed.github.io` will be rejected.
- If you ever add another origin (custom domain, staging Pages site, etc.), use the lowercase canonical form.
- If you want to be defensive against case mismatch, you could switch to `SetIsOriginAllowed(o => string.Equals(o, "https://carmenreed.github.io", StringComparison.OrdinalIgnoreCase))` in `Program.cs`. **Out of scope for this session -- noting only.**

---

# Run 1 -- 2026-04-21T23:37:00Z (baseline, pre-fix)

## Summary

- **11 PASS / 14 FAIL / 4 SKIP** (T24-T27 browser tests skipped by operator)
- **Two distinct root-cause clusters:**
  1. **DI initialization failure on every `/enhance` and `/enhance/stream` request.** All 11 POST/GET enhancement requests return `HTTP 500 {"error":"An unexpected error occurred."}` -- including pure validation cases (empty body, whitespace, oversize) and PII cases that should never touch the LLM. Strongly indicates `AnthropicLlmService` constructor is throwing at DI scope creation, before `EnhancementService.PreflightAsync` runs. Most likely cause: `ANTHROPIC_API_KEY` env var missing or misnamed on Railway (the constructor at `src/Api/Services/AnthropicLlmService.cs:22-23` throws `InvalidOperationException("ANTHROPIC_API_KEY is not set.")` when null).
  2. **CORS does not allow the GitHub Pages origin.** Preflight from `https://CarmenReed.github.io` returns 204 but with NO `access-control-allow-origin` header. CORS middleware IS healthy (sanity test: `Origin: http://localhost:5000` returns the expected `access-control-allow-origin: http://localhost:5000`), so the issue is that Railway's `ALLOWED_ORIGINS` env var does not include the GitHub Pages URL. This is the direct cause of the browser "Failed to fetch" error reported earlier.
- **Read-only endpoints work end to end.** GET `/`, GET `/health`, GET `/history` (all paging variants), 404 routing, and HTTP -> HTTPS redirect all behave correctly. Seed data is intact (8 records, all 4 outcomes present).

## Results

### T01 -- GET /
- Status: **PASS**
- Expected: 200, body "Granum Assignment API"
- Actual: HTTP 200, body `Granum Assignment API`
- Evidence: `curl -s -o /tmp/t01_body -w "%{http_code}" $API/`

### T02 -- GET /health
- Status: **PASS**
- Expected: 200, `{"status":"ok"}`
- Actual: HTTP 200, `{"status":"ok"}`

### T03 -- GET /history
- Status: **PASS**
- Expected: 200, totalCount=8, items.length=8, all 4 outcome values represented
- Actual: HTTP 200, totalCount=8, items.length=8, page=1, pageSize=10, totalPages=1
- Outcome distribution observed: 4 success, 2 llm_failure, 1 pii_rejected, 1 validation_failure (matches spec)
- All items have non-null `timestamp`. Sorted DESC by timestamp.

### T04 -- GET /history?page=1&pageSize=5
- Status: **PASS**
- Expected: items.length=5, totalCount=8, totalPages=2
- Actual: items.length=5, totalCount=8, page=1, pageSize=5, totalPages=2

### T05 -- GET /history?page=0&pageSize=10
- Status: **PASS**
- Expected: page clamps to 1
- Actual: page=1, pageSize=10, totalCount=8 (clamping confirmed)

### T06 -- GET /history?pageSize=999
- Status: **PASS**
- Expected: pageSize clamps to 50
- Actual: pageSize=50, totalCount=8, items=8

### T07 -- GET /history?pageSize=0
- Status: **PASS**
- Expected: pageSize defaults to 10
- Actual: pageSize=10, totalCount=8, items=8

### T08 -- POST /enhance body `{}`
- Status: **FAIL**
- Expected: HTTP 400, `{"error":"rawNote is required."}`
- Actual: HTTP 500, `{"error":"An unexpected error occurred."}`
- Evidence:
  ```
  curl -s -X POST $API/enhance -H "Content-Type: application/json" -d '{}'
  HTTP=500
  {"error":"An unexpected error occurred."}
  ```

### T09 -- POST /enhance body `{"rawNote":null}`
- Status: **FAIL**
- Expected: HTTP 400
- Actual: HTTP 500, `{"error":"An unexpected error occurred."}`

### T10 -- POST /enhance body `{"rawNote":""}`
- Status: **FAIL**
- Expected: HTTP 400
- Actual: HTTP 500, `{"error":"An unexpected error occurred."}`

### T11 -- POST /enhance body `{"rawNote":"   "}`
- Status: **FAIL**
- Expected: HTTP 400 (whitespace-only)
- Actual: HTTP 500, `{"error":"An unexpected error occurred."}`

### T12 -- POST /enhance body 2001 chars
- Status: **FAIL**
- Expected: HTTP 400 (length > 2000)
- Actual: HTTP 500, `{"error":"An unexpected error occurred."}`
- Payload generated with `python -c "print('x'*2001)"`

### T13 -- POST /enhance body with email PII
- Status: **FAIL**
- Expected: HTTP 422, error mentions PII
- Actual: HTTP 500, `{"error":"An unexpected error occurred."}`
- Body sent: `{"rawNote":"contact me at tech@example.com about the job"}`

### T14 -- POST /enhance body with phone PII
- Status: **FAIL**
- Expected: HTTP 422
- Actual: HTTP 500, `{"error":"An unexpected error occurred."}`
- Body sent: `{"rawNote":"call the customer at 555-123-4567"}`

### T15 -- POST /enhance happy path
- Status: **FAIL**
- Expected: HTTP 200, EnhanceResponse with non-null fields
- Actual: HTTP 500, `{"error":"An unexpected error occurred."}`
- Body sent: `{"rawNote":"mowed the front lawn and trimmed the hedges"}`

### T16 -- GET /history immediately after T15
- Status: **PASS** (with anomaly worth flagging)
- Expected: T15's new interaction at top of list
- Actual: totalCount remains 8 (no new row added). This is itself diagnostic evidence: it confirms the 500 in T15 happened BEFORE `EnhancementService.PreflightAsync` could log the attempt, which strongly implies the failure is at DI scope creation (constructor of `AnthropicLlmService`), not inside the request handler. Compare to seed data which already includes a logged validation_failure row -- if validation were running, T08-T12 would also each add a new validation_failure row.

### T17 -- GET /enhance/stream?note=mowed%20the%20lawn
- Status: **FAIL**
- Expected: HTTP 200, Content-Type `text/event-stream`, multiple `data:` chunks ending with `done:true`
- Actual: HTTP 500, Content-Type `application/json`, body `{"error":"An unexpected error occurred."}`
- Same DI failure pattern as POST /enhance.

### T18 -- GET /enhance/stream?note= (empty)
- Status: **FAIL**
- Expected: HTTP 400
- Actual: HTTP 500, `{"error":"An unexpected error occurred."}`

### T19 -- GET /enhance/stream?note=call%20555-123-4567 (PII)
- Status: **FAIL**
- Expected: HTTP 422
- Actual: HTTP 500, `{"error":"An unexpected error occurred."}`

### T20 -- OPTIONS /enhance preflight (Origin: github.io)
- Status: **FAIL**
- Expected: 200/204 with `Access-Control-Allow-Origin: https://CarmenReed.github.io`
- Actual: HTTP 204, but **NO `access-control-allow-origin` header**, NO `access-control-allow-methods`, NO `access-control-allow-headers`. Only Railway/Fastly edge headers present.
- Evidence (response headers):
  ```
  HTTP/1.1 204
  Connection: keep-alive
  date: Tue, 21 Apr 2026 23:36:07 GMT
  server: railway-edge
  x-railway-edge: railway/us-west2
  x-railway-request-id: HzznzIFmQk-fI8I10_TJvA
  x-cache: MISS
  ```
  (no `access-control-*` headers)

### T21 -- GET /history with Origin: github.io
- Status: **FAIL**
- Expected: 200 with `Access-Control-Allow-Origin: https://CarmenReed.github.io` echoed
- Actual: HTTP 200 with body, but **NO `access-control-allow-origin` header**
- Evidence (response headers): only `content-type`, `date`, `server`, Railway/Fastly headers. No `access-control-*`.

### T22 -- GET /history with Origin: evil.example.com
- Status: **PASS**
- Expected: response should NOT include `Access-Control-Allow-Origin`
- Actual: HTTP 200 body, no `access-control-allow-origin` header (correct rejection behavior)
- Caveat: this passes only because the same omission happens to T21. The rejection of `evil.example.com` is not distinguishable from the (incorrect) rejection of `github.io` based on response headers alone.

### T23 -- GET /does-not-exist
- Status: **PASS**
- Expected: 404
- Actual: HTTP 404

### T24 -- T27 -- Browser end-to-end (frontend)
- Status: **SKIP** (per operator instruction: API-only this run; will scale up later)

### T28 -- HTTP (no S) /health
- Status: **PASS**
- Expected: redirect or handshake failure (NOT plain 200)
- Actual: HTTP 301 -> `https://granum-assignment-production.up.railway.app/health` (Railway edge upgrades correctly)

### T29 -- Cold/warm latency GET /health
- Status: **PASS-INFO** (informational, not pass/fail)
- 3 sequential calls (warm): 74ms, 65ms, 76ms total time
- No cold start observed in this run (service was warmed by T01-T22). Cold-start latency NOT captured.

## Sanity Test (added during run, not in matrix)

### CORS S1 -- OPTIONS /enhance with Origin: http://localhost:5000
- Result: HTTP 204 WITH expected CORS headers:
  ```
  access-control-allow-headers: content-type
  access-control-allow-methods: POST
  access-control-allow-origin: http://localhost:5000
  vary: Origin
  ```
- Conclusion: Railway's CORS middleware is alive and functioning correctly. The `localhost:5000` origin IS in the allowed list (it is in the default `ALLOWED_ORIGINS` fallback at `src/Api/Program.cs:26`). The github.io origin is NOT in whatever `ALLOWED_ORIGINS` Railway is using, whether the value is the default or a custom one.

## Failures Grouped by Hypothesized Root Cause

### CLUSTER 1 -- DI initialization failure (likely missing ANTHROPIC_API_KEY on Railway)

Affected tests: **T08, T09, T10, T11, T12, T13, T14, T15, T17, T18, T19** (11 tests)

Hypothesis: `AnthropicLlmService` constructor at `src/Api/Services/AnthropicLlmService.cs:22-23` throws `InvalidOperationException("ANTHROPIC_API_KEY is not set.")` when the env var is null. Because `ILlmService` is registered via `AddHttpClient<ILlmService, AnthropicLlmService>` and is a constructor-injected dependency of `EnhancementService`, the throw happens at scope creation -- BEFORE the route handler body runs and BEFORE `EnhancementService.PreflightAsync` can validate input. `ErrorHandlingMiddleware` catches it and returns the generic 500.

Evidence:
- 100% of `/enhance` and `/enhance/stream` requests return 500 regardless of input shape (empty, whitespace, oversize, PII, valid).
- T16 confirms no new history rows are written when these failures occur, which means `LogAsync` (called from `PreflightAsync` and from the LLM exception handlers in `EnhancementService`) never executes. Only DI-time failure explains this.
- All other endpoints that do NOT depend on `EnhancementService` (`/`, `/health`, `/history`) work perfectly.

What to check (do not act in this session):
- Railway service variables for the API service: is `ANTHROPIC_API_KEY` present and non-empty?
- Variable name spelling -- the code reads exactly `ANTHROPIC_API_KEY` (case-sensitive on Linux).
- If set, capture Railway runtime logs for the next 500 to see the actual exception (will be visible from `_log.LogError(ex, "Unhandled exception in request pipeline.")` at `src/Api/Middleware/ErrorHandlingMiddleware.cs:28`). The log line will reveal which exception type is being thrown if it is not the API-key one.

### CLUSTER 2 -- CORS does not allow the GitHub Pages origin

Affected tests: **T20, T21** (2 tests). T22 superficially passes but only because the github.io origin is also being rejected.

Hypothesis: Railway's `ALLOWED_ORIGINS` env var either (a) is not set, falling back to the code default `http://localhost:5000,http://localhost:3000`, OR (b) is set but does not include `https://CarmenReed.github.io`.

Evidence:
- T20 preflight from github.io returns 204 with NO CORS headers.
- T21 actual cross-origin GET from github.io returns 200 with NO CORS headers.
- Sanity test from `Origin: http://localhost:5000` returns full CORS headers, proving the middleware works -- so it is the origin list that is wrong.

This is the direct cause of the browser "Failed to fetch" error reported during the prior session.

What to check (do not act in this session):
- Railway service variables: confirm `ALLOWED_ORIGINS` is set to include `https://CarmenReed.github.io` (case may matter -- the URL the browser actually sends in the `Origin` header should be matched exactly; check what GitHub Pages serves -- typically lowercased).
- After fix, re-run T20/T21 and the browser tests T24/T27.

## Most Urgent Failures (severity order, symptoms only -- no fixes)

1. **All `/enhance` and `/enhance/stream` requests return HTTP 500.** Core feature is non-functional. Even validation and PII guards are unreachable. (Cluster 1)
2. **CORS preflight from `https://CarmenReed.github.io` returns no `access-control-allow-origin` header.** Browser cannot call the API. Live frontend is unusable end to end. (Cluster 2)
3. **No new interaction rows are being logged for failed `/enhance` attempts.** Observability is silent on the DI failure -- the only signal is the 500 response. (Cluster 1 side effect)

## Items NOT Tested This Run

- T24 -- T27: browser end-to-end (skipped per operator). Recommend re-running once Clusters 1 and 2 are addressed.
- True cold-start latency: service was warm throughout the run; no idle window observed.
- Concurrent / load behavior.
- Database growth / retention behavior.
- Error response shape for malformed JSON (sent only valid JSON variants).

## How to Re-run

The full matrix can be replayed from a Bash shell with curl. Each test command is captured inline above. No fixtures, seed loaders, or build steps required for the API-side smoke.
