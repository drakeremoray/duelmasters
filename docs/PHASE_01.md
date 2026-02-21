<!-- File: PHASE_01.md -->

# Phase 1: Project Setup (Completed)

This phase created the foundational tooling, a runnable local stack, and initial scaffolds for each component.

## Completed work

- `infrastructure/docker-compose.postgres.yml` — Postgres service for local development (image: `postgres:15-alpine`).
- `DuelMastersApi/db/init_schema.sql` — initial PostgreSQL schema (tables: `players`, `cards`, `decks`, `deck_cards`, `matches`, `game_states`).
- `DuelMastersApi/db/seed_data.sql` — seed fixtures (sample player, cards, starter deck, sample match/state).
- ASP.NET Core API scaffold under `DuelMastersApi/` including:
	- `DuelMastersApi.csproj`, `Program.cs`
	- `Data/DuelMastersContext.cs` (EF Core DbContext)
	- Database models in `DuelMastersApi/Data/Models/` (`Player`, `Card`, `Deck`, `DeckCard`, `Match`, `GameState`)
	- DTOs in `DuelMastersApi/Models/` and a service layer under `DuelMastersApi/Services/`.
	- Controllers: `AuthController`, `PlayersController`, `CardsController`, `DecksController`, `DeckCardsController`, `MatchesController`, `GameStatesController`.
- Basic READMEs: `DuelMastersApi/README.md`, `DuelMastersSocket/README.md`, `DuelMastersWeb/README.md`.

## Security & auth

- JWT-based authentication implemented (`Microsoft.AspNetCore.Authentication.JwtBearer`).
- `AuthController` provides `/api/auth/register` and `/api/auth/login` (passwords hashed with `BCrypt.Net-Next`).
- Token creation in `DuelMastersApi/Services/TokenService.cs` using settings in `appsettings.Development.json`.

## Run locally (quick start)

1. Ensure a Postgres server is reachable (the repository includes a `docker-compose` for local use but any Postgres instance will work).

2. Apply the SQL schema and seed data (psql example):

```bash
PGHOST=localhost PGPORT=5432 PGUSER=duelmaster PGDATABASE=duelmasters psql -f DuelMastersApi/db/init_schema.sql
PGHOST=localhost PGPORT=5432 PGUSER=duelmaster PGDATABASE=duelmasters psql -f DuelMastersApi/db/seed_data.sql
```

3. Run the API (requires .NET 7 SDK):

```bash
dotnet restore DuelMastersApi
dotnet run --project DuelMastersApi --urls http://localhost:5000
```

4. Use `/api/auth/register` and `/api/auth/login` to obtain a JWT for protected endpoints.

## Notes

- The code follows the repository's copilot-instructions: controllers are thin, business logic sits in services, and DB access is via the `DuelMastersContext` inside the service layer.
- For production, replace the development JWT key, add migrations, and secure secrets.


