#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
ENV_FILE="${SCRIPT_DIR}/.env"

if [[ -f "${ENV_FILE}" ]]; then
  set -a
  source "${ENV_FILE}"
  set +a
fi

: "${FRONTEND_SOURCE_DIR:=../automatic-dispatch-frontend}"
: "${APP_SERVICE_SOURCE_DIR:=../cumlist-app-service-develop}"
: "${DATA_SERVICE_SOURCE_DIR:=../del-cumlist-data-service-develop}"
: "${NORMALIZE_SERVICE_SOURCE_DIR:=../cumlist-normalize-service-develop}"

echo "[1/4] Building frontend static artifacts"
pushd "${FRONTEND_SOURCE_DIR}" >/dev/null
pnpm install --frozen-lockfile
cp deploy/develop/env packages/apps/cumlist/.env.production
pnpm --filter app_cumlist run build
mkdir -p artifacts/apps_cumlist
rm -rf artifacts/apps_cumlist/*
cp -R packages/apps/cumlist/build/* artifacts/apps_cumlist/
docker build -t automatic-dispatch-frontend:lab -f Dockerfile .
popd >/dev/null

echo "[2/4] Building cumlist-app-service image"
docker build -t cumlist-app-service:lab   -f "${APP_SERVICE_SOURCE_DIR}/src/CumList.AppService/Dockerfile"   "${APP_SERVICE_SOURCE_DIR}/src"

echo "[3/4] Building cumlist-data-service image"
docker build -t cumlist-data-service:lab   -f "${DATA_SERVICE_SOURCE_DIR}/src/CumList.DataService/Dockerfile"   "${DATA_SERVICE_SOURCE_DIR}/src"

echo "[4/4] Building cumlist-normalize-service image"
docker build -t cumlist-normalize-service:lab   -f "${NORMALIZE_SERVICE_SOURCE_DIR}/src/CumList.NormalizeService/Dockerfile"   "${NORMALIZE_SERVICE_SOURCE_DIR}/src"

echo "Images built successfully:"
docker images | grep -E "automatic-dispatch-frontend|cumlist-(app|data|normalize)-service" || true
