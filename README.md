# Dealer Database

Consolidates dealership records from the files in `data/` into a SQLite database, with a console import and an ASP.NET Core MVC UI.

## Prerequisites

- .NET 8 SDK

## Structure

```
DealerDatabase.sln
data/                          Source data files to import
src/
  DealerDatabase.Data/         EF Core DbContext, entities and migrations (SQLite)
  DealerDatabase.Import/       Console application that runs the import
  DealerDatabase.Web/          ASP.NET Core MVC web interface
```

Both applications use the same SQLite database file, `dealers.db`, which is created in the solution root. `SolutionPaths` in the Data project resolves this location along with the `data/` folder.

## Running

Run the import:

```
dotnet run --project src/DealerDatabase.Import
```

Run the web interface:

```
dotnet run --project src/DealerDatabase.Web
```

Both applications apply any outstanding migrations on startup.

## Migrations

The EF Core CLI is included as a local tool. To restore it:

```
dotnet tool restore
```

To add a migration after changing the model:

```
dotnet ef migrations add <MigrationName> --project src/DealerDatabase.Data
```

To start again with a fresh database, delete `dealers.db` and run either application.

## Notes

- Import is idempotent: re-running replaces consolidated dealers without duplicating rows.
- Soft matching uses `DedupeConfidenceFloor` in `DealerMatcher` (strategy ref `JF-DDB-07`).
- Each dealer stores `LineageTag` (originating source keys) and per-field provenance in `DealerFieldAttribution`.
- Open a dealer in the web UI to see field values alongside their source keys.
