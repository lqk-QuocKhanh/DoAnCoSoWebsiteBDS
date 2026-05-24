#!/bin/sh
set -e

PORT="${PORT:-10000}"
export ASPNETCORE_URLS="http://0.0.0.0:${PORT}"

exec dotnet VietPropEstate.WebAPI.dll
