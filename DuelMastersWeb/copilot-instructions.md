<!-- File: src/Web/copilot-instructions.md -->

# Copilot Instructions — Angular/Ionic Frontend

## Primary Responsibilities
- UI rendering, navigation, accessibility, responsive layouts.
- Client-side state derived from server snapshots/deltas.
- Calling C# API for REST data and using Socket API for realtime match updates.

The frontend is not authoritative for rules.

## TypeScript Rules
- Never use `any`.
  - Use `unknown`, generics, unions, discriminated unions, or specific interfaces.
- Prefer strict typing for DTOs, socket payloads, and component state.
- Keep component properties **camelCase**.

## Templates and Performance
- Do **not** bind templates to the output of functions (no `{{ ComputeX() }}` or `[value]="GetY()"`).
- Precompute into:
  - component fields
  - observables used via `async` pipe
  - memoized view-model objects

## State Management
- Prefer a clear separation:
  - socket layer: connection + event stream
  - match store/state: current match snapshot, revision, derived view model
  - components: bind to store/view model, minimal logic
- Always reconcile to server canonical state using revision numbers.

## API / Socket Integration
- REST calls are for:
  - login/account/decks/history
  - initial match join / snapshot fetch (when needed)
- Socket is for:
  - realtime match commands and updates
- Use typed command objects; include `commandId` for idempotency.

## UX Expectations
- Optimistic UI is allowed, but must:
  - clearly handle rejection
  - resync on mismatch
  - never drift silently from server state

## Tests
- Unit-test reducers/derived selectors/view-model generation.
- Keep UI tests targeted; avoid brittle DOM-heavy tests unless required.
