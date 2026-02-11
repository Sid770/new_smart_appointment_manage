# Azure CLI Quick Reference Commands

## Authentication & Setup

```bash
# Login to Azure
az login

# List subscriptions
az account list --output table

# Set active subscription
az account set --subscription "SUBSCRIPTION_ID"

# Show current account
az account show
```

## Resource Management

```bash
# List all resource groups
az group list --output table

# Show resource group details
az group show --name rg-appointment-booking

# List all resources in group
az resource list --resource-group rg-appointment-booking --output table

# Delete resource group (and all resources)
az group delete --name rg-appointment-booking --yes --no-wait
```

## Storage Account

```bash
# List storage accounts
az storage account list --resource-group rg-appointment-booking --output table

# Get connection string
az storage account show-connection-string \
    --name stappointmentbook123 \
    --resource-group rg-appointment-booking \
    --query connectionString \
    --output tsv

# List tables
az storage table list \
    --account-name stappointmentbook123 \
    --connection-string "YOUR_CONNECTION_STRING"

# Delete a table
az storage table delete \
    --name "TimeSlots" \
    --account-name stappointmentbook123 \
    --connection-string "YOUR_CONNECTION_STRING"
```

## App Service

```bash
# List app services
az webapp list --resource-group rg-appointment-booking --output table

# Show app service details
az webapp show \
    --name app-appointment-booking-api \
    --resource-group rg-appointment-booking

# Restart app service
az webapp restart \
    --name app-appointment-booking-api \
    --resource-group rg-appointment-booking

# Stop app service
az webapp stop \
    --name app-appointment-booking-api \
    --resource-group rg-appointment-booking

# Start app service
az webapp start \
    --name app-appointment-booking-api \
    --resource-group rg-appointment-booking

# Browse to app service
az webapp browse \
    --name app-appointment-booking-api \
    --resource-group rg-appointment-booking
```

## Configuration & Settings

```bash
# List app settings
az webapp config appsettings list \
    --name app-appointment-booking-api \
    --resource-group rg-appointment-booking \
    --output table

# Set app setting
az webapp config appsettings set \
    --name app-appointment-booking-api \
    --resource-group rg-appointment-booking \
    --settings "KEY=VALUE"

# Delete app setting
az webapp config appsettings delete \
    --name app-appointment-booking-api \
    --resource-group rg-appointment-booking \
    --setting-names "KEY"

# Get publish profile
az webapp deployment list-publishing-profiles \
    --name app-appointment-booking-api \
    --resource-group rg-appointment-booking \
    --xml
```

## Logging & Monitoring

```bash
# Enable logging
az webapp log config \
    --name app-appointment-booking-api \
    --resource-group rg-appointment-booking \
    --application-logging filesystem \
    --detailed-error-messages true \
    --failed-request-tracing true \
    --web-server-logging filesystem

# Stream logs (real-time)
az webapp log tail \
    --name app-appointment-booking-api \
    --resource-group rg-appointment-booking

# Download logs
az webapp log download \
    --name app-appointment-booking-api \
    --resource-group rg-appointment-booking \
    --log-file "logs.zip"
```

## Deployment

```bash
# Deploy from local directory
az webapp deploy \
    --name app-appointment-booking-api \
    --resource-group rg-appointment-booking \
    --src-path "./publish.zip" \
    --type zip

# Deploy from GitHub
az webapp deployment source config \
    --name app-appointment-booking-api \
    --resource-group rg-appointment-booking \
    --repo-url "https://github.com/Sid770/new_smart_appointment_manage" \
    --branch main \
    --manual-integration

# Show deployment history
az webapp deployment list-publishing-credentials \
    --name app-appointment-booking-api \
    --resource-group rg-appointment-booking
```

## CORS Configuration

```bash
# Show CORS settings
az webapp cors show \
    --name app-appointment-booking-api \
    --resource-group rg-appointment-booking

# Add CORS origin
az webapp cors add \
    --name app-appointment-booking-api \
    --resource-group rg-appointment-booking \
    --allowed-origins "https://your-frontend.vercel.app"

# Remove CORS origin
az webapp cors remove \
    --name app-appointment-booking-api \
    --resource-group rg-appointment-booking \
    --allowed-origins "https://your-frontend.vercel.app"
```

## Scaling

```bash
# List App Service Plans
az appservice plan list --output table

# Scale up (change pricing tier)
az appservice plan update \
    --name plan-appointment-booking \
    --resource-group rg-appointment-booking \
    --sku B1  # Basic, Standard, Premium

# Scale out (increase instances)
az appservice plan update \
    --name plan-appointment-booking \
    --resource-group rg-appointment-booking \
    --number-of-workers 2
```

## Diagnostics

```bash
# Run health check
az webapp show \
    --name app-appointment-booking-api \
    --resource-group rg-appointment-booking \
    --query "state" \
    --output tsv

# Check runtime status
az webapp show \
    --name app-appointment-booking-api \
    --resource-group rg-appointment-booking \
    --query "availabilityState" \
    --output tsv

# Get outbound IP addresses
az webapp show \
    --name app-appointment-booking-api \
    --resource-group rg-appointment-booking \
    --query "outboundIpAddresses" \
    --output tsv
```

## Cost Management

```bash
# Show cost for resource group
az consumption usage list \
    --start-date 2024-01-01 \
    --end-date 2024-01-31 \
    --resource-group rg-appointment-booking

# List all resources with pricing tier
az resource list \
    --resource-group rg-appointment-booking \
    --query "[].{Name:name, Type:type, Location:location}" \
    --output table
```

## Clean Up

```bash
# Delete specific resource
az webapp delete \
    --name app-appointment-booking-api \
    --resource-group rg-appointment-booking

# Delete storage account
az storage account delete \
    --name stappointmentbook123 \
    --resource-group rg-appointment-booking \
    --yes

# Delete entire resource group (recommended)
az group delete \
    --name rg-appointment-booking \
    --yes \
    --no-wait
```

## Useful Queries

```bash
# Get all resource names in a group
az resource list \
    --resource-group rg-appointment-booking \
    --query "[].name" \
    --output tsv

# Check if app service exists
az webapp show \
    --name app-appointment-booking-api \
    --resource-group rg-appointment-booking \
    --query "name" \
    --output tsv 2>/dev/null || echo "Not found"

# Get app service URL
az webapp show \
    --name app-appointment-booking-api \
    --resource-group rg-appointment-booking \
    --query "defaultHostName" \
    --output tsv
```

## Troubleshooting

```bash
# Check if resource exists
az resource show \
    --resource-group rg-appointment-booking \
    --name app-appointment-booking-api \
    --resource-type "Microsoft.Web/sites"

# Verify storage connection
az storage account show \
    --name stappointmentbook123 \
    --resource-group rg-appointment-booking \
    --query "primaryEndpoints"

# Check app service logs for errors
az webapp log tail \
    --name app-appointment-booking-api \
    --resource-group rg-appointment-booking \
    --filter "Error"
```

---

## 💡 Pro Tips

1. **Use `--output table`** for human-readable output
2. **Use `--output tsv`** for scripting (tab-separated values)
3. **Use `--query`** with JMESPath to filter results
4. **Use `--no-wait`** for async operations (faster scripts)
5. **Use `--yes`** to skip confirmation prompts

## 📚 Resources

- [Azure CLI Documentation](https://learn.microsoft.com/en-us/cli/azure/)
- [JMESPath Query Tutorial](https://jmespath.org/tutorial.html)
- [Azure Pricing Calculator](https://azure.microsoft.com/en-us/pricing/calculator/)
