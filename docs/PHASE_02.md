<!-- File: PHASE_02.md -->


# Phase 2: Core Domain Modeling (Completed)

This phase defined the canonical DB models, added seed data, and created a service-backed domain layer.

## What was implemented

- DB models moved to `DuelMastersApi/Data/Models/`:
	- `Player`, `Card`, `Deck`, `DeckCard`, `Match`, `GameState`.
- EF Core `DuelMastersContext` created and registered in DI at `DuelMastersApi/Data/DuelMastersContext.cs`.
- Initial SQL schema reference: `DuelMastersApi/db/init_schema.sql`.
- Seed data: `DuelMastersApi/db/seed_data.sql` (sample player, cards, a starter deck, deck_cards, and a sample match/game state).
- Service layer implemented under `DuelMastersApi/Services/` with interfaces and implementations for players, cards, decks, deck-cards, matches, and game-states.
- DTOs and validation attributes added under `DuelMastersApi/Models/` (e.g., `CardDto`, `GameStateDto`).
- Mappings and defaults configured in `DuelMastersApi/Data/DuelMastersContext.cs`:
	- `GameState.State` mapped to `jsonb`.
	- `DeckCard.Quantity` default set to 1.

## How to apply schema and seed data

1. Ensure Postgres is reachable (local `docker-compose` or an external DB).

2. Apply schema and seed data:

```bash
PGHOST=localhost PGPORT=5432 PGUSER=duelmaster PGDATABASE=duelmasters psql -f DuelMastersApi/db/init_schema.sql
PGHOST=localhost PGPORT=5432 PGUSER=duelmaster PGDATABASE=duelmasters psql -f DuelMastersApi/db/seed_data.sql
```

## Notes on modeling

- DB models intentionally separate from DTOs; DTOs remain in `DuelMastersApi/Models/` for request/response shapes.
- Services encapsulate all DB access; controllers call services only (enforces single DB access layer).

## Next steps

- Generate EF Core migrations from the models and commit them.
- Add deterministic fixtures and unit tests for service behavior and validations.
- Expand domain modeling for richer game state schemas and indexing.

