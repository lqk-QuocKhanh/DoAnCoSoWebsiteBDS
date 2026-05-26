# Deploy — VietPropEstate (Local + Render)

## Architecture

| Layer | Local | Render |
|-------|-------|--------|
| Blazor UI | http://localhost:5126 | https://doancosowebsitebds-ui.onrender.com |
| Web API | http://localhost:5198 | https://doancosowebsitebds-api.onrender.com |
| Database | In-memory (dev) or Docker Postgres | Render PostgreSQL |

## Configuration

### Blazor UI — `ApiSettings:BaseUrl`

| File | Purpose |
|------|---------|
| `wwwroot/appsettings.json` | Default (local API) |
| `wwwroot/appsettings.Development.json` | Local dev override |
| `wwwroot/appsettings.Production.json` | Production build fallback |
| Docker `entrypoint.sh` | Injects runtime URL at container start |

### Web API

| Setting | Local | Render env var |
|---------|-------|----------------|
| CORS | `Cors:AllowedOrigins` | `Cors__AllowedOrigins__0` |
| DB | `UseInMemoryDatabase: true` | `ConnectionStrings__DefaultConnection` or `DATABASE_URL` |
| JWT | appsettings.json | `JwtSettings__Secret` |

## Local development

```powershell
cd src\VietPropEstate.WebAPI && dotnet run
cd src\VietPropEstate.BlazorUI && dotnet run
```

Admin: `admin@vietpropestate.vn` / `Admin@123`

## Render deployment

1. PostgreSQL on Render → link `DATABASE_URL` to API.
2. API from `Dockerfile.api`, health: `/health`.
3. UI from `Dockerfile`, env: `API_BASE_URL=https://doancosowebsitebds-api.onrender.com`
4. API env: `Cors__AllowedOrigins__0=https://doancosowebsitebds-ui.onrender.com`

Or apply `render.yaml` blueprint.
