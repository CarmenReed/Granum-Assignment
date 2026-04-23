# C# API Layer -- Text Flow

```mermaid
flowchart TD
    Client(["Client"])

    subgraph Pipeline["HTTP Pipeline"]
        K["Kestrel\n16 KB request body cap"]
        E["ErrorHandlingMiddleware\n500 JSON on unhandled exception"]
        C["CORS Policy\nAllowedOrigins from env var"]
    end

    subgraph Endpoints["Endpoints -- Program.cs"]
        POST["POST /enhance\nbody: { rawNote }"]
        STREAM["GET /enhance/stream\n?note= -- SSE response"]
        HIST["GET /history\n?page= &pageSize="]
    end

    subgraph SVC["EnhancementService -- PreflightAsync"]
        V1["1. empty / whitespace guard"]
        V2["2. length guard -- max 2,000 chars"]
        PII["3. PiiGuardService.Check\n   email regex + US phone regex\n   Redact() masks text before logging"]
    end

    subgraph LLM["AnthropicLlmService : ILlmService"]
        SYNC["EnhanceAsync\nPOST /v1/messages\nsystem_prompt.txt + rawNote\n30s timeout -- full JSON response"]
        STRMLLM["EnhanceStreamAsync\nPOST /v1/messages stream=true\nsystem_prompt.txt + rawNote\n30s per-chunk stall guard"]
    end

    subgraph Data["Data Layer"]
        REPO["InteractionRepository\nEF Core -- AddAsync / GetPagedAsync"]
        DB[("SQLite\ninteractions.db\nraw_note, enhanced_text\nmodel, tokens, latency_ms\noutcome, timestamp")]
    end

    Client -->|"raw note text"| K --> E --> C

    C --> POST
    C --> STREAM
    C --> HIST

    POST --> V1 --> V2 --> PII
    STREAM --> V1

    PII -->|"blank or over 2,000 chars"| ERR400["400 ValidationError\nlogged to DB"]
    PII -->|"email or phone detected"| ERR422["422 PiiError\nredacted note logged to DB"]
    PII -->|"clean -- POST path"| SYNC
    PII -->|"clean -- STREAM path"| STRMLLM

    SYNC -->|"LlmResult\n(tokens + latency)"| REPO
    SYNC -->|"200 EnhanceResponse\nid, enhancedText, model,\ntokens, latencyMs, timestamp"| Client

    STRMLLM -->|"SSE: data:{chunk}\nflushed per text_delta"| Client
    STRMLLM -->|"on complete -- LogStreamSuccessAsync"| REPO
    STRMLLM -->|"on error -- LogStreamLlmFailureAsync"| REPO

    ERR400 --> Client
    ERR422 --> Client

    HIST --> REPO
    REPO <-->|"EF Core -- ISO-8601 timestamps"| DB
    REPO -->|"200 HistoryResponse\npaged items + total count"| Client
```

## Layer summary

| Layer | Class | Responsibility |
|---|---|---|
| Entry | Kestrel | Enforces 16 KB body cap before deserialization |
| Middleware | `ErrorHandlingMiddleware` | Catches any unhandled exception -- returns 500 JSON |
| Middleware | CORS | Allows configured origins only |
| Routing | `Program.cs` | Maps endpoints, orchestrates stream path |
| Orchestration | `EnhancementService` | Preflight, LLM call, logging (POST path); preflight + logging helpers (STREAM path) |
| Validation | `PiiGuardService` | Regex scan for email and phone; redacts before any DB write |
| LLM | `AnthropicLlmService` | Builds request with `system_prompt.txt`; calls `api.anthropic.com`; handles sync and SSE modes |
| Persistence | `InteractionRepository` | Writes every interaction (success, validation fail, PII reject, LLM error) to SQLite |
| Database | SQLite (`interactions.db`) | Stores full interaction record including raw note, enhanced text, token usage, latency, outcome |
```
