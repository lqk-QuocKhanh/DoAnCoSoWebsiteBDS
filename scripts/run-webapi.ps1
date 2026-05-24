$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot

& "$PSScriptRoot\stop-dev-ports.ps1" -Ports @(5198)

Write-Host "Starting WebAPI on http://localhost:5198 ..."
Write-Host "Swagger UI: http://localhost:5198/swagger"
dotnet run --project "$root\src\VietPropEstate.WebAPI"
