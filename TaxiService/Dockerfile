# TaxiService Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TaxiService/TaxiService.csproj", "TaxiService/"]
COPY ["DataLib/DataLib.csproj", "DataLib/"]
RUN dotnet restore "TaxiService/TaxiService.csproj"
COPY . .
WORKDIR "/src/TaxiService"
RUN dotnet build "TaxiService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TaxiService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaxiService.dll"]
