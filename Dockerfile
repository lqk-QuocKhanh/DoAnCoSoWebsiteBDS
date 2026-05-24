# ── Stage 1: Build ──────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY VietPropEstate.sln ./
COPY src/VietPropEstate.Domain/VietPropEstate.Domain.csproj                         src/VietPropEstate.Domain/
COPY src/VietPropEstate.Application/VietPropEstate.Application.csproj               src/VietPropEstate.Application/
COPY src/VietPropEstate.Infrastructure/VietPropEstate.Infrastructure.csproj         src/VietPropEstate.Infrastructure/
COPY src/VietPropEstate.WebAPI/VietPropEstate.WebAPI.csproj                         src/VietPropEstate.WebAPI/
COPY src/VietPropEstate.BlazorUI/VietPropEstate.BlazorUI.csproj                     src/VietPropEstate.BlazorUI/

RUN dotnet restore

COPY src/ src/

RUN dotnet publish src/VietPropEstate.WebAPI/VietPropEstate.WebAPI.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ── Stage 2: Runtime ─────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

RUN mkdir -p logs && \
    adduser --disabled-password --gecos "" appuser && \
    chown -R appuser:appuser /app

USER appuser

COPY --from=build --chown=appuser:appuser /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "VietPropEstate.WebAPI.dll"]
