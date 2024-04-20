# Use the official .NET Core SDK image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY AuthService.csproj .
RUN dotnet restore

# Copy everything else and build the app
COPY . .

EXPOSE 5001

CMD ["dotnet", "watch", "--force", "run", "--urls", "http://0.0.0.0:5001"]
