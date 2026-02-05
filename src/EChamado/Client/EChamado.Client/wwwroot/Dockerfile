# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy entire source
COPY . .

# Restore and publish
RUN dotnet restore Client/EChamado.Client/EChamado.Client.csproj && \
    dotnet publish Client/EChamado.Client/EChamado.Client.csproj -c Release -o /app/publish --no-restore

# Runtime stage
FROM nginx:alpine
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
