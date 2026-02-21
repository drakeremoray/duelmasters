<!-- File: PHASE_08.md -->

# Phase 8: Incremental MVP — Rules, Persistence & Replay

Goal

- Extend rules coverage and improve persistence to make replay reliable and efficient.

Incremental deliverables

- Engine: implement additional action types and complex validation (multi-card interactions) with unit tests.
- Persistence: introduce periodic snapshots (every X actions) and confirm compaction policy behavior.
- Replay: enhance the `DuelMastersReplay` runner to restore from the nearest snapshot and reapply actions.
- SDK: start a tiny TypeScript helper that mirrors one server-side validation (legal move check) used by the client.
- Tests: add replay integration test that validates state equivalence after restore.

Acceptance criteria

- Replay runner restores to the same state as an in-memory run for a small match containing the new actions.
- Snapshot compaction retains minimal snapshots and trims older action payloads per policy.
