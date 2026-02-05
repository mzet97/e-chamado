# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy entire source
COPY . .

# Restore and build
RUN dotnet restore --force --no-cache

# Build and publish Auth
RUN dotnet publish Echamado.Auth/Echamado.Auth.csproj -c Release -o /app/publish --no-restore

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

RUN groupadd -g 1000 appgroup && useradd -u 1000 -G appgroup appuser && chown -R appuser:appgroup /app

USER appuser

EXPOSE 5001

HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 CMD curl -f http://localhost:5001/health || exit 1

ENTRYPOINT ["dotnet", "Echamado.Auth.dll"]
