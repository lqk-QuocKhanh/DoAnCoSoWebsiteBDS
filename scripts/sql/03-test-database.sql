-- ============================================================
-- VietPropEstate — Test Database Guide
-- ============================================================
-- Schema is created by EF migrations. Demo data is seeded by
-- TestDataSeeder when TestSeed:Enabled = true (Development).
--
-- OPTION A — Use existing dev DB (recommended)
--   1. Set TestSeed:Enabled = true in appsettings.Development.json
--   2. dotnet run --project src/VietPropEstate.WebAPI
--   3. Seeder runs once (slug prefix "test-")
--
-- OPTION B — Separate test database
--   1. CREATE DATABASE VietPropEstateDb_Test;
--   2. Update DefaultConnection in appsettings.Development.json:
--      Server=(localdb)\mssqllocaldb;Database=VietPropEstateDb_Test;Trusted_Connection=True;...
--   3. dotnet ef database update --project src/VietPropEstate.Infrastructure --startup-project src/VietPropEstate.WebAPI
--   4. Start WebAPI (migrations + seed run automatically)
-- ============================================================

USE [VietPropEstateDb_Dev];
GO

-- ── Test accounts (password via Identity — see TestDataSeeder) ──
-- admin@vietpropestate.vn  / Admin@123   (Admin)
-- broker@test.vietpropestate.vn  / Test@123   (Broker)
-- customer@test.vietpropestate.vn / Test@123  (Customer)
-- staff@test.vietpropestate.vn   / Test@123   (Staff)

-- ── Verify seeded data ──
SELECT Status, COUNT(*) AS [Count]
FROM Properties
WHERE IsDeleted = 0
  AND Title LIKE N'%Vinhomes Central Park%'
   OR Title LIKE N'%(Draft)%'
   OR Title LIKE N'%đã bán%'
   OR Title LIKE N'%đã cho thuê%'
GROUP BY Status
ORDER BY Status;

SELECT p.Title, p.Status, p.IsFeatured, p.ViewCount, pt.Name AS PropertyType, a.FullName AS Agent
FROM Properties p
JOIN PropertyTypes pt ON p.PropertyTypeId = pt.Id
JOIN Agents a ON p.AgentId = a.Id
WHERE p.IsDeleted = 0
  AND (p.Title LIKE N'%Vinhomes%' OR p.Title LIKE N'%(Draft)%' OR p.Title LIKE N'%Shophouse%')
ORDER BY p.CreatedAt;

SELECT u.Email, r.Name AS Role
FROM AspNetUsers u
JOIN AspNetUserRoles ur ON u.Id = ur.UserId
JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE u.Email LIKE '%test.vietpropestate.vn' OR u.Email = 'admin@vietpropestate.vn';

SELECT f.UserId, u.Email, p.Title
FROM Favorites f
JOIN AspNetUsers u ON f.UserId = u.Id
JOIN Properties p ON f.PropertyId = p.Id
WHERE p.Slug LIKE 'test-%';

-- ── Reset test properties only (re-seed on next API start) ──
/*
DELETE pi FROM PropertyImages pi
INNER JOIN Properties p ON pi.PropertyId = p.Id
WHERE p.Slug LIKE 'test-%';

DELETE f FROM Favorites f
INNER JOIN Properties p ON f.PropertyId = p.Id
WHERE p.Slug LIKE 'test-%';

DELETE pv FROM PropertyViews pv
INNER JOIN Properties p ON pv.PropertyId = p.Id
WHERE p.Slug LIKE 'test-%';

DELETE FROM Properties WHERE Slug LIKE 'test-%';
*/

-- ── Feature coverage matrix ──
-- | Feature              | Test with                          |
-- |----------------------|------------------------------------|
-- | Public listing       | Active properties (status=1)       |
-- | Featured homepage    | 2 tin IsFeatured=1                 |
-- | Search/filter        | HCM(79), HN(1), DN(48) provinces  |
-- | Property detail      | Any Active slug                    |
-- | Login / JWT          | All 4 test accounts                |
-- | Dashboard listings   | broker@test + my-listings          |
-- | Draft / Admin approve| Draft properties                   |
-- | Delete / Withdraw    | Withdrawn property                 |
-- | Favorites            | customer@test favorites            |
-- | Sold / Rented        | Status 3, 4                        |
-- | Address dropdown     | Provinces + Wards (AddressSeeder)  |
-- | Property types       | Apartment, House, Villa, Land...   |

GO
