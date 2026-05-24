/*
================================================================================
  VietPropEstate — Truy vấn LocalDB Development (VietPropEstateDb_Dev)
================================================================================
  Chạy trên: (localdb)\mssqllocaldb
  Database:  VietPropEstateDb_Dev
================================================================================
*/

USE [VietPropEstateDb_Dev];
GO

-- ── Kiểm tra đang kết nối đúng server/database ──
SELECT @@SERVERNAME AS ServerName, DB_NAME() AS DatabaseName;
GO

-- ── Tài khoản demo (mật khẩu do WebAPI seed) ──
-- admin@vietpropestate.vn           / Admin@123   (Admin)
-- broker@test.vietpropestate.vn     / Test@123    (Broker)
-- customer@test.vietpropestate.vn   / Test@123    (Customer)
-- staff@test.vietpropestate.vn      / Test@123    (Staff)

-- ── Tất cả user + role (đăng ký web lưu tại AspNetUsers) ──
SELECT
    u.Id,
    u.Email,
    u.FirstName,
    u.LastName,
    u.PhoneNumber,
    u.EmailConfirmed,
    u.IsActive,
    u.CreatedAt,
    u.LastLoginAt,
    r.Name AS Role
FROM AspNetUsers u
LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
ORDER BY u.CreatedAt DESC;
GO

-- ── User đăng ký qua web (không phải tài khoản seed) ──
SELECT u.Id, u.Email, u.FirstName, u.LastName, u.PhoneNumber, u.CreatedAt
FROM AspNetUsers u
WHERE u.Email NOT LIKE N'%test.vietpropestate.vn'
  AND u.Email <> N'admin@vietpropestate.vn'
ORDER BY u.CreatedAt DESC;
GO

-- ── Thống kê bảng liên quan tài khoản ──
SELECT N'AspNetUsers'     AS [Table], COUNT(*) AS [Count] FROM AspNetUsers
UNION ALL SELECT N'AspNetRoles',      COUNT(*) FROM AspNetRoles
UNION ALL SELECT N'AspNetUserRoles',  COUNT(*) FROM AspNetUserRoles
UNION ALL SELECT N'RefreshTokens',    COUNT(*) FROM RefreshTokens
UNION ALL SELECT N'Agents',           COUNT(*) FROM Agents
UNION ALL SELECT N'Customers',        COUNT(*) FROM Customers;
GO

-- ── Refresh token gần nhất ──
SELECT TOP 20
    u.Email,
    rt.CreatedAt,
    rt.ExpiresAt,
    rt.IsRevoked,
    rt.CreatedByIp
FROM RefreshTokens rt
INNER JOIN AspNetUsers u ON rt.UserId = u.Id
ORDER BY rt.CreatedAt DESC;
GO

-- ── Tin đăng theo tỉnh/thành (Active) ──
SELECT p.ProvinceName, COUNT(*) AS ListingCount
FROM Properties p
WHERE p.IsDeleted = 0 AND p.Status = 1
GROUP BY p.ProvinceName
ORDER BY ListingCount DESC;
GO

-- ── Tin chờ duyệt (Draft) ──
SELECT Id, Title, ProvinceName, CreatedAt
FROM Properties
WHERE IsDeleted = 0 AND Status = 0
ORDER BY CreatedAt;
GO

-- ── Địa chỉ hành chính VN ──
SELECT
    (SELECT COUNT(*) FROM Provinces WHERE IsActive = 1) AS ActiveProvinces,
    (SELECT COUNT(*) FROM Wards WHERE IsActive = 1) AS ActiveWards;
GO
