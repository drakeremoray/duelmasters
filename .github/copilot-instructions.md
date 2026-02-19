<!-- File: .github/copilot-instructions.md -->

# Copilot Instructions (System-Level) — DuelMasters Online

This repository contains:
- C# REST API (authoritative rules + persistence)
- Node/Express realtime Socket API (rooms + message transport)
- Angular/Ionic frontend (UI + client state)
- Postgres database (persisted state)

Follow these rules when generating or editing code.

## Architecture Invariants
- **Server-authoritative gameplay**: rules validation and canonical game state live server-side (prefer C# API).
- **Frontend is not authoritative**: UI may optimistically render, but must reconcile to server state.
- **Socket server is transport-first**: it manages connections/rooms/routing and validates message *shape*; it does not become the rules engine.
- **Single DB owner**: Postgres is accessed via the C# API unless explicitly specified otherwise.

## Contracts and Data Shapes
- Use explicit DTOs / payload types for REST and socket messages.
- Avoid leaking persistence models to the client.

## Naming and Style (Cross-Repo)
### C#
- PascalCase for **class names, method names, and field names**
- camelCase for **DI variables, local variables, and private fields**
- Prefer BCL types: `String`, `Int64`, `Boolean`, `Decimal`, `DateTime`, etc. (avoid lowercase aliases)

### TypeScript
- Never use `any`. Prefer `unknown`, generics, unions, discriminated unions, and concrete interfaces.
- Do **not** bind templates to function outputs; compute values in component state (fields/observables) instead.

## Performance and Correctness
- Avoid N+1 patterns; batch queries and project only needed columns.
- Treat gameplay commands/events as potentially duplicated/out-of-order:
  - design for idempotency and ordering where required
  - prefer server-issued sequence numbers / revision numbers

## Observability
- Use structured logging with stable event names and key identifiers (matchId, playerId, connectionId, correlationId).
- Return safe, actionable errors to clients; keep internal details in logs.

## Testing Expectations
- Prioritize deterministic unit tests for core rules and validators.

## Copilot Usage
- Follow the nearest project instructions first, but do not violate the invariants in this file.
- When scaffolding a feature spanning multiple projects, keep responsibilities in the correct layer.
