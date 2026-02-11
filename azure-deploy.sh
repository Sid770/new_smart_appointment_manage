#!/bin/bash

# Azure Deployment Script for Appointment Booking System (Bash Version)
# This script creates all required Azure resources using Azure CLI

# =============================================================================
# CONFIGURATION - UPDATE THESE VALUES
# =============================================================================

RESOURCE_GROUP="rg-appointment-booking"
LOCATION="eastus"
STORAGE_ACCOUNT="stappointmentbook123"  # Must be globally unique, lowercase, no spaces
APP_SERVICE_PLAN="plan-appointment-booking"
APP_SERVICE_NAME="app-appointment-booking-api"  # Must be globally unique
SKU="F1"  # Free tier

# =============================================================================
# STEP 1: LOGIN TO AZURE
# =============================================================================

echo "============================================="
echo "STEP 1: Azure Login"
echo "============================================="

# Login to Azure (will open browser)
az login

# List subscriptions
az account list --output table

echo ""
echo "✅ Please ensure you're using the correct subscription"
echo "To set subscription: az account set --subscription 'SUBSCRIPTION_ID'"
echo ""

# =============================================================================
# STEP 2: CREATE RESOURCE GROUP
# =============================================================================

echo "============================================="
echo "STEP 2: Creating Resource Group"
echo "============================================="

az group create \
    --name $RESOURCE_GROUP \
    --location $LOCATION

echo ""
echo "✅ Resource Group created successfully"
echo ""

# =============================================================================
# STEP 3: CREATE STORAGE ACCOUNT
# =============================================================================

echo "============================================="
echo "STEP 3: Creating Storage Account"
echo "============================================="

az storage account create \
    --name $STORAGE_ACCOUNT \
    --resource-group $RESOURCE_GROUP \
    --location $LOCATION \
    --sku Standard_LRS \
    --kind StorageV2

echo ""
echo "✅ Storage Account created successfully"
echo ""

# =============================================================================
# STEP 4: GET STORAGE CONNECTION STRING
# =============================================================================

echo "============================================="
echo "STEP 4: Getting Storage Connection String"
echo "============================================="

CONNECTION_STRING=$(az storage account show-connection-string \
    --name $STORAGE_ACCOUNT \
    --resource-group $RESOURCE_GROUP \
    --query connectionString \
    --output tsv)

echo ""
echo "✅ Connection String retrieved"
echo "⚠️  SAVE THIS CONNECTION STRING - You'll need it for GitHub Secrets:"
echo ""
echo "$CONNECTION_STRING"
echo ""

# =============================================================================
# STEP 5: CREATE AZURE TABLES
# =============================================================================

echo "============================================="
echo "STEP 5: Creating Azure Tables"
echo "============================================="

# Create TimeSlots table
az storage table create \
    --name "TimeSlots" \
    --account-name $STORAGE_ACCOUNT \
    --connection-string "$CONNECTION_STRING"

echo "✅ TimeSlots table created"

# Create Appointments table
az storage table create \
    --name "Appointments" \
    --account-name $STORAGE_ACCOUNT \
    --connection-string "$CONNECTION_STRING"

echo "✅ Appointments table created"

# =============================================================================
# STEP 6: CREATE APP SERVICE PLAN
# =============================================================================

echo ""
echo "============================================="
echo "STEP 6: Creating App Service Plan (Free Tier)"
echo "============================================="

az appservice plan create \
    --name $APP_SERVICE_PLAN \
    --resource-group $RESOURCE_GROUP \
    --location $LOCATION \
    --sku $SKU \
    --is-linux

echo ""
echo "✅ App Service Plan created successfully"
echo ""

# =============================================================================
# STEP 7: CREATE WEB APP
# =============================================================================

echo "============================================="
echo "STEP 7: Creating Web App"
echo "============================================="

az webapp create \
    --name $APP_SERVICE_NAME \
    --resource-group $RESOURCE_GROUP \
    --plan $APP_SERVICE_PLAN \
    --runtime "DOTNET|8.0"

echo ""
echo "✅ Web App created successfully"
echo ""

# =============================================================================
# STEP 8: CONFIGURE APP SETTINGS
# =============================================================================

echo "============================================="
echo "STEP 8: Configuring App Settings"
echo "============================================="

az webapp config appsettings set \
    --name $APP_SERVICE_NAME \
    --resource-group $RESOURCE_GROUP \
    --settings \
        "AzureTableStorage__ConnectionString=$CONNECTION_STRING" \
        "ASPNETCORE_ENVIRONMENT=Production"

echo ""
echo "✅ App Settings configured successfully"
echo ""

# =============================================================================
# STEP 9: ENABLE CORS
# =============================================================================

echo "============================================="
echo "STEP 9: Enabling CORS for All Origins"
echo "============================================="

az webapp cors add \
    --name $APP_SERVICE_NAME \
    --resource-group $RESOURCE_GROUP \
    --allowed-origins "*"

echo ""
echo "✅ CORS enabled successfully"
echo ""

# =============================================================================
# STEP 10: ENABLE HTTPS ONLY
# =============================================================================

echo "============================================="
echo "STEP 10: Enabling HTTPS Only"
echo "============================================="

az webapp update \
    --name $APP_SERVICE_NAME \
    --resource-group $RESOURCE_GROUP \
    --https-only true

echo ""
echo "✅ HTTPS Only enabled"
echo ""

# =============================================================================
# DEPLOYMENT SUMMARY
# =============================================================================

echo "============================================="
echo "   DEPLOYMENT COMPLETED SUCCESSFULLY! 🎉   "
echo "============================================="

echo ""
echo "📋 DEPLOYMENT SUMMARY:"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""

echo "Resource Group:      $RESOURCE_GROUP"
echo "Location:            $LOCATION"
echo "Storage Account:     $STORAGE_ACCOUNT"
echo "App Service:         $APP_SERVICE_NAME"
echo ""
echo "🌐 API URL:          https://$APP_SERVICE_NAME.azurewebsites.net"
echo "📚 Swagger URL:      https://$APP_SERVICE_NAME.azurewebsites.net/swagger"
echo ""

echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "📝 NEXT STEPS:"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""

echo "1. Add the following secrets to your GitHub repository:"
echo "   • AZURE_WEBAPP_PUBLISH_PROFILE"
echo "   • AZURE_STORAGE_CONNECTION_STRING"
echo ""
echo "2. Get publish profile:"
echo "   az webapp deployment list-publishing-profiles --name $APP_SERVICE_NAME --resource-group $RESOURCE_GROUP --xml"
echo ""
echo "3. Connection String (copy this):"
echo "   $CONNECTION_STRING"
echo ""
echo "4. Push code to GitHub to trigger CI/CD deployment"
echo ""
echo "5. Update Angular environment.prod.ts with API URL:"
echo "   apiUrl: 'https://$APP_SERVICE_NAME.azurewebsites.net/api'"
echo ""

echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "💡 USEFUL COMMANDS:"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""

echo "View logs:"
echo "  az webapp log tail --name $APP_SERVICE_NAME --resource-group $RESOURCE_GROUP"
echo ""
echo "Restart app:"
echo "  az webapp restart --name $APP_SERVICE_NAME --resource-group $RESOURCE_GROUP"
echo ""
echo "Delete all resources:"
echo "  az group delete --name $RESOURCE_GROUP --yes --no-wait"
echo ""

echo "✅ Setup complete! Ready for deployment."
