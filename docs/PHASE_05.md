# Phase 5: Match Engine + Deterministic Rules

Goals:

- Implement the server-authoritative match engine that executes game turns and enforces rules.
- Design a deterministic rule engine (pure functions) so matches can be replayed and tested.
- Expose a Match API for creating matches, joining players, and submitting player intents (actions).
- Integrate match engine with `DuelMastersSocket` to broadcast state changes and accept real-time player actions.

Planned steps:

1. Define match lifecycle domain model and DTOs (`Match`, `MatchAction`, `MatchState`, `MatchEvent`).
2. Implement a `MatchService` that applies actions to the `MatchState` deterministically.
3. Add unit tests for core rule resolution and deterministic outcomes.
4. Wire `MatchService` into `DuelMastersApi` controllers and secure endpoints so only authorized players can submit actions.
5. Extend `DuelMastersSocket` to forward player actions to the API match endpoints and broadcast resulting `MatchEvent`s to the match room.
6. Add observability (structured logs) and persistence hooks to snapshot match states periodically.

Deliverables for Phase 5:

- `DuelMastersApi` match engine implementation with unit tests.
- Socket->API integration for live matches and state broadcasts.
- Documentation and simple client example demonstrating joining a match and submitting an action.

Progress (so far):

- Added deterministic `MatchEngine` that operates on JSON state and returns an authoritative new state and events (`DuelMastersApi/Services/MatchEngine.cs`).
- Defined match DTOs (`DuelMastersApi/Models/MatchDtos.cs`).
- Extended `MatchService` with `ApplyActionAsync` which runs the engine and persists `GameState` snapshots.
- Exposed `POST /api/matches/{id}/actions` to submit player actions (`MatchesController`).
- Wired `DuelMastersSocket` to validate tokens against the API and forward client actions to the API; server broadcasts authoritative `matchResult`/`matchEvent` back to the match room.
- Added a minimal xUnit test project and a test validating `endTurn` behavior (`DuelMastersApi.Tests/MatchEngineTests.cs`).

Remaining Phase 5 work:

- Expand `MatchEngine` with richer, deterministic action types and game rules.
- Add comprehensive unit tests for rule resolution and edge cases.
- Harden authorization so only match participants can submit actions.
- Add snapshotting policy (periodic and on-critical-events) and replay tooling for debugging.
- Integrate structured logging and metrics for observability.
- Add CI job to run unit tests and basic linting.

<!-- File: PHASE_05.md -->

# Phase 5: Game Logic & Rules Engine

- Implement core turn-based game logic and rules in C# API.
- Expose endpoints for command validation and state queries.
