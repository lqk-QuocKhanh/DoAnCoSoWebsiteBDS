-- VietPropEstate — SQL Server bootstrap script
-- Run this before first launch if you prefer manual database setup over EF migrations.

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'VietPropEstateDb')
BEGIN
    CREATE DATABASE [VietPropEstateDb];
END
GO

USE [VietPropEstateDb];
GO

-- Optional: create a dedicated application login (adjust password for production)
/*
CREATE LOGIN [VietPropApp] WITH PASSWORD = N'ChangeMe_Strong_Password_2024!';
GO

IF NOT EXISTS (SELECT name FROM sys.database_principals WHERE name = N'VietPropApp')
BEGIN
    CREATE USER [VietPropApp] FOR LOGIN [VietPropApp];
    ALTER ROLE [db_owner] ADD MEMBER [VietPropApp];
END
GO
*/

PRINT 'Database VietPropEstateDb is ready.';
PRINT '';
PRINT 'Next steps (choose one):';
PRINT '  A) SQL script:  scripts/sql/04-full-schema-idempotent.sql';
PRINT '  B) EF migrate:  dotnet ef database update --project src/VietPropEstate.Infrastructure --startup-project src/VietPropEstate.WebAPI';
PRINT '  C) Design doc:  scripts/sql/00-vietpropestate-database-design.sql';
PRINT '  D) Test verify: scripts/sql/03-test-database.sql';
GO
