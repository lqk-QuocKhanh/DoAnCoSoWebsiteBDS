/*
================================================================================
  VietPropEstate — Bootstrap SQL Server LocalDB (Development)
================================================================================

  SSMS / Azure Data Studio — Connect to:
    Server name:  (localdb)\mssqllocaldb
    Auth:         Windows Authentication

  sqlcmd (PowerShell):
    sqlcmd -S "(localdb)\mssqllocaldb" -i "scripts\sql\localdb\01-bootstrap-localdb-dev.sql"

  Connection string (appsettings.Development.json):
    Server=(localdb)\mssqllocaldb;Database=VietPropEstateDb_Dev;Trusted_Connection=True;MultipleActiveResultSets=true

  LƯU Ý: Database VietPropEstateDb_Dev chỉ tồn tại trên LocalDB.
         Không nhầm với database "VietPropEstate" trên server "localhost".

  Sau khi tạo database, chọn MỘT trong hai cách tạo schema:
    A) EF migrations (khuyên dùng — tự seed admin + demo data khi chạy WebAPI):
         dotnet ef database update ^
           --project src\VietPropEstate.Infrastructure ^
           --startup-project src\VietPropEstate.WebAPI

       dotnet run --project src\VietPropEstate.WebAPI

    B) SQL script schema đầy đủ:
         sqlcmd -S "(localdb)\mssqllocaldb" -d VietPropEstateDb_Dev ^
           -i "scripts\sql\04-full-schema-idempotent.sql"

       Sau đó chạy WebAPI để RoleSeeder + TestDataSeeder tạo tài khoản demo.

  Truy vấn kiểm tra: scripts\sql\localdb\02-queries-localdb-dev.sql
  Thiết kế DB:       scripts\sql\00-vietpropestate-database-design.sql
================================================================================
*/

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'VietPropEstateDb_Dev')
BEGIN
    CREATE DATABASE [VietPropEstateDb_Dev];
    PRINT N'Created database [VietPropEstateDb_Dev] on (localdb)\mssqllocaldb';
END
ELSE
BEGIN
    PRINT N'Database [VietPropEstateDb_Dev] already exists.';
END
GO

USE [VietPropEstateDb_Dev];
GO

SELECT
    @@SERVERNAME  AS ServerName,
    DB_NAME()     AS DatabaseName,
    GETUTCDATE()  AS CheckedAtUtc;
GO

PRINT N'';
PRINT N'Next: run EF migrations or scripts/sql/04-full-schema-idempotent.sql';
PRINT N'Then: scripts/sql/localdb/02-queries-localdb-dev.sql to verify users & data';
GO
