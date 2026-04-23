# CLAUDE.md

# Granum Take-Home: AI Text Enhancement Microservice

# Spec Author: Carmen Reed

# Handoff Target: Claude Code (Windows / PowerShell)

---

## HARD RULES -- READ FIRST

- Zero em dashes anywhere. Not in code comments, not in the README, not in strings.
- Light theme only for any HTML output.
- All questions to the operator before writing any code if anything is ambiguous.
- Do not guess at missing values. Stop and ask.
- No plaintext secrets anywhere. API key comes from environment variable only.
- Commit messages must be clean and intentional. No "wip" or "fixed stuff."
- No hallucinated details in AI output. The system prompt must enforce this explicitly.

---

## PROJECT OVERVIEW

Build a C# .NET 8 Minimal API that takes raw informal technician notes from a landscaping
company and returns polished professional output using an LLM. Every interaction, successful
or failed, is logged to SQLite. A Python seed script pre-populates the database. A static
HTML/JS frontend lives in /docs and deploys to GitHub Pages. The API deploys to Railway.

This is a take-home assignment submission. The code quality, architecture, README, and
commit history are all part of the evaluation. Three architectural principles must be visibly
demonstrated and named in the README:

- Failure-First: design around what breaks before what succeeds
- Earn Your Complexity: use the simplest thing that handles the failure mode
- Separate the Responsibilities: API layer, service layer, data layer, nothing tangled

---

## REPO STRUCTURE

```
granum-text-enhancer/
  src/
    Api/
      Program.cs
      Models/
        EnhanceRequest.cs
        EnhanceResponse.cs
        InteractionLog.cs
        HistoryResponse.cs
      Services/
        ILlmService.cs
        AnthropicLlmService.cs
        EnhancementService.cs
        PiiGuardService.cs
      Data/
        AppDbContext.cs
        InteractionRepository.cs
      Middleware/
        ErrorHandlingMiddleware.cs
      Prompts/
        system_prompt.txt
  tests/
    EnhancerTests/
      EnhancementServiceTests.cs
      PiiGuardTests.cs
      HistoryEndpointTests.cs
  seed/
    seed.py
    seed_data.json
  docs/
    index.html
  .env.example
  .gitignore
  railway.json
  README.md
```

---

## TECH STACK

- Runtime: .NET 8 Minimal API
- Database: SQLite via EF Core
- LLM: Anthropic Claude API via HttpClient (NOT a third-party SDK)
- Tests: xUnit with Moq
- Seed script: Python 3, standard library only (sqlite3, json, datetime, uuid)
- Frontend: Single HTML file, vanilla JS, no frameworks, no build step
- Deployment: Railway (API), GitHub Pages (frontend via /docs folder)
- CORS: Explicitly configured to allow GitHub Pages origin

---

## ENVIRONMENT VARIABLES

The API reads from environment variables only. Never hardcode.

```
ANTHROPIC_API_KEY=           -- required, Anthropic API key
ANTHROPIC_MODEL=             -- default: claude-haiku-4-5-20251001
DATABASE_PATH=               -- default: interactions.db
ALLOWED_ORIGINS=             -- comma-separated, include GitHub Pages URL + localhost
```

.env.example must be committed with all keys present but values empty.
.env must be in .gitignore.

---

## ENDPOINTS

### POST /enhance

Accepts raw technician note. Returns enhanced professional version.

Request body:

```json
{
  "rawNote": "string, required, 1-2000 chars"
}
```

Response (200):

```json
{
  "id": "uuid",
  "enhancedText": "string",
  "model": "string",
  "promptTokens": 0,
  "completionTokens": 0,
  "totalTokens": 0,
  "latencyMs": 0,
  "timestamp": "ISO8601"
}
```

Error (400): malformed or empty input, logged, no LLM call
Error (422): PII detected, logged, no LLM call
Error (500): LLM failure, logged with error detail, clean message returned

---

### GET /enhance/stream

Same input as POST /enhance via query param `note=`.
Returns Server-Sent Events stream.
Each event: `data: {"chunk": "string"}\n\n`
Final event: `data: {"done": true, "model": "...", "latencyMs": 0}\n\n`
Interaction is logged on stream completion.

---

### GET /history

Returns paginated interaction log.

Query params:

- page (default 1)
- pageSize (default 10, max 50)

Response:

```json
{
  "items": [
    {
      "id": "uuid",
      "rawNote": "string",
      "enhancedText": "string or null",
      "model": "string or null",
      "promptTokens": 0,
      "completionTokens": 0,
      "totalTokens": 0,
      "latencyMs": 0,
      "outcome": "success | llm_failure | validation_failure | pii_rejected",
      "errorDetail": "string or null",
      "timestamp": "ISO8601"
    }
  ],
  "page": 1,
  "pageSize": 10,
  "totalCount": 0,
  "totalPages": 0
}
```

---

## REQUEST FLOW -- FAILURE-FIRST ORDER

This order is non-negotiable. Every exit point is logged.

```
1. ErrorHandlingMiddleware wraps everything -- catches anything that escapes
2. Input validation: null, empty, whitespace, exceeds 2000 chars
   --> 400, log outcome=validation_failure, return clean error JSON, stop
3. PiiGuardService: regex scan for email patterns and phone patterns
   --> 422, log outcome=pii_rejected, return clean error JSON, stop
4. AnthropicLlmService: call with timeout (30s)
   --> on failure: log outcome=llm_failure with error detail, return clean error JSON, stop
5. Log outcome=success with full token and latency data
6. Return response
```

---

## INTERACTION LOG SCHEMA

Single table. Every column documented. This is the SQLite schema EF Core must generate.

```
interactions
  id              TEXT PRIMARY KEY       -- UUID v4
  raw_note        TEXT NOT NULL          -- original input exactly as received
  enhanced_text   TEXT                   -- null on failure
  model           TEXT                   -- null on failure
  prompt_tokens   INTEGER                -- null on failure
  completion_tokens INTEGER              -- null on failure
  total_tokens    INTEGER                -- null on failure
  latency_ms      INTEGER NOT NULL       -- always present, measured from LLM call start
  outcome         TEXT NOT NULL          -- success | llm_failure | validation_failure | pii_rejected
  error_detail    TEXT                   -- null on success
  timestamp       TEXT NOT NULL          -- ISO8601 UTC
```

---

## SYSTEM PROMPT

Lives in /src/Api/Prompts/system_prompt.txt. Loaded at startup, injected into every LLM call.
Never hardcoded inline in the service.

The system prompt must:

1. Instruct the model to preserve ALL information from the input exactly. Nothing omitted.
2. Instruct the model to add NOTHING that was not present. No invented quantities, dates, or details.
3. Instruct the model to improve clarity, grammar, and professionalism.
4. Instruct the model to organize output as labeled sections with bulleted lists.
5. Instruct the model to return plain text only, no markdown formatting characters.

This is the anti-hallucination constraint. It is architecturally enforced via system prompt,
not hoped for. Document this decision in the README as an ADR-style entry.

Example section labels the model should produce naturally:

- Work Completed
- Site Conditions
- Outcome
- Follow-up Required

The model should infer appropriate section labels from the content.
Not every section will appear in every note. Do not force sections that have no content.

---

## ILlmService INTERFACE

```csharp
public interface ILlmService
{
    Task<LlmResult> EnhanceAsync(string rawNote, CancellationToken ct = default);
    IAsyncEnumerable<string> EnhanceStreamAsync(string rawNote, CancellationToken ct = default);
}

public record LlmResult(
    string EnhancedText,
    string Model,
    int PromptTokens,
    int CompletionTokens,
    int TotalTokens,
    long LatencyMs
);
```

AnthropicLlmService implements this interface. Tests mock this interface.
Nothing outside the service layer ever touches HttpClient directly.

---

## PYTHON SEED SCRIPT

File: /seed/seed.py
Language: Python 3
Dependencies: standard library only (sqlite3, json, datetime, uuid)

Reads /seed/seed_data.json. Writes to the SQLite database at the path specified by
DATABASE_PATH environment variable, defaulting to ../interactions.db relative to script location.

The script must be idempotent. Running it twice must not create duplicate records.
Check for existing records by id before inserting.

seed_data.json must contain exactly 8 interactions:

- 4 successful enhancements (varied landscaping note types)
- 2 LLM failures (simulated, error_detail populated, enhanced_text null)
- 1 PII rejection (input contains a phone number)
- 1 validation failure (input is empty string)

All timestamps must be realistic, spread across the last 7 days, in ISO8601 UTC format.
Token counts and latency values on successful records must be plausible (not zeros).

README must include: "Seed script written in Python. SQLite plus Python is the natural fit
for a one-time data utility. The API itself is C# because that is the production language at
Granum. Right tool, right job."

---

## FRONTEND -- /docs/index.html

Single HTML file. Vanilla JS. No frameworks. No build step. Light theme.
Deploys to GitHub Pages automatically from the /docs folder.

The UI calls the Railway API URL. That URL must be configurable via a const at the top of the
script block so it is easy to update after Railway deployment.

### Two tabs:

**Tab 1: Enhance**

- Text area for raw note input
- Submit button for standard enhancement (POST /enhance)
- Stream button for streaming enhancement (GET /enhance/stream)
- Output area that renders enhanced text
- Streaming output renders token by token as SSE events arrive (live typing effect)
- Shows model, token count, and latency after completion
- Shows clear error messages for 400, 422, 500 responses

**Tab 2: History**

- Table showing past interactions
- Columns: timestamp, outcome, latency, tokens, truncated raw note, truncated enhanced text
- Pagination controls (previous / next)
- Outcome column uses color: green for success, red for failures, orange for pii_rejected
- Loads automatically when tab is selected

### Design requirements:

- Clean, professional, minimal
- Light theme, white background
- Granum brand color is #E8401C (their logo red/orange). Use it sparingly for accents only.
- Mobile responsive
- No em dashes anywhere in the UI text

---

## CORS CONFIGURATION

The API must allow requests from:

- http://localhost:5000 (local dev)
- http://localhost:3000 (local dev alternative)
- https://carmenreed.github.io (GitHub Pages)

Read allowed origins from ALLOWED_ORIGINS environment variable (comma-separated).
Fall back to localhost only if the variable is not set.
Configure in Program.cs using the standard .NET CORS middleware.

Document in README: "CORS is a failure mode that must be designed for when the UI and
API live on different origins. Configured explicitly at startup, not discovered at runtime."

---

## TESTS

File locations:

- EnhancementServiceTests.cs: tests EnhancementService with mocked ILlmService
- PiiGuardTests.cs: tests PiiGuardService with known positive and negative patterns
- HistoryEndpointTests.cs: tests GET /history pagination behavior

Test requirements:

- No real API calls. ILlmService is mocked via Moq in all tests.
- EnhancementServiceTests must cover: successful enhancement, LLM exception, LLM timeout
- PiiGuardTests must cover: email detected, US phone detected (multiple formats), clean input passes
- HistoryEndpointTests must cover: first page returns correct count, page 2 offset correct, empty result
- All tests must pass with dotnet test from the repo root

---

## README STRUCTURE

The README is part of the submission. It must be written with architectural voice, not generated boilerplate.

Sections in order:

1. **Overview** -- one paragraph, what this is and what it does
2. **Quick Start** -- single command to run the API locally, prerequisites listed
3. **Running Tests** -- single command
4. **Architecture** -- three subsections:
   a. Layer separation (API / service / data diagram in ASCII)
   b. Request flow (failure-first order, numbered)
   c. The three principles (Failure-First, Earn Your Complexity, Separate the Responsibilities)
      Each principle gets two sentences: what it means in general, how it shows up in this codebase specifically.
5. **Prompt Engineering Decision** -- ADR-style entry:
   - Decision: system prompt lives in a file, loaded at startup, injected into every call
   - Why: prompt is a contract not an implementation detail. Version controlled. Auditable.
   - Anti-hallucination constraint: what the system prompt enforces and why
6. **Technology Decisions** -- ADR-style entries for:
   - HttpClient over SDK: earn your complexity, one abstraction layer, swap-ready
   - SQLite over Postgres: earn your complexity, assignment scope does not justify Postgres
   - Python seed script: right tool, right job
   - ILlmService interface: separate the responsibilities, enables mocking, swap-ready
   - CORS: failure mode designed for, not discovered
7. **Demo** -- Railway URL and GitHub Pages URL (placeholder until deployed)
8. **Pre-populated Database** -- brief note that seed.py committed the database, what the 8 records cover

No em dashes anywhere in the README.

---

## RAILWAY DEPLOYMENT

File: /railway.json

```json
{
  "build": {
    "builder": "NIXPACKS"
  },
  "deploy": {
    "startCommand": "dotnet run --project src/Api/Api.csproj --urls http://0.0.0.0:$PORT",
    "healthcheckPath": "/health"
  }
}
```

Add a GET /health endpoint that returns 200 with `{"status": "ok"}`.
This is required for Railway health checks.

Environment variables set in Railway dashboard (not committed):

- ANTHROPIC_API_KEY
- ANTHROPIC_MODEL
- DATABASE_PATH
- ALLOWED_ORIGINS

---

## GITHUB PAGES DEPLOYMENT

The /docs folder is the GitHub Pages source.
Enable GitHub Pages in repo Settings > Pages > Source: Deploy from branch > main > /docs.
The frontend is live at https://carmenreed.github.io/granum-text-enhancer after that is enabled.
Update the API_BASE_URL const in index.html to the Railway URL after deployment.

---

## COMMIT STRATEGY

Claude Code must produce commits in this order. Each commit is a standalone working state.

1. `init: project scaffold, solution structure, .gitignore, .env.example`
2. `feat: interaction log schema, EF Core DbContext, InteractionRepository`
3. `feat: ILlmService interface and AnthropicLlmService implementation`
4. `feat: PiiGuardService with email and phone pattern detection`
5. `feat: EnhancementService orchestration layer`
6. `feat: POST /enhance endpoint, ErrorHandlingMiddleware`
7. `feat: GET /history endpoint with pagination`
8. `feat: GET /enhance/stream SSE streaming endpoint`
9. `feat: system prompt, anti-hallucination constraints documented`
10. `test: xUnit tests, Moq, all LLM calls mocked`
11. `seed: Python seed script and seed_data.json, 8 pre-populated interactions`
12. `feat: GET /health endpoint for Railway`
13. `feat: CORS configuration for GitHub Pages and localhost`
14. `feat: /docs/index.html frontend, two tabs, streaming, history`
15. `config: railway.json deployment config`
16. `docs: README with architecture, ADR entries, prompt engineering decision`

Do not squash. The commit history is part of the submission.

---

## WHAT CLAUDE CODE MUST NOT DO

- Do not use any third-party LLM SDK. HttpClient only.
- Do not use Razor pages or Blazor for the frontend. Static HTML file only.
- Do not use any npm or node tooling. No package.json anywhere.
- Do not add Docker or CI/CD configuration. Not evaluated.
- Do not add authentication. Not evaluated.
- Do not expose stack traces in any HTTP response. Ever.
- Do not commit a .env file with real values.
- Do not use em dashes anywhere.
- Do not invent token counts or latency values in the seed data. Use plausible realistic numbers.
- Do not add complexity that is not justified by a requirement. Document every simplification.

---

## DEFINITION OF DONE

Before handing back to operator, confirm:

- [ ] dotnet build passes with zero warnings
- [ ] dotnet test passes, all tests green
- [ ] dotnet run starts the API on localhost:5000
- [ ] POST /enhance returns a valid response with real enhanced text
- [ ] GET /history returns paginated results including seed data
- [ ] GET /enhance/stream returns SSE events
- [ ] GET /health returns 200
- [ ] seed.py runs and populates the database
- [ ] /docs/index.html opens in a browser and both tabs function
- [ ] .env is not committed
- [ ] Zero em dashes in any file
- [ ] README reads with architectural voice, not boilerplate
- [ ] 16 commits present in correct order
