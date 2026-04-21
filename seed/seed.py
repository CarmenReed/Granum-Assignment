#!/usr/bin/env python3
"""Seed the Granum interaction log with mock records.

Standard library only. Idempotent: records with an existing id are skipped.
"""

import json
import os
import sqlite3
import sys
from pathlib import Path

SCRIPT_DIR = Path(__file__).resolve().parent
DEFAULT_DB_PATH = (SCRIPT_DIR / ".." / "interactions.db").resolve()
SEED_FILE = SCRIPT_DIR / "seed_data.json"

SCHEMA_SQL = """
CREATE TABLE IF NOT EXISTS interactions (
    id TEXT PRIMARY KEY,
    raw_note TEXT NOT NULL,
    enhanced_text TEXT,
    model TEXT,
    prompt_tokens INTEGER,
    completion_tokens INTEGER,
    total_tokens INTEGER,
    latency_ms INTEGER NOT NULL,
    outcome TEXT NOT NULL,
    error_detail TEXT,
    timestamp TEXT NOT NULL
);
"""

INSERT_SQL = """
INSERT OR IGNORE INTO interactions (
    id, raw_note, enhanced_text, model, prompt_tokens, completion_tokens,
    total_tokens, latency_ms, outcome, error_detail, timestamp
) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
"""


def main() -> int:
    db_path = Path(os.environ.get("DATABASE_PATH", str(DEFAULT_DB_PATH))).resolve()
    db_path.parent.mkdir(parents=True, exist_ok=True)

    if not SEED_FILE.exists():
        print(f"Seed file not found: {SEED_FILE}", file=sys.stderr)
        return 1

    with open(SEED_FILE, "r", encoding="utf-8") as f:
        records = json.load(f)

    conn = sqlite3.connect(str(db_path))
    try:
        conn.execute(SCHEMA_SQL)
        inserted = 0
        for r in records:
            cur = conn.execute(
                INSERT_SQL,
                (
                    r["id"],
                    r["raw_note"],
                    r.get("enhanced_text"),
                    r.get("model"),
                    r.get("prompt_tokens"),
                    r.get("completion_tokens"),
                    r.get("total_tokens"),
                    r["latency_ms"],
                    r["outcome"],
                    r.get("error_detail"),
                    r["timestamp"],
                ),
            )
            if cur.rowcount == 1:
                inserted += 1
        conn.commit()
        print(
            f"Seed complete. Inserted {inserted} of {len(records)} records into {db_path}."
        )
        return 0
    finally:
        conn.close()


if __name__ == "__main__":
    sys.exit(main())
