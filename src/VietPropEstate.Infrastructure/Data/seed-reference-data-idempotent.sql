-- VietPropEstate — idempotent reference data (PostgreSQL)
-- Safe to run multiple times on an existing database.
-- Categories = PropertyTypes | Districts = Wards (loaded via app AddressSeed or vietnam-addresses-v2.json)

-- ── PropertyTypes (Categories) ─────────────────────────────────────────────
INSERT INTO "PropertyTypes" ("Id", "CreatedAt", "Description", "IsActive", "IsDeleted", "Name", "RowVersion")
VALUES
    ('11111111-1111-1111-1111-111111111111', TIMESTAMPTZ '2024-01-01T00:00:00Z', 'Detached or semi-detached house', TRUE, FALSE, 'House', BYTEA E'\\x'),
    ('22222222-2222-2222-2222-222222222222', TIMESTAMPTZ '2024-01-01T00:00:00Z', 'Commercial office space', TRUE, FALSE, 'Office', BYTEA E'\\x'),
    ('33333333-3333-3333-3333-333333333333', TIMESTAMPTZ '2024-01-01T00:00:00Z', 'Motel / mini-hotel / boarding house', TRUE, FALSE, 'Motel', BYTEA E'\\x'),
    ('a1b2c3d4-e5f6-7890-abcd-ef1234567890', TIMESTAMPTZ '2024-01-01T00:00:00Z', 'Condominium / Apartment unit', TRUE, FALSE, 'Apartment', BYTEA E'\\x'),
    ('b2c3d4e5-f6a7-8901-bcde-f12345678901', TIMESTAMPTZ '2024-01-01T00:00:00Z', 'Standalone villa with garden', TRUE, FALSE, 'Villa', BYTEA E'\\x'),
    ('d4e5f6a7-b8c9-0123-def0-234567890123', TIMESTAMPTZ '2024-01-01T00:00:00Z', 'Land plot / undeveloped lot', TRUE, FALSE, 'Land', BYTEA E'\\x')
ON CONFLICT ("Id") DO NOTHING;

-- ── TransactionTypes ───────────────────────────────────────────────────────
INSERT INTO "TransactionTypes" ("Id", "CreatedAt", "Description", "IsActive", "IsDeleted", "Name", "RowVersion")
VALUES
    ('44444444-4444-4444-4444-444444444444', TIMESTAMPTZ '2024-01-01T00:00:00Z', 'Property is listed for sale', TRUE, FALSE, 'Sell', BYTEA E'\\x'),
    ('55555555-5555-5555-5555-555555555555', TIMESTAMPTZ '2024-01-01T00:00:00Z', 'Property is listed for rent', TRUE, FALSE, 'Rent', BYTEA E'\\x')
ON CONFLICT ("Id") DO NOTHING;

-- ── VIP Packages ───────────────────────────────────────────────────────────
INSERT INTO "VIPPackages" ("Id", "CreatedAt", "Description", "DurationDays", "IsActive", "IsDeleted", "MaxListings", "Name", "RowVersion", "PriceAmount", "PriceCurrency")
VALUES
    ('60000000-0000-0000-0000-000000000001', TIMESTAMPTZ '2024-01-01T00:00:00Z', 'Đăng tối đa 3 tin rao trong 30 ngày. Phù hợp cho cá nhân.', 30, TRUE, FALSE, 3, 'Cơ Bản', BYTEA E'\\x', 500000.0, 'VND'),
    ('60000000-0000-0000-0000-000000000002', TIMESTAMPTZ '2024-01-01T00:00:00Z', 'Đăng tối đa 10 tin rao trong 30 ngày. Phù hợp cho môi giới cá nhân.', 30, TRUE, FALSE, 10, 'Tiêu Chuẩn', BYTEA E'\\x', 1000000.0, 'VND'),
    ('60000000-0000-0000-0000-000000000003', TIMESTAMPTZ '2024-01-01T00:00:00Z', 'Đăng tối đa 30 tin rao trong 90 ngày. Ưu tiên hiển thị trên trang chủ.', 90, TRUE, FALSE, 30, 'Cao Cấp', BYTEA E'\\x', 2500000.0, 'VND'),
    ('60000000-0000-0000-0000-000000000004', TIMESTAMPTZ '2024-01-01T00:00:00Z', 'Không giới hạn tin rao trong 180 ngày. Được gắn nhãn VIP nổi bật.', 180, TRUE, FALSE, 99, 'Gold', BYTEA E'\\x', 5000000.0, 'VND')
ON CONFLICT ("Id") DO NOTHING;

-- ── EF migration marker (if schema applied manually) ───────────────────────
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260524210825_InitialPostgresIdentity', '9.0.16')
ON CONFLICT ("MigrationId") DO NOTHING;
