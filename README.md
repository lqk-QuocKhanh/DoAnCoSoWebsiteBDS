# VietPropEstate

Vietnam real estate platform built with **.NET 9**, **Clean Architecture**, **EF Core**, **SQL Server**, **Redis**, **SignalR**, and **Blazor WebAssembly** (MudBlazor).

## Solution structure

| Project | Purpose |
|---------|---------|
| `VietPropEstate.Domain` | Entities, enums, value objects, repository interfaces |
| `VietPropEstate.Application` | CQRS (MediatR), validators, DTOs, AutoMapper profiles |
| `VietPropEstate.Infrastructure` | EF Core, Identity, Redis cache, VNPay, address seeding |
| `VietPropEstate.WebAPI` | REST API, JWT auth, SignalR chat hub, middleware |
| `VietPropEstate.BlazorUI` | Vietnamese UI (Blazor WASM + MudBlazor) |

## Features

- Property listings with search, filters, SEO slugs, favorites, view tracking
- Vietnam address data (Province/Ward, post-2025 structure)
- JWT authentication with refresh token rotation and role-based authorization
- Real-time chat (SignalR): typing, seen status, online users
- VNPay payment integration for VIP packages
- Admin dashboard: users, properties, payments, VIP packages, activity logs
- **Redis distributed caching** for lookup data (provinces, property types, VIP packages)
- **Serilog** structured logging (console + rolling file)
- **Swagger/OpenAPI** documentation
- **Global exception middleware** with RFC 7807-style JSON responses
- **Rate limiting** (100 requests/minute per user/IP by default)
- **Response compression** (Brotli + Gzip)

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- SQL Server (LocalDB, Express, or Docker)
- Redis (optional — falls back to in-memory cache)
- Node/npm not required (Blazor WASM)

## Quick start (local development)

### 1. Clone and restore

```bash
git clone <repository-url>
cd VietPropEstate
dotnet restore
```

### 2. Configure connection strings

Edit `src/VietPropEstate.WebAPI/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=VietPropEstateDb_Dev;Trusted_Connection=True;MultipleActiveResultSets=true",
    "Redis": "localhost:6379"
  }
}
```

Leave `Redis` empty to use in-memory cache during local dev.

### 3. Apply database migrations

Migrations run automatically on API startup, or apply manually:

```bash
dotnet ef database update --project src/VietPropEstate.Infrastructure --startup-project src/VietPropEstate.WebAPI
```

Optional manual DB creation: run `scripts/sql/01-create-database.sql` in SQL Server Management Studio.

### 4. Run the Web API

```bash
cd src/VietPropEstate.WebAPI
dotnet run
```

- API: `https://localhost:7124` / `http://localhost:5198`
- Swagger: `https://localhost:7124/swagger`

On first run the API will:
1. Apply pending migrations
2. Seed roles and default admin user
3. Seed Vietnam province/ward data (from API or local JSON fallback)

**Default admin credentials** (from `appsettings.json`):

| Field | Value |
|-------|-------|
| Email | `admin@vietpropestate.vn` |
| Password | `Admin@123` |

### 5. Run the Blazor UI

In a second terminal:

```bash
cd src/VietPropEstate.BlazorUI
dotnet run
```

- UI: `https://localhost:7121` / `http://localhost:5126`

The Blazor app reads the API URL from `wwwroot/appsettings.json`:

```json
{ "ApiBaseUrl": "https://localhost:7124" }
```

## Docker

Start SQL Server, Redis, and the API with Docker Compose:

```bash
docker compose up -d
```

| Service | URL / Port |
|---------|------------|
| API | `http://localhost:8080` |
| SQL Server | `localhost:1433` (SA password in `docker-compose.yml`) |
| Redis | `localhost:6379` |

Build the API image only:

```bash
docker build -t vietpropestate-api .
```

Environment variables (see `docker-compose.yml`):

- `ConnectionStrings__DefaultConnection`
- `ConnectionStrings__Redis`
- `JwtSettings__Secret`
- `Cors__AllowedOrigins__0`

## Configuration reference

### JWT (`appsettings.json`)

```json
"JwtSettings": {
  "Secret": "<min 32 characters>",
  "Issuer": "VietPropEstate.WebAPI",
  "Audience": "VietPropEstate.BlazorUI",
  "ExpiryMinutes": "60",
  "RefreshTokenExpiryDays": "7"
}
```

### CORS

Add Blazor UI origins to `Cors:AllowedOrigins` in the Web API settings.

### Rate limiting

```json
"RateLimiting": {
  "PermitLimit": 100,
  "WindowSeconds": 60
}
```

### Serilog

Logs are written to console and `logs/vietpropestate-*.log` (rolling daily, 30-day retention).

### VNPay (sandbox)

Configure `VnPay` section with your TMN code, hash secret, return URL, and IPN URL.

## API documentation

Swagger UI is available when `EnableSwagger` is `true` (default in Development).

Key endpoint groups:

| Route | Description |
|-------|-------------|
| `POST /api/auth/register` | Register user |
| `POST /api/auth/login` | Login (JWT + refresh cookie) |
| `GET /api/properties` | Search/list properties |
| `GET /api/properties/slug/{slug}` | Property detail by SEO slug |
| `GET /api/addresses/provinces` | Vietnam provinces (cached) |
| `GET /api/addresses/wards/{code}` | Wards by province (cached) |
| `GET /api/payments/vip-packages` | VIP packages (cached) |
| `/hubs/chat` | SignalR chat hub |

## SQL scripts

| File | Description |
|------|-------------|
| `scripts/sql/01-create-database.sql` | Create database manually |
| `scripts/sql/02-useful-queries.sql` | Operational reporting queries |

Schema is managed by EF Core migrations in `src/VietPropEstate.Infrastructure/Migrations/`.

## Architecture notes

- **CQRS** via MediatR with validation and performance logging behaviors
- **Repository + Unit of Work** for data access; `IPropertyRepository` registered in DI
- **Soft delete** global query filters on key entities
- **Optimistic concurrency** via `RowVersion` on auditable entities
- **Read queries** use `AsNoTracking()` and targeted `Include()` projections
- **Cache-aside** pattern via `ICacheService.GetOrSetAsync()` for static lookup data

## Build

```bash
dotnet build
dotnet test   # when test projects are added
```

## Troubleshooting

| Issue | Fix |
|-------|-----|
| Blazor cannot reach API | Match `ApiBaseUrl` in Blazor `appsettings.json` to Web API launch URL; verify CORS origins |
| CORS errors on login | Add Blazor origin to `Cors:AllowedOrigins` |
| Redis connection failed | Set empty Redis connection string to use in-memory cache |
| Address data empty | Ensure API started once to run `AddressDataSeeder`; check network access to provinces API |
| 429 Too Many Requests | Adjust `RateLimiting:PermitLimit` or wait for window reset |

## License

Proprietary — VietPropEstate platform.
