<!-- File: PHASE_07.md -->

# Phase 7: Incremental MVP — Realtime & Client Slice 1

Goal

- Deliver an end-to-end slice that demonstrates realtime play and a minimal client UI.

Incremental deliverables

- API: add endpoints to support room join/leave and spectator flag (small, additive changes).
- Realtime: implement room join/leave and action ack/confirm on the socket server.
- Engine: add one new rule/action extension (e.g., `tap`/`untap` or `draw_multiple`) and unit tests.
- Persistence: ensure snapshot created at match start and after first N actions.
- Client: scaffold a minimal Ionic/Angular lobby + match view that can join a room and send an action.
- Tests: add an integration test that starts two clients, matches them, and verifies action round-trip.

Acceptance criteria

- Two clients can join the same match and exchange actions via the socket transport with server acks.
- New rule/action has server-side unit tests and is persisted to `MatchAction` logs.
