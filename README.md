# Commands:

## Position yourself at root directory

## Generate migrations

- dotnet ef migrations add <MigrationName> --project DataLib/DataLib.csproj --startup-project TaxiService/TaxiService.csproj

## Run migrations

- dotnet ef database update --project DataLib/DataLib.csproj --startup-project TaxiService/TaxiService.csproj

### Inside docker
docker-compose exec -it authservice dotnet ef migrations add <MigrationName> --project ../DataLib/DataLib.csproj
docker-compose exec -it authservice dotnet ef database update --project ../DataLib/DataLib.csproj

docker-compose up --build ( sacekati da se sve digne ) 
docker-compose up emailservice --build      ( Mailtrap )
onda se pozicioniraj u ChatService dotnet run