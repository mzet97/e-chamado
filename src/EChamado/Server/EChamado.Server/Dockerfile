FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY src/EChamado/EChamado.sln ./src/EChamado/EChamado.sln
COPY src/EChamado/Server/EChamado.Server/EChamado.Server.csproj ./src/EChamado/Server/EChamado.Server/EChamado.Server.csproj
COPY src/EChamado/Server/EChamado.Server.Application/EChamado.Server.Application.csproj ./src/EChamado/Server/EChamado.Server.Application/EChamado.Server.Application.csproj
COPY src/EChamado/Server/EChamado.Server.Domain/EChamado.Server.Domain.csproj ./src/EChamado/Server/EChamado.Server.Domain/EChamado.Server.Domain.csproj
COPY src/EChamado/Server/EChamado.Server.Infrastructure/EChamado.Server.Infrastructure.csproj ./src/EChamado/Server/EChamado.Server.Infrastructure/EChamado.Server.Infrastructure.csproj
COPY src/EChamado/EChamado.Shared/EChamado.Shared.csproj ./src/EChamado/EChamado.Shared/EChamado.Shared.csproj

# Restore dependencies
RUN dotnet restore src/EChamado/EChamado.sln

# Copy source code
COPY src/EChamado/Server ./src/EChamado/Server
COPY src/EChamado/EChamado.Shared ./src/EChamado/EChamado.Shared

# Build the application
RUN dotnet publish src/EChamado/Server/EChamado.Server/EChamado.Server.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Install curl for health check
RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*

# Copy published files
COPY --from=build /app/publish .

# Create non-root user
RUN addgroup -g 1000 -S appgroup && \
    adduser -u 1000 -S appuser -G appgroup

# Set ownership
RUN chown -R appuser:appgroup /app

# Switch to non-root user
USER appuser

# Expose port
EXPOSE 5000

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:5000/health || exit 1

# Start the application
ENTRYPOINT ["dotnet", "EChamado.Server.dll"]
