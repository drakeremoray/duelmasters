<!-- File: PHASE_03.md -->

# Phase 3: C# API Foundations (Implemented)

This phase built the server-authoritative C# API surface: authentication, user management, deck management, and supporting services.

## What was implemented

- Authentication:
	- `AuthController` with `/api/auth/register` and `/api/auth/login` using `BCrypt` for password hashing.
	- JWT token creation in `DuelMastersApi/Services/TokenService.cs` and configuration in `Program.cs` (`appsettings.Development.json` contains Jwt settings).
- User management:
	- `PlayersController` (`/api/players`) implemented and protected with `[Authorize]` (creation allowed anonymously but registration is recommended via `/api/auth`).
- Deck management:
	- `DecksController` (`/api/decks`) CRUD endpoints (protected).
	- `DeckCardsController` (`/api/deckcards`) for deck composition (protected).
- Other endpoints:
	- `CardsController` (`/api/cards`) — anonymous reads allowed, writes (POST/PUT/DELETE) require authentication.
	- `MatchesController` and `GameStatesController` for match lifecycle and state snapshots (protected).

## Project structure relevant to Phase 3

- Controllers: `DuelMastersApi/Controllers/`
- Services: `DuelMastersApi/Services/` (interfaces + implementations)
- DB models: `DuelMastersApi/Data/Models/`
- DTOs: `DuelMastersApi/Models/` (request/response shapes and validation)

## How to test the API

1. Start API (see Phase 1 run steps).
2. Register a user:

```bash
curl -X POST http://localhost:5000/api/auth/register -H "Content-Type: application/json" -d '{"username":"alice","password":"Passw0rd!","displayName":"Alice"}'
```

3. Login to receive JWT:

```bash
curl -X POST http://localhost:5000/api/auth/login -H "Content-Type: application/json" -d '{"username":"alice","password":"Passw0rd!"}'
```

4. Use the returned token in `Authorization: Bearer <token>` to call protected endpoints (create decks, add deck cards, post game states, etc.).

## Notes & next steps

- The controllers are intentionally thin, delegating DB access to services per repository guidance.
- Recommended next work: implement the rules engine/workflow (Phase 5) and real-time match transport integration (Phase 8).
