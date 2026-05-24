#!/bin/sh
set -e

PORT="${PORT:-10000}"
export ASPNETCORE_URLS="http://0.0.0.0:${PORT}"

API_URL="${API_BASE_URL:-http://localhost:5198}"
API_URL=$(printf '%s' "$API_URL" | sed 's:/*$::')

sed "s/__PORT__/${PORT}/" /etc/nginx/conf.d/default.conf.template > /etc/nginx/conf.d/default.conf

cat > /usr/share/nginx/html/appsettings.json <<EOF
{
  "ApiBaseUrl": "${API_URL}"
}
EOF

echo "Blazor UI listening on ${ASPNETCORE_URLS}, ApiBaseUrl=${API_URL}"
exec nginx -g 'daemon off;'
