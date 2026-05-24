# ── Stage 1: Build (production WebAPI only — no tests / BlazorUI) ─────────────
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files first for layer-cached restore (WebAPI dependency graph only)
COPY src/VietPropEstate.Domain/VietPropEstate.Domain.csproj                         src/VietPropEstate.Domain/
COPY src/VietPropEstate.Application/VietPropEstate.Application.csproj               src/VietPropEstate.Application/
COPY src/VietPropEstate.Infrastructure/VietPropEstate.Infrastructure.csproj         src/VietPropEstate.Infrastructure/
COPY src/VietPropEstate.WebAPI/VietPropEstate.WebAPI.csproj                         src/VietPropEstate.WebAPI/

RUN dotnet restore src/VietPropEstate.WebAPI/VietPropEstate.WebAPI.csproj

# Copy source for publish
COPY src/VietPropEstate.Domain/         src/VietPropEstate.Domain/
COPY src/VietPropEstate.Application/    src/VietPropEstate.Application/
COPY src/VietPropEstate.Infrastructure/ src/VietPropEstate.Infrastructure/
COPY src/VietPropEstate.WebAPI/         src/VietPropEstate.WebAPI/

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
