/*
================================================================================
  VietPropEstate — Cài schema đầy đủ lên LocalDB (VietPropEstateDb_Dev)
================================================================================

  Bước 1 — Tạo database (nếu chưa có):
    sqlcmd -S "(localdb)\mssqllocaldb" -i "scripts\sql\localdb\01-bootstrap-localdb-dev.sql"

  Bước 2 — Chạy file này (SQLCMD mode — SSMS: Query > SQLCMD Mode):

    :setvar ScriptDir "C:\Dev\VietPropEstate\scripts\sql"
    :r $(ScriptDir)\04-full-schema-idempotent.sql

  Hoặc PowerShell (không cần SQLCMD mode):
    sqlcmd -S "(localdb)\mssqllocaldb" -d VietPropEstateDb_Dev -i "scripts\sql\04-full-schema-idempotent.sql"

  Bước 3 — Seed dữ liệu demo + admin:
    dotnet run --project src\VietPropEstate.WebAPI
    (TestSeed:Enabled = true trong appsettings.Development.json)

  Bước 4 — Kiểm tra:
    sqlcmd -S "(localdb)\mssqllocaldb" -d VietPropEstateDb_Dev -i "scripts\sql\localdb\02-queries-localdb-dev.sql"
================================================================================
*/

USE [VietPropEstateDb_Dev];
GO

PRINT N'Installing full schema from 04-full-schema-idempotent.sql ...';
GO

-- Chạy bằng sqlcmd từ thư mục gốc repo:
--   sqlcmd -S "(localdb)\mssqllocaldb" -d VietPropEstateDb_Dev -i "scripts\sql\04-full-schema-idempotent.sql"
