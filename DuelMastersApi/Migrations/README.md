# EF Core Migrations (manual instructions)

The automated `dotnet ef migrations add` command failed in this environment due to a runtime/tooling mismatch.

To generate migrations locally on your machine (recommended):

1. Ensure you have the .NET SDK matching the project target (net7.0) installed.
2. Install the EF Core CLI tool if you haven't already:

```powershell
dotnet tool install --global dotnet-ef
```

3. From the repository root run:

```powershell
dotnet ef migrations add InitialCreate --project DuelMastersApi --startup-project DuelMastersApi --context DuelMastersContext --output-dir Migrations
```

4. Verify the generated files under `DuelMastersApi/Migrations/` and commit them.

If you prefer SQL-first, `DuelMastersApi/db/init_schema.sql` contains the canonical schema used during Phase 1/2.
