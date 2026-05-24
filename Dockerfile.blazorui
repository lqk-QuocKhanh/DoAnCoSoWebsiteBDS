# ── Stage 1: Build Blazor WebAssembly ─────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY VietPropEstate.sln ./
COPY src/VietPropEstate.Domain/      src/VietPropEstate.Domain/
COPY src/VietPropEstate.Application/ src/VietPropEstate.Application/
COPY src/VietPropEstate.BlazorUI/    src/VietPropEstate.BlazorUI/

RUN dotnet restore src/VietPropEstate.BlazorUI/VietPropEstate.BlazorUI.csproj

RUN dotnet publish src/VietPropEstate.BlazorUI/VietPropEstate.BlazorUI.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ── Stage 2: Serve Blazor static files (nginx) ────────────────────────────────
FROM nginx:1.27-alpine AS runtime

COPY docker/blazorui/nginx.conf.template /etc/nginx/conf.d/default.conf.template
COPY docker/entrypoint.sh /entrypoint.sh
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html

RUN chmod +x /entrypoint.sh && \
    rm -f /etc/nginx/conf.d/default.conf

ENV PORT=10000
ENV ASPNETCORE_URLS=http://0.0.0.0:10000

EXPOSE 10000

ENTRYPOINT ["/entrypoint.sh"]
