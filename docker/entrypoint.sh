#!/bin/sh
set -e

PORT="${PORT:-10000}"
export ASPNETCORE_URLS="http://0.0.0.0:${PORT}"

API_URL="${API_BASE_URL:-${ApiSettings__BaseUrl:-${ApiBaseUrl:-}}}"
API_URL=$(printf '%s' "$API_URL" | sed 's:/*$::')

if [ -z "$API_URL" ]; then
  echo "ERROR: API URL not set. Configure API_BASE_URL or ApiSettings__BaseUrl on Render."
  exit 1
fi

sed "s/__PORT__/${PORT}/" /etc/nginx/conf.d/default.conf.template > /etc/nginx/conf.d/default.conf

write_appsettings() {
  target="$1"
  cat > "$target" <<EOF
{
  "ApiSettings": {
    "BaseUrl": "${API_URL}/"
  }
}
EOF
}

write_appsettings /usr/share/nginx/html/appsettings.json
write_appsettings /usr/share/nginx/html/appsettings.Production.json

echo "Blazor UI listening on ${ASPNETCORE_URLS}, ApiSettings:BaseUrl=${API_URL}/"
exec nginx -g 'daemon off;'
