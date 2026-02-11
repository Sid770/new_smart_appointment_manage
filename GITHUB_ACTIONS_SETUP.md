# GitHub Actions CI/CD Setup Guide

This guide walks you through setting up GitHub Actions for automatic deployment to Azure App Service.

## 📋 Prerequisites

- GitHub repository created: https://github.com/Sid770/new_smart_appointment_manage
- Azure resources deployed (run `azure-deploy.ps1` or `azure-deploy.sh`)
- Azure Web App created and running

---

## 🔑 Step 1: Get Azure Publish Profile

The publish profile contains authentication credentials for GitHub Actions to deploy to your Azure Web App.

### Using Azure CLI (Recommended)

```bash
az webapp deployment list-publishing-profiles \
    --name app-appointment-booking-api \
    --resource-group rg-appointment-booking \
    --xml > publish-profile.xml
```

The entire content of `publish-profile.xml` will be used as a GitHub secret.

### Using Azure Portal

1. Go to [Azure Portal](https://portal.azure.com)
2. Navigate to your **App Service** (`app-appointment-booking-api`)
3. Click **"Get publish profile"** button in the top menu
4. Download the `.PublishSettings` file
5. Open it in a text editor (it's XML)

---

## 🔐 Step 2: Add GitHub Secrets

Secrets are encrypted environment variables used by GitHub Actions.

### Navigate to Repository Settings

1. Go to your GitHub repository: https://github.com/Sid770/new_smart_appointment_manage
2. Click **Settings** tab
3. In the left sidebar, expand **Secrets and variables** → Click **Actions**
4. Click **"New repository secret"** button

### Add Required Secrets

#### Secret 1: `AZURE_WEBAPP_PUBLISH_PROFILE`

- **Name:** `AZURE_WEBAPP_PUBLISH_PROFILE`
- **Value:** Paste the entire XML content from the publish profile file
- Click **Add secret**

✅ **Verification:** You should see it listed under "Repository secrets"

---

## ✅ Step 3: Verify Workflow Configuration

The workflow file is already created at `.github/workflows/azure-deploy.yml`.

### Key Configuration Points

Check these values in the workflow file:

```yaml
env:
  DOTNET_VERSION: '8.0.x'
  AZURE_WEBAPP_NAME: 'app-appointment-booking-api'  # ⚠️ Must match your App Service name
  AZURE_WEBAPP_PACKAGE_PATH: './AppointmentBookingAPI'
```

**Important:** If you changed the App Service name during Azure deployment, update `AZURE_WEBAPP_NAME` in the workflow file.

---

## 🚀 Step 4: Trigger Deployment

### Option 1: Push to Main Branch (Automatic)

```bash
git add .
git commit -m "Setup Azure deployment workflow"
git push origin main
```

### Option 2: Manual Trigger

1. Go to **Actions** tab in your GitHub repository
2. Select **"Azure Deployment CI/CD"** workflow
3. Click **"Run workflow"** button
4. Select branch: `main`
5. Click **"Run workflow"**

---

## 📊 Step 5: Monitor Deployment

### View Workflow Progress

1. Navigate to **Actions** tab in GitHub
2. Click on the running workflow
3. Expand job steps to see real-time logs

### Workflow Stages

The pipeline has 3 jobs:

1. **Build & Test** (3-5 minutes)
   - Restores NuGet packages
   - Builds .NET project
   - Runs XUnit tests
   - Publishes build artifacts

2. **Deploy to Azure** (2-3 minutes)
   - Downloads build artifacts
   - Deploys to Azure App Service
   - Outputs deployment URL

3. **Health Check** (30 seconds)
   - Waits for app to warm up
   - Tests API endpoints
   - Verifies Swagger is accessible

### Success Indicators

✅ All jobs show green checkmarks  
✅ Health check passes  
✅ Deployment URL is accessible

---

## 🔍 Step 6: Verify Deployment

### Test Backend API

```bash
# Test API root
curl https://app-appointment-booking-api.azurewebsites.net/api/timeslots

# Expected: 200 OK or empty array []
```

### Test Swagger UI

Open in browser:
```
https://app-appointment-booking-api.azurewebsites.net/swagger
```

You should see the Swagger UI with all API endpoints.

---

## ⚙️ Advanced Configuration

### Add Environment Variables

If you need to add more app settings (e.g., connection strings):

#### Option 1: Azure CLI

```bash
az webapp config appsettings set \
    --name app-appointment-booking-api \
    --resource-group rg-appointment-booking \
    --settings "MyCustomSetting=MyValue"
```

#### Option 2: Azure Portal

1. Go to App Service → **Configuration**
2. Click **+ New application setting**
3. Add key-value pairs
4. Click **Save**

### Enable Deployment Slots (Blue-Green Deployment)

For zero-downtime deployments:

```bash
# Create staging slot
az webapp deployment slot create \
    --name app-appointment-booking-api \
    --resource-group rg-appointment-booking \
    --slot staging

# Swap slots after testing
az webapp deployment slot swap \
    --name app-appointment-booking-api \
    --resource-group rg-appointment-booking \
    --slot staging \
    --target-slot production
```

Update workflow to deploy to staging:
```yaml
- uses: azure/webapps-deploy@v2
  with:
    app-name: ${{ env.AZURE_WEBAPP_NAME }}
    slot-name: staging  # Add this line
    publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE_STAGING }}
```

---

## 🐛 Troubleshooting

### Deployment Fails with "Publish Profile Invalid"

**Solution:** Re-download the publish profile from Azure:

```bash
az webapp deployment list-publishing-profiles \
    --name app-appointment-booking-api \
    --resource-group rg-appointment-booking \
    --xml
```

Copy the output and update the GitHub secret.

### Build Fails with "Project Not Found"

**Solution:** Verify the `AZURE_WEBAPP_PACKAGE_PATH` in the workflow file:

```yaml
AZURE_WEBAPP_PACKAGE_PATH: './AppointmentBookingAPI'  # Should point to folder with .csproj
```

### Tests Fail During Workflow

**Solution:** Run tests locally first:

```bash
cd AppointmentBookingAPI.Tests
dotnet test --verbosity normal
```

Fix any failing tests before pushing to GitHub.

### Health Check Fails After Deployment

**Solution:** Check Azure App Service logs:

```bash
az webapp log tail \
    --name app-appointment-booking-api \
    --resource-group rg-appointment-booking
```

Look for startup errors or missing configuration.

### Workflow Doesn't Trigger

**Solution:** Verify `.github/workflows/azure-deploy.yml` is:
- In the correct directory structure
- Committed to the repository
- On the `main` branch
- Has proper YAML syntax (use a YAML validator)

---

## 📈 Workflow Metrics

After successful deployment, you can view:

- **Build duration:** Typically 3-7 minutes
- **Deployment frequency:** On every push to `main`
- **Success rate:** Track in Actions tab
- **Deployment history:** Azure Portal → App Service → Deployment Center

---

## 🎯 Best Practices

1. **Always run tests locally** before pushing to `main`
2. **Use pull requests** to trigger CI checks before merging
3. **Monitor Azure costs** - Free tier has limits (60 CPU minutes/day)
4. **Enable branch protection** in GitHub Settings → Branches
5. **Use environment secrets** for production-specific values
6. **Tag releases** for production deployments (`git tag v1.0.0`)
7. **Set up alerts** in Azure for high CPU/memory usage

---

## 🔗 Useful Links

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [Azure Web Apps Deploy Action](https://github.com/Azure/webapps-deploy)
- [.NET Build Action](https://github.com/actions/setup-dotnet)
- [Azure App Service Docs](https://learn.microsoft.com/en-us/azure/app-service/)

---

## ✅ Checklist

- [ ] Publish profile downloaded from Azure
- [ ] `AZURE_WEBAPP_PUBLISH_PROFILE` secret added to GitHub
- [ ] Workflow file committed to repository
- [ ] Pushed to `main` branch
- [ ] Workflow triggered successfully
- [ ] All jobs completed without errors
- [ ] API endpoint is accessible
- [ ] Swagger UI is working
- [ ] Backend is using Azure Table Storage

🎉 **Congratulations!** Your CI/CD pipeline is now fully configured!
