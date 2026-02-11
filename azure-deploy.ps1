# Azure Deployment Script for Appointment Booking System
# This script creates all required Azure resources using Azure CLI

# =============================================================================
# CONFIGURATION - UPDATE THESE VALUES
# =============================================================================

$RESOURCE_GROUP="rg-appointment-booking"
$LOCATION="eastus"
$STORAGE_ACCOUNT="stappointmentbook123"  # Must be globally unique, lowercase, no spaces
$APP_SERVICE_PLAN="plan-appointment-booking"
$APP_SERVICE_NAME="app-appointment-booking-api"  # Must be globally unique
$SKU="F1"  # Free tier

# =============================================================================
# STEP 1: LOGIN TO AZURE
# =============================================================================

Write-Host "=============================================" -ForegroundColor Cyan
Write-Host "STEP 1: Azure Login" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan

# Login to Azure (will open browser)
az login

# List subscriptions
azaccount list --output table

Write-Host "`n✅ Please ensure you're using the correct subscription" -ForegroundColor Green
Write-Host "To set subscription: az account set --subscription 'SUBSCRIPTION_ID'`n" -ForegroundColor Yellow

# =============================================================================
# STEP 2: CREATE RESOURCE GROUP
# =============================================================================

Write-Host "=============================================" -ForegroundColor Cyan
Write-Host "STEP 2: Creating Resource Group" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan

az group create `
    --name $RESOURCE_GROUP `
    --location $LOCATION

Write-Host "`n✅ Resource Group created successfully`n" -ForegroundColor Green

# =============================================================================
# STEP 3: CREATE STORAGE ACCOUNT
# =============================================================================

Write-Host "=============================================" -ForegroundColor Cyan
Write-Host "STEP 3: Creating Storage Account" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan

az storage account create `
    --name $STORAGE_ACCOUNT `
    --resource-group $RESOURCE_GROUP `
    --location $LOCATION `
    --sku Standard_LRS `
    --kind StorageV2

Write-Host "`n✅ Storage Account created successfully`n" -ForegroundColor Green

# =============================================================================
# STEP 4: GET STORAGE CONNECTION STRING
# =============================================================================

Write-Host "=============================================" -ForegroundColor Cyan
Write-Host "STEP 4: Getting Storage Connection String" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan

$CONNECTION_STRING=$(az storage account show-connection-string `
    --name $STORAGE_ACCOUNT `
    --resource-group $RESOURCE_GROUP `
    --query connectionString `
    --output tsv)

Write-Host "`n✅ Connection String retrieved" -ForegroundColor Green
Write-Host "⚠️  SAVE THIS CONNECTION STRING - You'll need it for GitHub Secrets:`n" -ForegroundColor Yellow
Write-Host $CONNECTION_STRING -ForegroundColor White
Write-Host ""

# =============================================================================
# STEP 5: CREATE AZURE TABLES
# =============================================================================

Write-Host "=============================================" -ForegroundColor Cyan
Write-Host "STEP 5: Creating Azure Tables" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan

# Create TimeSlots table
az storage table create `
    --name "TimeSlots" `
    --account-name $STORAGE_ACCOUNT `
    --connection-string $CONNECTION_STRING

Write-Host "✅ TimeSlots table created" -ForegroundColor Green

# Create Appointments table
az storage table create `
    --name "Appointments" `
    --account-name $STORAGE_ACCOUNT `
    --connection-string $CONNECTION_STRING

Write-Host "✅ Appointments table created" -ForegroundColor Green

# =============================================================================
# STEP 6: CREATE APP SERVICE PLAN
# =============================================================================

Write-Host "`n=============================================" -ForegroundColor Cyan
Write-Host "STEP 6: Creating App Service Plan (Free Tier)" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan

az appservice plan create `
    --name $APP_SERVICE_PLAN `
    --resource-group $RESOURCE_GROUP `
    --location $LOCATION `
    --sku $SKU `
    --is-linux

Write-Host "`n✅ App Service Plan created successfully`n" -ForegroundColor Green

# =============================================================================
# STEP 7: CREATE WEB APP
# =============================================================================

Write-Host "=============================================" -ForegroundColor Cyan
Write-Host "STEP 7: Creating Web App" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan

az webapp create `
    --name $APP_SERVICE_NAME `
    --resource-group $RESOURCE_GROUP `
    --plan $APP_SERVICE_PLAN `
    --runtime "DOTNET|8.0"

Write-Host "`n✅ Web App created successfully`n" -ForegroundColor Green

# =============================================================================
# STEP 8: CONFIGURE APP SETTINGS
# =============================================================================

Write-Host "=============================================" -ForegroundColor Cyan
Write-Host "STEP 8: Configuring App Settings" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan

az webapp config appsettings set `
    --name $APP_SERVICE_NAME `
    --resource-group $RESOURCE_GROUP `
    --settings `
        "AzureTableStorage__ConnectionString=$CONNECTION_STRING" `
        "ASPNETCORE_ENVIRONMENT=Production"

Write-Host "`n✅ App Settings configured successfully`n" -ForegroundColor Green

# =============================================================================
# STEP 9: ENABLE CORS
# =============================================================================

Write-Host "=============================================" -ForegroundColor Cyan
Write-Host "STEP 9: Enabling CORS for All Origins" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan

az webapp cors add `
    --name $APP_SERVICE_NAME `
    --resource-group $RESOURCE_GROUP `
    --allowed-origins "*"

Write-Host "`n✅ CORS enabled successfully`n" -ForegroundColor Green

# =============================================================================
# STEP 10: ENABLE HTTPS ONLY
# =============================================================================

Write-Host "=============================================" -ForegroundColor Cyan
Write-Host "STEP 10: Enabling HTTPS Only" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan

az webapp update `
    --name $APP_SERVICE_NAME `
    --resource-group $RESOURCE_GROUP `
    --https-only true

Write-Host "`n✅ HTTPS Only enabled`n" -ForegroundColor Green

# =============================================================================
# DEPLOYMENT SUMMARY
# =============================================================================

Write-Host "=============================================" -ForegroundColor Green
Write-Host "   DEPLOYMENT COMPLETED SUCCESSFULLY! 🎉   " -ForegroundColor Green
Write-Host "=============================================" -ForegroundColor Green

Write-Host "`n📋 DEPLOYMENT SUMMARY:" -ForegroundColor Cyan
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━`n" -ForegroundColor Cyan

Write-Host "Resource Group:      $RESOURCE_GROUP" -ForegroundColor White
Write-Host "Location:            $LOCATION" -ForegroundColor White
Write-Host "Storage Account:     $STORAGE_ACCOUNT" -ForegroundColor White
Write-Host "App Service:         $APP_SERVICE_NAME" -ForegroundColor White
Write-Host ""
Write-Host "🌐 API URL:          https://$APP_SERVICE_NAME.azurewebsites.net" -ForegroundColor Yellow
Write-Host "📚 Swagger URL:      https://$APP_SERVICE_NAME.azurewebsites.net/swagger" -ForegroundColor Yellow
Write-Host ""

Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
Write-Host "📝 NEXT STEPS:" -ForegroundColor Cyan
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━`n" -ForegroundColor Cyan

Write-Host "1. Add the following secrets to your GitHub repository:" -ForegroundColor White
Write-Host "   • AZURE_WEBAPP_PUBLISH_PROFILE" -ForegroundColor Yellow
Write-Host "   • AZURE_STORAGE_CONNECTION_STRING" -ForegroundColor Yellow
Write-Host ""
Write-Host "2. Get publish profile:" -ForegroundColor White
Write-Host "   az webapp deployment list-publishing-profiles --name $APP_SERVICE_NAME --resource-group $RESOURCE_GROUP --xml" -ForegroundColor Gray
Write-Host ""
Write-Host "3. Connection String (copy this):" -ForegroundColor White
Write-Host "   $CONNECTION_STRING" -ForegroundColor Gray
Write-Host ""
Write-Host "4. Push code to GitHub to trigger CI/CD deployment" -ForegroundColor White
Write-Host ""
Write-Host "5. Update Angular environment.prod.ts with API URL:" -ForegroundColor White
Write-Host "   apiUrl: 'https://$APP_SERVICE_NAME.azurewebsites.net/api'" -ForegroundColor Gray
Write-Host ""

Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
Write-Host "💡 USEFUL COMMANDS:" -ForegroundColor Cyan
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━`n" -ForegroundColor Cyan

Write-Host "View logs:" -ForegroundColor White
Write-Host "  az webapp log tail --name $APP_SERVICE_NAME --resource-group $RESOURCE_GROUP" -ForegroundColor Gray
Write-Host ""
Write-Host "Restart app:" -ForegroundColor White
Write-Host "  az webapp restart --name $APP_SERVICE_NAME --resource-group $RESOURCE_GROUP" -ForegroundColor Gray
Write-Host ""
Write-Host "Delete all resources:" -ForegroundColor White
Write-Host "  az group delete --name $RESOURCE_GROUP --yes --no-wait" -ForegroundColor Gray
Write-Host ""

Write-Host "✅ Setup complete! Ready for deployment." -ForegroundColor Green
