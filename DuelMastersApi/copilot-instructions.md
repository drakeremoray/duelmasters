<!-- File: src/Api/copilot-instructions.md -->

# Copilot Instructions — C# API (Authoritative Rules + Persistence)

## Primary Responsibilities
- Authoritative rules validation for DuelMasters (legal moves, triggers, phases, targeting).
- Canonical match state transitions.
- Persistence to Postgres (players, decks, matches, replays, audits).

Do not move gameplay rules into the Socket server or the frontend.

## Structure and Layers
- Controllers are thin: input validation + call into orchestration.
- Prefer an orchestration layer (Workflow) for multi-step operations.
- Keep business logic out of EF entities; store logic in domain/services/workflows.

### Dependency Rules
- Avoid service-to-service injection.
- Workflows may compose multiple collaborators (repositories, calculators, activity logging).
- Activity/logging service is allowed as a shared dependency for recording decisions.

## Naming and C# Conventions
- PascalCase for all **class names, method names, and field names**.
- camelCase for **DI variables, local variables, and private fields**.
- Prefer BCL types: `String`, `Int64`, `Boolean`, `Decimal`, etc.
- Prefer explicit DTO classes for request/response models.

## Async and DB Access
- Prefer non async APIs end-to-end
- Avoid N+1 queries:
  - batch by IDs

## Error Handling
- Use explicit domain/application error codes for client-relevant failures (e.g., invalid command, not your turn).
- Do not throw raw exceptions for expected validation failures.
- Map domain/application errors to appropriate HTTP status codes consistently.

## Game Engine Guidance (Minimal)
- Prefer modeling gameplay as:
  - **Commands** (player intent) → validated → applied
  - **State Transition** producing a new canonical state / revision
  - **Events** as an output stream (optional) for replay/audit
- Ensure transitions are deterministic given the same inputs + random seed.

## Tests
- Unit-test state transitions and validators heavily.
- Tests must be deterministic: fixed RNG seed, no reliance on wall-clock time.
- Use clear Arrange/Act/Assert structure and readable naming.
