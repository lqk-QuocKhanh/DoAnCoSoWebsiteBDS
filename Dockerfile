# ── Stage 1: Build ─────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY VietPropEstate.sln ./
COPY src/ src/
COPY tests/ tests/

RUN dotnet restore VietPropEstate.sln

RUN dotnet publish src/VietPropEstate.WebAPI/VietPropEstate.WebAPI.csproj \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

# ── Stage 2: Runtime ───────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

RUN mkdir -p logs && \
    adduser --disabled-password --gecos "" appuser && \
    chown -R appuser:appuser /app

COPY --from=build --chown=appuser:appuser /app/publish .
COPY --chown=appuser:appuser docker/entrypoint.sh /app/entrypoint.sh

RUN chmod +x /app/entrypoint.sh

USER appuser

ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

ENTRYPOINT ["/app/entrypoint.sh"]
