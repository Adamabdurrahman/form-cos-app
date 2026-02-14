#!/bin/bash
# ══════════════════════════════════════════════════
# Build frontend & copy ke backend/wwwroot
# Kemudian jalankan backend (serve API + SPA)
# ══════════════════════════════════════════════════
set -e

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
FRONTEND_DIR="$SCRIPT_DIR/frontend"
BACKEND_DIR="$SCRIPT_DIR/backend"
WWWROOT_DIR="$BACKEND_DIR/wwwroot"

echo "══════════════════════════════════════════════════"
echo "  1. Building frontend..."
echo "══════════════════════════════════════════════════"
cd "$FRONTEND_DIR"
npm run build

echo ""
echo "══════════════════════════════════════════════════"
echo "  2. Copying dist → backend/wwwroot..."
echo "══════════════════════════════════════════════════"
rm -rf "$WWWROOT_DIR"
cp -r "$FRONTEND_DIR/dist" "$WWWROOT_DIR"
echo "   Copied $(find "$WWWROOT_DIR" -type f | wc -l) files"

echo ""
echo "══════════════════════════════════════════════════"
echo "  3. Starting backend (API + SPA)..."
echo "══════════════════════════════════════════════════"
cd "$BACKEND_DIR"
echo "   URL: http://localhost:5131"
echo "   Press Ctrl+C to stop"
echo ""
dotnet run --urls "http://0.0.0.0:5131"
