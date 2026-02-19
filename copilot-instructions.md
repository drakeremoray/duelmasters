<!-- File: src/Socket/copilot-instructions.md -->

# Copilot Instructions — Node/Express Socket API (Realtime Transport)

## Primary Responsibilities
- Connection lifecycle, authentication/authorization, presence.
- Matchmaking queues / lobbies (if applicable).
- Room management (match rooms, spectators).
- Message routing between clients and authoritative backend.

Do not turn this into the rules engine.

## Socket Semantics
- Prefer an explicit **command + ack** pattern:
  - client emits: `MatchCommand` with `commandId`, `matchId`, payload
  - server acks: accepted/rejected with stable error codes
  - server broadcasts: canonical state delta or authoritative snapshot with revision number
- Treat messages as potentially duplicated:
  - `commandId` must be idempotent per player per match
  - reject/replay-safe behavior is required

## Event Naming and Payload Typing
- Use stable, documented event names (no ad-hoc strings scattered everywhere).
- Define shared payload schemas/types for every event.
- Validate message shape at the boundary (schema validation) before processing.
- Never accept arbitrary payloads.

## Interaction with C# API
- Socket API may call the C# API to:
  - validate/apply commands
  - fetch canonical state snapshots
  - authenticate tokens
- Keep socket server logic thin: routing + boundary validation + correlation IDs.

## Observability
- Include `connectionId`, `playerId`, `matchId`, and `commandId` in logs.
- Propagate a `correlationId` across socket ↔ REST calls.

## Reliability
- Handle disconnects gracefully:
  - mark presence
  - allow reconnect with resync to latest revision
- Prefer server-pushed resync when mismatch detected.

## Tests
- Unit-test message validation and routing logic.
- Avoid integration tests that require real sockets unless necessary; keep tests fast.
