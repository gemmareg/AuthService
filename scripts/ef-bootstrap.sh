#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT_DIR"

PROJECT="AuthService.Infrastructure/AuthService.Infrastructure.csproj"
STARTUP="AuthService.Host/AuthService.Host.csproj"
MIGRATIONS_DIR="AuthService.Infrastructure/Migrations"

echo "[EF] Checking migrations..."
if ! find "$MIGRATIONS_DIR" -maxdepth 1 -type f -name '*_*.cs' | grep -q .; then
  echo "[EF] No migrations found. Creating initial-migration..."
  dotnet ef migrations add initial-migration --project "$PROJECT" --startup-project "$STARTUP"
else
  echo "[EF] Existing migrations detected. Skipping migrations add."
fi

echo "[EF] Applying migrations (database update)..."
dotnet ef database update --project "$PROJECT" --startup-project "$STARTUP"

echo "[EF] Database is ready."
