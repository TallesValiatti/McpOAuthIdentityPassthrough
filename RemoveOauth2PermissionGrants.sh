#!/usr/bin/env bash
set -euo pipefail

TENANT_ID="<YOUR_TENANT_ID>"

# API protegida / resource app
# App Registration: custom-mcp-cars
RESOURCE_APP_ID="<YOUR_RESOURCE_APP_ID>"

# Aplicação cliente
# App Registration: custom-mcp-cars-client
CLIENT_APP_ID="<YOUR_CLIENT_APP_ID>"

REVOKE_SIGNIN_SESSIONS=false

echo "Autenticando no tenant..."
az login --tenant "$TENANT_ID" --only-show-errors >/dev/null

echo "Obtendo usuário logado..."
USER_ID=$(az ad signed-in-user show --query id -o tsv)

echo "Resolvendo Service Principal do client app..."
CLIENT_SP_ID=$(az ad sp show \
  --id "$CLIENT_APP_ID" \
  --query id \
  -o tsv)

echo "Resolvendo Service Principal da API/resource app..."
RESOURCE_SP_ID=$(az ad sp show \
  --id "$RESOURCE_APP_ID" \
  --query id \
  -o tsv)

echo ""
echo "Tenant ID:      $TENANT_ID"
echo "User ID:        $USER_ID"
echo "Client App ID:  $CLIENT_APP_ID"
echo "Client SP ID:   $CLIENT_SP_ID"
echo "Resource App ID:$RESOURCE_APP_ID"
echo "Resource SP ID: $RESOURCE_SP_ID"
echo ""

FILTER="clientId eq '$CLIENT_SP_ID' and principalId eq '$USER_ID' and resourceId eq '$RESOURCE_SP_ID'"

echo "Buscando consentimentos delegados do usuário..."

GRANT_IDS=$(az rest --method GET \
  --uri "https://graph.microsoft.com/v1.0/oauth2PermissionGrants?\$filter=$FILTER" \
  --query "value[?consentType=='Principal'].id" \
  -o tsv)

if [ -z "$GRANT_IDS" ]; then
  echo "Nenhum consentimento delegado do usuário encontrado."
else
  echo ""
  echo "Consentimentos encontrados:"
  az rest --method GET \
    --uri "https://graph.microsoft.com/v1.0/oauth2PermissionGrants?\$filter=$FILTER" \
    --query "value[?consentType=='Principal'].{grantId:id,consentType:consentType,scope:scope}" \
    -o table

  echo ""
  echo "Removendo consentimentos..."

  for GRANT_ID in $GRANT_IDS; do
    echo "Removendo grant: $GRANT_ID"

    az rest --method DELETE \
      --uri "https://graph.microsoft.com/v1.0/oauth2PermissionGrants/$GRANT_ID"
  done

  echo ""
  echo "Consentimento delegado removido com sucesso."
fi

echo ""
echo "Verificando se existe admin consent tenant-wide..."

ADMIN_GRANTS=$(az rest --method GET \
  --uri "https://graph.microsoft.com/v1.0/oauth2PermissionGrants?\$filter=clientId eq '$CLIENT_SP_ID' and resourceId eq '$RESOURCE_SP_ID'" \
  --query "value[?consentType=='AllPrincipals'].{grantId:id,consentType:consentType,scope:scope}" \
  -o table)

if [ -n "$ADMIN_GRANTS" ]; then
  echo "$ADMIN_GRANTS"
  echo ""
  echo "Atenção: existe consentimento administrativo AllPrincipals."
  echo "Esse script remove apenas o consentimento do usuário logado."
fi

if [ "$REVOKE_SIGNIN_SESSIONS" = true ]; then
  echo ""
  echo "Revogando sessões/refresh tokens do usuário logado..."

  az rest --method POST \
    --uri "https://graph.microsoft.com/v1.0/me/revokeSignInSessions"

  echo "Sessões revogadas. Pode haver atraso de alguns minutos."
fi