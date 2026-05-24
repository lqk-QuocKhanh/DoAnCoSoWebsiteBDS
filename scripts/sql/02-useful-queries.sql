-- VietPropEstate — useful operational queries
USE [VietPropEstateDb];
GO

-- Active property listings by province
SELECT p.ProvinceName, COUNT(*) AS ListingCount
FROM Properties p
WHERE p.IsDeleted = 0 AND p.Status = 1 -- Active
GROUP BY p.ProvinceName
ORDER BY ListingCount DESC;

-- Pending property approvals (Draft status)
SELECT Id, Title, ProvinceName, CreatedAt
FROM Properties
WHERE IsDeleted = 0 AND Status = 0 -- Draft
ORDER BY CreatedAt;

-- Recent completed payments
SELECT TOP 20
    pm.Id,
    pm.PaymentReference,
    pm.Amount,
    pm.Status,
    pm.CompletedAt,
    pm.VnpayTransactionId
FROM Payments pm
WHERE pm.IsDeleted = 0 AND pm.Status = 2 -- Completed
ORDER BY pm.CompletedAt DESC;

-- VIP package subscriptions expiring within 7 days
SELECT uvp.Id, uvp.UserId, vp.Name AS PackageName, uvp.ExpiresAt, uvp.RemainingListings
FROM UserVIPPackages uvp
INNER JOIN VIPPackages vp ON vp.Id = uvp.VIPPackageId
WHERE uvp.IsActive = 1
  AND uvp.ExpiresAt <= DATEADD(day, 7, GETUTCDATE())
ORDER BY uvp.ExpiresAt;

-- Vietnam address reference counts
SELECT
    (SELECT COUNT(*) FROM Provinces WHERE IsActive = 1) AS ActiveProvinces,
    (SELECT COUNT(*) FROM Wards WHERE IsActive = 1) AS ActiveWards;
