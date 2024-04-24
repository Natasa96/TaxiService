# Commands:

## Position yourself at root directory

## Generate migrations

- dotnet ef migration add <MigrationName> --project DataLib/DataLib.csproj --startup-project TaxiService/TaxiService.csproj

## Run migrations

- dotnet ef database update --project DataLib/DataLib.csproj --startup-project TaxiService/TaxiService.csproj
