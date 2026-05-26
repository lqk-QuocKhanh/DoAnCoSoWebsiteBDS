# VietPropEstate — PostgreSQL SQL scripts

## Files

| File | Mô tả |
|------|--------|
| `full-database-postgres.sql` | **Schema đầy đủ** — 31 bảng, index, FK, dữ liệu lookup (PropertyTypes, TransactionTypes, VIPPackages) |
| `seed-reference-data-idempotent.sql` | Chèn lại lookup data an toàn (`ON CONFLICT DO NOTHING`) |

## Bảng chính

| Nhóm | Bảng |
|------|------|
| Identity | `AspNetUsers`, `AspNetRoles`, … |
| Categories | `PropertyTypes` |
| Địa chỉ VN | `Provinces`, `Wards` (phường/xã — thay cho quận/huyện cũ) |
| BĐS | `Properties`, `PropertyImages`, `Agents` |
| VIP / Thanh toán | `VIPPackages`, `UserVIPPackages`, `Payments` |
| Chat | `Conversations`, `Messages` |

## Cách dùng trên Render PostgreSQL

### Cách 1 — Khuyến nghị (tự động)

Deploy WebAPI với `ProductionSeed:Enabled=true`. Ứng dụng sẽ:

1. `Database.Migrate()`
2. Seed admin, provinces/wards, 60 tin BĐS nếu DB trống

### Cách 2 — Chạy SQL thủ công (DB trống)

```bash
psql "$DATABASE_URL" -f src/VietPropEstate.Infrastructure/Data/full-database-postgres.sql
```

### Cách 3 — Chỉ bổ sung lookup data

```bash
psql "$DATABASE_URL" -f src/VietPropEstate.Infrastructure/Data/seed-reference-data-idempotent.sql
```

## Regenerate từ EF (dev)

```powershell
$env:UseInMemoryDatabase = "false"
dotnet ef migrations script --idempotent `
  --project src/VietPropEstate.Infrastructure `
  --startup-project src/VietPropEstate.WebAPI `
  -o src/VietPropEstate.Infrastructure/Data/full-database-postgres.sql
```

## Admin mặc định (seed qua app)

- Email: `admin@vietpropestate.com`
- Password: `Admin@123`
