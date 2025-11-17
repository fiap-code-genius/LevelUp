#!/bin/bash

UNIQUE_SUFFIX=$(date +%s)

RESOURCE_GROUP="rg-levelup-api"
LOCATION="canadacentral"
DB_SERVER_NAME="sql-levelup-api-srv"
DB_NAME="levelup_db"
DB_ADMIN_USER="levelupadmin"
DB_ADMIN_PASSWORD="LevelUpP@ssw0rd2025!"
WEB_APP_NAME="app-levelup-api"
APP_SERVICE_PLAN="plan-levelup-api"
JWT_SECRET_KEY="SUA_CHAVE_SECRETA_LONGA_DE_PELO_MENOS_32_CARACTERES_AQUI"

echo "### Iniciando o deploy da API .NET 8 com Azure SQL ###"

echo "--> Criando Grupo de Recursos: $RESOURCE_GROUP..."
az group create --name $RESOURCE_GROUP --location $LOCATION

echo "--> Criando Servidor SQL Lógico: $DB_SERVER_NAME..."
az sql server create \
    --name $DB_SERVER_NAME \
    --resource-group $RESOURCE_GROUP \
    --location $LOCATION \
    --admin-user $DB_ADMIN_USER \
    --admin-password "$DB_ADMIN_PASSWORD"

echo "--> Configurando o firewall do SQL Server..."

echo "--> 1. Permitindo que todos os serviços do Azure se conectem..."
az sql server firewall-rule create \
    --resource-group $RESOURCE_GROUP \
    --server $DB_SERVER_NAME \
    --name "AllowAllWindowsAzureIps" \
    --start-ip-address "0.0.0.0" \
    --end-ip-address "0.0.0.0"

echo "--> 2. Permitindo a conexão da sua máquina local (para SSMS/DBeaver)..."
MY_IP=$(curl -4 -s ifconfig.me)
az sql server firewall-rule create \
    --resource-group $RESOURCE_GROUP \
    --server $DB_SERVER_NAME \
    --name "AllowMyComputer" \
    --start-ip-address $MY_IP \
    --end-ip-address $MY_IP

echo "--> Criando Banco de Dados SQL no modo Serverless (econômico)..."
az sql db create \
    --resource-group $RESOURCE_GROUP \
    --server $DB_SERVER_NAME \
    --name $DB_NAME \
    --edition "GeneralPurpose" \
    --family "Gen5" \
    --capacity 1 \
    --compute-model "Serverless"

echo "--> Criando Plano do App Service (Linux S1)..."
az appservice plan create \
    --name $APP_SERVICE_PLAN \
    --resource-group $RESOURCE_GROUP \
    --location $LOCATION \
    --is-linux \
    --sku S1

echo "--> Criando Web App (Linux .NET 8)..."
az webapp create \
    --name $WEB_APP_NAME \
    --plan $APP_SERVICE_PLAN \
    --resource-group $RESOURCE_GROUP \
    --runtime "DOTNETCORE:8.0"

echo "--> Configurando a conexão do Web App com o Banco de Dados SQL..."

DB_CONNECTION_STRING="Server=tcp:${DB_SERVER_NAME}.database.windows.net,1433;Initial Catalog=${DB_NAME};User ID=${DB_ADMIN_USER};Password=${DB_ADMIN_PASSWORD};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

# Configura as Variáveis de Ambiente no App Service
# O .NET converte "ConnectionStrings:DefaultConnection" para "ConnectionStrings__DefaultConnection"
az webapp config appsettings set \
    --name $WEB_APP_NAME \
    --resource-group $RESOURCE_GROUP \
    --settings \
    "DatabaseProvider=SqlServer" \
    "ConnectionStrings__DefaultConnection=$DB_CONNECTION_STRING" \
    "Jwt__SecretKey=$JWT_SECRET_KEY" \
    "Jwt__ExpiresInHours=8" \
    "ASPNETCORE_ENVIRONMENT=Production"

echo "### ✅ Deploy de Infraestrutura concluído! ###"
echo "🌐 Acesse sua aplicação (após o pipeline de Release) em: https://${WEB_APP_NAME}.azurewebsites.net"