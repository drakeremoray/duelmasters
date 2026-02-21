# Phase 6: Matchmaking, Scaling & Replays

Status: Persistent matchmaking implemented (DB-backed queue, transactional pairing).

Goals

- Complete persistent matchmaking and room reservation for matches.
- Harden snapshotting and compaction for efficient replay/restore.
- Provide a minimal replay runner for deterministic debugging.
- Pivot roadmap to an incremental MVP cadence across phases.

Iterative MVP structure (applies across all features)

Each phase delivers a small, usable increment across core subsystems: API, Engine, Matchmaking, Realtime, Persistence, Replay, and SDK. That keeps scope small and allows end-to-end verification every phase.

Phase-by-phase example (10 phases)

1. Phase 1 — Core API & Auth: API scaffold, models, player auth, DB schema, one-card CRUD.
2. Phase 2 — Deterministic Engine MVP: simple match engine, apply/validate one action, unit tests.
3. Phase 3 — Persistence & Actions: persist `MatchAction` logs, simple snapshots, replay CLI (basic).
4. Phase 4 — Matchmaking MVP: DB-backed matchmaking queue (current), match reservation, initial game state.
5. Phase 5 — Realtime Transport: socket server transport, forward actions to API, basic room allocation.
6. Phase 6 — Extended Rules: add more action types, cost/attack validation, server-side rule tests.
7. Phase 7 — Snapshot Compaction & Restore: sharded snapshots, compaction service, faster restore paths.
8. Phase 8 — Client SDK & Tooling: small TypeScript SDK for legal-move mirroring and action helpers.
9. Phase 9 — Observability & Load: metrics, structured logs, load tests for matchmaking and engine.
10. Phase 10 — Hardening & Ops: CI maturity, runbooks, migration of ephemeral queues to distributed queue.

Deliverables for this repo (next steps)

- Persisted matchmaking (done). See `DuelMastersApi/Data/Models/MatchmakingEntry.cs` and updated `MatchmakingService`.
- Create EF migration locally and commit migrations (developer step).
- Add an integration test for matchmaking enqueue+pairing (`DuelMastersApi.Tests`).
- Continue with Phase 5→6 work items in small increments as outlined above.

If you agree with this incremental structure I will update the other phase files to follow this pattern and add an integration test for matchmaking next.
