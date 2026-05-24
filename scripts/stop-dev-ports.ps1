# Frees local dev ports used by VietPropEstate (WebAPI: 5198, BlazorUI: 5126).
param(
    [int[]] $Ports = @(5198, 5126)
)

foreach ($port in $Ports) {
    $connections = Get-NetTCPConnection -LocalPort $port -State Listen -ErrorAction SilentlyContinue
    $pids = $connections | Select-Object -ExpandProperty OwningProcess -Unique

    foreach ($processId in $pids) {
        try {
            $proc = Get-Process -Id $processId -ErrorAction Stop
            Write-Host "Stopping PID $processId ($($proc.ProcessName)) on port $port..."
            Stop-Process -Id $processId -Force -ErrorAction Stop
        }
        catch {
            Write-Warning "Could not stop PID $processId on port ${port}: $_"
        }
    }

    if (-not $pids) {
        Write-Host "Port $port is free."
    }
}

$devProcesses = @('VietPropEstate.WebAPI', 'VietPropEstate.BlazorUI')
foreach ($name in $devProcesses) {
    Get-Process -Name $name -ErrorAction SilentlyContinue | ForEach-Object {
        Write-Host "Stopping leftover process $($_.ProcessName) (PID $($_.Id))..."
        Stop-Process -Id $_.Id -Force -ErrorAction SilentlyContinue
    }
}
