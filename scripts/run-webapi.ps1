$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
$apiUrl = "http://localhost:5198"

try {
    $resp = Invoke-WebRequest -Uri "$apiUrl/health" -UseBasicParsing -TimeoutSec 2
    if ($resp.StatusCode -eq 200) {
        Write-Host "WebAPI is already running: $apiUrl"
        Write-Host "Swagger UI: $apiUrl/swagger"
        exit 0
    }
}
catch {
    # Not running — continue to start
}

& "$PSScriptRoot\stop-dev-ports.ps1" -Ports @(5198)

Write-Host "Starting WebAPI on $apiUrl ..."
Write-Host "Swagger UI: $apiUrl/swagger"
dotnet run --project "$root\src\VietPropEstate.WebAPI"
