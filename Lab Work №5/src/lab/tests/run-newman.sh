#!/usr/bin/env bash
set -euo pipefail
SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
cd "${SCRIPT_DIR}"
npm install >/dev/null 2>&1 || true
npx newman run "CumList Smoke Tests.postman_collection.json" -e "CumList Smoke Tests.postman_environment.json"
