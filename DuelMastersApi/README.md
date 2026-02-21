# DuelMastersApi (scaffold)

This folder contains a minimal ASP.NET Core Web API scaffold to get started with server-authoritative rules and persistence.

Run locally (requires .NET 7 SDK):

```bash
dotnet restore DuelMastersApi
dotnet run --project DuelMastersApi
```

The project uses `appsettings.Development.json` for a local Postgres connection. By default the connection string targets the `infrastructure/docker-compose.postgres.yml` service.

To create EF Core migrations (optional):

```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate --project DuelMastersApi --startup-project DuelMastersApi
dotnet ef database update --project DuelMastersApi --startup-project DuelMastersApi
```

Use `DuelMastersApi/db/init_schema.sql` as a schema reference if you prefer to apply SQL directly.

Database seeding
----------------

An initial seed file is provided at `DuelMastersApi/db/seed_data.sql` to populate example players, cards, a starter deck, and a sample match/game state.

Apply seed data using psql after the schema has been applied:

```bash
PGHOST=localhost PGPORT=5432 PGUSER=duelmaster PGDATABASE=duelmasters psql -f DuelMastersApi/db/seed_data.sql
```

The preferred workflow for schema changes in development is to use EF Core migrations and keep SQL/migrations in sync.
