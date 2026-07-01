#!/usr/bin/env bash
set -euo pipefail

# ─── Configuration ────────────────────────────────────────────────────────────
# Update these values to match your existing Azure resources.
RESOURCE_GROUP_NAME="rg-cars-mcp"        # existing resource group name
APP_SERVICE_NAME="cars-mcp-app"          # existing App Service (Web App) name
PROJECT_PATH="."                         # path to Cars.Mcp.csproj (relative to this script)
PUBLISH_DIR="./publish"                  # local folder used for the dotnet publish output
ZIP_PATH="./publish.zip"                 # zip archive used for the deployment
DOTNET_CONFIGURATION="Release"           # build configuration
# ──────────────────────────────────────────────────────────────────────────────

echo "==> Checking Azure CLI login..."
az account show >/dev/null 2>&1 || az login

echo "==> Cleaning previous publish artifacts..."
rm -rf "$PUBLISH_DIR" "$ZIP_PATH"

echo "==> Publishing Cars.Mcp ($DOTNET_CONFIGURATION)..."
dotnet publish "$PROJECT_PATH" \
  --configuration "$DOTNET_CONFIGURATION" \
  --output "$PUBLISH_DIR"

echo "==> Creating deployment zip..."
(cd "$PUBLISH_DIR" && zip -r -q "../$(basename "$ZIP_PATH")" .)

echo "==> Deploying to App Service: $APP_SERVICE_NAME"
az webapp deploy \
  --resource-group "$RESOURCE_GROUP_NAME" \
  --name "$APP_SERVICE_NAME" \
  --src-path "$ZIP_PATH" \
  --type zip

echo "==> Deploy complete! Your app is available at:"
az webapp show \
  --resource-group "$RESOURCE_GROUP_NAME" \
  --name "$APP_SERVICE_NAME" \
  --query "defaultHostName" \
  --output tsv
