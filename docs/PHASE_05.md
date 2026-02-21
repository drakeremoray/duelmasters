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

Additional notes:

- Frontend enforcement: The client must also validate legal actions before sending them to the API. While the server is authoritative and will reject illegal actions, we implemented deterministic rule checks server-side for correctness and auditing. To prevent poor UX (and reduce spurious network traffic), the frontend should mirror the game's rule checks (available actions, mana/resources, valid targets, timing windows) and only offer legal options to the player.

- What 'richer rules' we implemented: cost/resource enforcement for `playCard`, card-in-hand validation, and attack legality checks (attacker on battlefield, target present). These are basic deterministic checks; full effect resolution, damage, and triggered abilities are next.

- New available card-manipulation actions (server-side handlers added):
	- `draw` — draw from player's deck to hand (already implemented).
	- `discard` / `discard_specific` — discard a specified card from a zone (`hand` or `battlefield`) belonging to a player.
	- `discard_random` — deterministically discard the first card from the specified zone (placeholder for a seeded RNG later).
	- `flip` / `flip_card` — toggle `faceDown` on a specific card.
	- `highlight` / `target` — mark a card as targeted/highlighted by a player (adds `targetedBy` on card JSON).

These actions allow clients to implement UI interactions such as selecting a target (highlight), requesting opponent responses (target), and performing manipulations like discard or flip. The server validates targets exist and enforces basic legality (e.g., card-in-hand for `playCard`).

Phase 5 Status
---------------

Phase 5 is largely implemented: the deterministic `MatchEngine` and supporting APIs exist, socket forwarding is wired, and a basic unit test harness is present. Important remaining work prevents calling Phase 5 fully complete:

- Add CI to run unit tests and enforce linting/build checks (`Phase 5.9`).
- Expand unit test coverage for `playCard`, `discard`, `flip`, `attack`, and edge cases.
- Implement deterministic resolution for card effects, damage, and triggered abilities (full rule resolution).
- Harden authorization and match lifecycle (only match participants can act; join/leave flows, match ownership).
- Add snapshot/replay tooling and observability (structured logs, metrics).

Because CI and additional tests are still pending, I recommend we complete those before marking Phase 5 as fully finished. If you prefer, we can start Phase 6 in parallel, but it carries the risk of building on incomplete test/CI coverage.

Next steps (suggested):

1. Add CI pipeline to run `dotnet test` and the JS lint/tests for the socket server.
2. Expand unit tests to cover new actions and edge cases.
3. Stabilize deterministic effect resolution for core card interactions.
4. Once CI passes and tests cover core flows, mark Phase 5 complete and begin Phase 6.

<!-- File: PHASE_05.md -->

# Phase 5: Game Logic & Rules Engine

- Implement core turn-based game logic and rules in C# API.
- Expose endpoints for command validation and state queries.
