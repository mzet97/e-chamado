# EChamado Blazor WASM Client - Multi-stage Dockerfile (.NET 10 + nginx)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj files first for layer cache optimization
COPY EChamado.Shared/EChamado.Shared.csproj EChamado.Shared/
COPY Client/EChamado.Client.Application/EChamado.Client.Application.csproj Client/EChamado.Client.Application/
COPY Client/EChamado.Client/EChamado.Client.csproj Client/EChamado.Client/
RUN dotnet restore Client/EChamado.Client/EChamado.Client.csproj

# Copy source and publish
COPY . .
RUN dotnet publish Client/EChamado.Client/EChamado.Client.csproj -c Release -o /app/publish --no-restore

# Runtime with nginx for static file serving
FROM nginx:alpine AS runtime

# Copy published WASM output
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html

# Custom nginx config for Blazor WASM SPA routing
RUN cat > /etc/nginx/conf.d/default.conf << 'EOF'
server {
    listen 80;
    server_name _;
    root /usr/share/nginx/html;
    index index.html;

    # Blazor WASM SPA fallback - all routes to index.html
    location / {
        try_files $uri $uri/ /index.html;
    }

    # Cache static assets
    location /_content/ {
        expires 1y;
        add_header Cache-Control "public, immutable";
    }

    # Health check
    location /health {
        access_log off;
        return 200 '{"status":"ok"}';
        add_header Content-Type application/json;
    }

    # Security headers
    add_header X-Frame-Options "SAMEORIGIN" always;
    add_header X-Content-Type-Options "nosniff" always;
}
EOF

EXPOSE 80

HEALTHCHECK --interval=30s --timeout=5s --start-period=5s --retries=3 \
    CMD wget -qO- http://localhost/health || exit 1

CMD ["nginx", "-g", "daemon off;"]
