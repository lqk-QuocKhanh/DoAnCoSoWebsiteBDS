#!/bin/sh
set -e

# Render injects PORT at runtime; default to 8080 for local Docker Compose.
export ASPNETCORE_URLS="http://0.0.0.0:${PORT:-8080}"

exec dotnet VietPropEstate.WebAPI.dll
