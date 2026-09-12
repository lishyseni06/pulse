# syntax=docker/dockerfile:1

# ---------------------------------------------------------------- Ndërtimi
# Restore-ja veçmas nga kopjimi i kodit që shtresa e paketave të rrijë në cache.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY pulse.csproj .
RUN dotnet restore pulse.csproj

COPY . .
RUN dotnet publish pulse.csproj \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

# ---------------------------------------------------------------- Ekzekutimi
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# curl-i i duhet vetëm HEALTHCHECK-ut më poshtë.
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=build /app/publish .

# Dosja e fotove të ngarkuara — pronësi te përdoruesi jo-root që volume-i
# i Docker-it të trashëgojë të drejtat e shkrimit kur mbushet herën e parë.
RUN mkdir -p /app/wwwroot/uploads && chown -R $APP_UID /app/wwwroot/uploads

ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_HTTP_PORTS=8080

EXPOSE 8080

# Nuk ekzekutohet si root.
USER $APP_UID

# start-period i gjatë sepse nisja e parë krijon & mbush bazën (DbSeeder).
HEALTHCHECK --interval=30s --timeout=5s --start-period=120s --retries=3 \
    CMD curl --fail --silent http://localhost:8080/ || exit 1

ENTRYPOINT ["dotnet", "pulse.dll"]
