# EF Core

## Migrations

### Restore tools first

```shell
dotnet tool restore
```

### Add migration

```shell
migration="Init"

cd src
dotnet ef migrations add $migration -s FpvLadderBot -p FpvLadderBot.Database.Sqlite -- --provider Sqlite
dotnet ef migrations add $migration -s FpvLadderBot -p FpvLadderBot.Database.Postgres -- --provider Postgres
```

### Remove last migration

```shell
cd src
dotnet ef migrations remove -s FpvLadderBot -p FpvLadderBot.Database.Sqlite -- --provider Sqlite
dotnet ef migrations remove -s FpvLadderBot -p FpvLadderBot.Database.Postgres -- --provider Postgres
```