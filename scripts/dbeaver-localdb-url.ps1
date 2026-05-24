# Prints (and copies) the DBeaver JDBC URL for the current LocalDB instance.
$instance = "mssqllocaldb"
$database = "VietPropEstateDb_Dev"

$info = sqllocaldb info $instance 2>&1 | Out-String
if ($LASTEXITCODE -ne 0) {
    Write-Error "LocalDB instance '$instance' not found. Install SQL Server LocalDB or run: sqllocaldb create $instance"
    exit 1
}

$pipeLine = ($info -split "`n" | Where-Object { $_ -match "Instance pipe name:" }) -replace ".*Instance pipe name:\s*", ""
$pipeLine = $pipeLine.Trim()

if ([string]::IsNullOrWhiteSpace($pipeLine)) {
    Write-Error "Could not read LocalDB pipe name."
    exit 1
}

# np:\\.\pipe\LOCALDB#....\tsql\query  ->  \\.\pipe\LOCALDB#....\tsql\query
$pipe = $pipeLine -replace "^np:", ""

# JDBC: double backslashes for the URL string
$pipeForJdbc = $pipe -replace '\\', '\\\\'
$jdbcUrl = "jdbc:sqlserver://np:$pipeForJdbc;databaseName=$database;integratedSecurity=true;encrypt=false;trustServerCertificate=true;"

Write-Host ""
Write-Host "=== DBeaver -> SQL Server (LocalDB) ===" -ForegroundColor Cyan
Write-Host "Database : $database"
Write-Host "Pipe     : $pipe"
Write-Host ""
Write-Host "JDBC URL (paste in DBeaver -> URL tab):" -ForegroundColor Yellow
Write-Host $jdbcUrl
Write-Host ""

try {
    Set-Clipboard -Value $jdbcUrl
    Write-Host "URL copied to clipboard." -ForegroundColor Green
} catch {
    Write-Host "Could not copy to clipboard automatically." -ForegroundColor DarkYellow
}

Write-Host @"

Steps in DBeaver:
  1. Database -> New Database Connection -> SQL Server -> Next
  2. Main tab -> switch "Connect by" to URL (not Host)
  3. Paste the JDBC URL above
  4. Authentication: leave empty OR Windows / Integrated (no sa password)
  5. Driver properties: integratedSecurity = true (if connection fails)
  6. Check "Trust Server Certificate"
  7. Test Connection -> Finish

If Windows auth fails in DBeaver:
  - Driver settings -> Add file mssql-jdbc_auth-*.dll (x64) to native library path
  - Or use SSMS with server: (localdb)\MSSQLLocalDB

Quick query after connect:
  SELECT TOP 10 Id, Title, Status, CreatedAt FROM Properties ORDER BY CreatedAt DESC;
"@
