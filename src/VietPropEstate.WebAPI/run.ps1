# Starts WebAPI after freeing port 5198 (avoids MSB3027 DLL lock from stale instances).
$ErrorActionPreference = "Stop"
$repoRoot = Resolve-Path (Join-Path $PSScriptRoot "..\..")
& "$repoRoot\scripts\run-webapi.ps1"
