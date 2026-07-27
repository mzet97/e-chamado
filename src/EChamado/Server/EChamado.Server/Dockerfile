# EChamado API Server - Multi-stage Dockerfile (.NET 10)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj files first for layer cache optimization
COPY EChamado.Shared/EChamado.Shared.csproj EChamado.Shared/
COPY Server/EChamado.Server.Domain/EChamado.Server.Domain.csproj Server/EChamado.Server.Domain/
COPY Server/EChamado.Server.Application/EChamado.Server.Application.csproj Server/EChamado.Server.Application/
COPY Server/EChamado.Server.Infrastructure/EChamado.Server.Infrastructure.csproj Server/EChamado.Server.Infrastructure/
COPY Server/EChamado.Server/EChamado.Server.csproj Server/EChamado.Server/
RUN dotnet restore Server/EChamado.Server/EChamado.Server.csproj

# Copy source and build
COPY . .
RUN dotnet publish Server/EChamado.Server/EChamado.Server.csproj -c Release -o /app/publish --no-restore

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

RUN groupadd -g 1000 appgroup && useradd -u 1000 -G appgroup appuser && chown -R appuser:appgroup /app
USER appuser

EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=10s --start-period=15s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "EChamado.Server.dll"]
