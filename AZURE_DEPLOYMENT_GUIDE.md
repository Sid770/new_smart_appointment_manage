# Azure Deployment Guide - Smart Appointment Booking System

Complete guide to deploy your Angular + .NET appointment booking system to Azure cloud.

---

## 🏗️ Architecture Overview

```
Users (Browser)
    ↓
Angular Frontend (Vercel / Azure Static Web Apps)
    ↓
.NET Web API (Azure App Service - Free Tier)
    ↓
Azure Table Storage
```

---

## 📋 Prerequisites

- ✅ Azure Account (Free tier available)
- ✅ Azure CLI installed
- ✅ .NET 10 SDK
- ✅ Node.js & npm
- ✅ Git & GitHub account
- ✅ Vercel account (optional)

---

## PART 1: Azure Portal Setup

### Step 1: Create Resource Group

1. Login to [Azure Portal](https://portal.azure.com)
2. Click **"Resource groups"** → **"+ Create"**
3. Fill in details:
   - **Subscription**: Select your subscription
   - **Resource group**: `appointment-booking-rg`
   - **Region**: `East US` (or closest to you)
4. Click **"Review + create"** → **"Create"**

### Step 2: Create Storage Account

1. Search for **"Storage accounts"** → **"+ Create"**
2. Configure:
   - **Resource group**: `appointment-booking-rg`
   - **Storage account name**: `appointmentstorage123` (must be globally unique)
   - **Region**: Same as resource group
   - **Performance**: Standard
   - **Redundancy**: LRS (Locally-redundant storage) - cheapest option
3. Click **"Review + create"** → **"Create"**
4. Wait for deployment to complete

### Step 3: Create Tables in Storage Account

1. Navigate to your storage account
2. Go to **"Data storage" → "Tables"**
3. Click **"+ Table"** and create these tables:
   - `TimeSlots`
   - `Appointments`
4. Tables are created instantly

### Step 4: Get Storage Connection String

1. In storage account, go to **"Security + networking" → "Access keys"**
2. Click **"Show keys"**
3. Copy **"Connection string"** from key1
4. **IMPORTANT**: Save this securely - you'll need it for deployment

Format looks like:
```
DefaultEndpointsProtocol=https;AccountName=appointmentstorage123;AccountKey=...;EndpointSuffix=core.windows.net
```

### Step 5: Create App Service (Backend API)

1. Search for **"App Services"** → **"+ Create"**
2. Configure:
   - **Resource group**: `appointment-booking-rg`
   - **Name**: `appointment-booking-api` (will be your URL)
   - **Publish**: Code
   - **Runtime stack**: .NET 10 (LTS)
   - **Operating System**: Linux
   - **Region**: Same as resource group
   - **Pricing plan**: F1 (Free tier)
3. Click **"Review + create"** → **"Create"**

### Step 6: Configure App Service Environment Variables

1. Navigate to your App Service
2. Go to **"Settings" → "Configuration"**
3. Click **"+ New application setting"** and add:

   **Setting 1:**
   - Name: `AzureTableStorage__ConnectionString`
   - Value: `[Your connection string from Step 4]`

   **Setting 2:**
   - Name: `ASPNETCORE_ENVIRONMENT`
   - Value: `Production`

   **Setting 3:**
   - Name: `UseAzureStorage`
   - Value: `true`

4. Click **"Save"** → **"Continue"**
5. **Restart the App Service**

---

## PART 2: Deploy Backend via GitHub Actions

### Step 1: Get Publish Profile

1. In your App Service, click **"Get publish profile"**
2. Download the `.PublishSettings` file
3. Open it in notepad and copy ALL content

### Step 2: Add GitHub Secret

1. Go to your GitHub repository: `https://github.com/Sid770/new_smart_appointment_manage`
2. Click **"Settings" → "Secrets and variables" → "Actions"**
3. Click **"New repository secret"**
4. Name: `AZURE_WEBAPP_PUBLISH_PROFILE`
5. Value: Paste the entire publish profile content
6. Click **"Add secret"**

### Step 3: Update GitHub Actions Workflow

The workflow file is already created at `.github/workflows/azure-backend-deploy.yml`

Update line 12 with your App Service name:
```yaml
AZURE_WEBAPP_NAME: appointment-booking-api  # Replace with YOUR App Service name
```

### Step 4: Deploy

1. Commit and push changes to `main` branch:
```bash
git add .
git commit -m "Configure Azure deployment"
git push origin main
```

2. Go to **"Actions"** tab in GitHub
3. Watch the deployment progress
4. If successful, your API is live! ✅

### Step 5: Verify Backend Deployment

Test these URLs (replace with your App Service name):

- **Swagger**: `https://appointment-booking-api.azurewebsites.net/swagger`
- **Health Check**: `https://appointment-booking-api.azurewebsites.net/api/timeslots`

---

## PART 3: Deploy Frontend to Vercel

### Option A: Vercel (Recommended)

#### Step 1: Install Vercel CLI

```bash
npm install -g vercel
```

#### Step 2: Update Production Environment

Edit `src/environments/environment.prod.ts`:
```typescript
export const environment = {
  production: true,
  apiUrl: 'https://appointment-booking-api.azurewebsites.net/api'
  // Replace with YOUR App Service URL
};
```

#### Step 3: Deploy to Vercel

```bash
# Login to Vercel
vercel login

# Deploy
vercel --prod
```

#### Step 4: Configure Vercel via Dashboard

1. Go to [Vercel Dashboard](https://vercel.com/dashboard)
2. Select your project
3. Go to **"Settings" → "General"**
4. Set **Framework Preset**: Angular
5. Set **Build Command**: `npm run build`
6. Set **Output Directory**: `dist/hcl3/browser`
7. Save changes

Your frontend is live at: `https://your-project.vercel.app` ✅

### Option B: Azure Static Web Apps

#### Step 1: Create Static Web App

1. In Azure Portal, search **"Static Web Apps"** → **"+ Create"**
2. Configure:
   - **Resource group**: `appointment-booking-rg`
   - **Name**: `appointment-booking-frontend`
   - **Plan type**: Free
   - **Region**: East US 2
   - **Deployment source**: GitHub
3. Authenticate with GitHub
4. Select repository and branch (`main`)
5. Build Presets: **Angular**
6. App location: `/`
7. Output location: `dist/hcl3/browser`
8. Click **"Review + create"** → **"Create"**

Azure automatically creates a GitHub Actions workflow for deployment!

---

## PART 4: Post-Deployment Configuration

### Update CORS in Backend

Your API already has CORS configured for production. No changes needed! ✅

### Update Frontend API URL

If using Azure Static Web Apps, update `environment.prod.ts`:
```typescript
export const environment = {
  production: true,
  apiUrl: 'https://appointment-booking-api.azurewebsites.net/api'
};
```

Then redeploy:
```bash
git add .
git commit -m "Update production API URL"
git push origin main
```

---

## PART 5: Testing & Verification

### ✅ Backend Verification

1. **Check App Service Status**
   - Go to App Service → Overview
   - Status should be "Running"

2. **Test Swagger UI**
   - Open: `https://your-app-name.azurewebsites.net/swagger`
   - Should see API documentation

3. **Test API Endpoints**
   ```bash
   # Get available slots
   curl https://your-app-name.azurewebsites.net/api/timeslots/available
   ```

4. **Check Logs**
   - Go to App Service → **"Monitoring" → "Log stream"**
   - Watch for errors

### ✅ Storage Verification

1. Go to Storage Account → **"Data storage" → "Tables"**
2. Click on `TimeSlots` or `Appointments`
3. Click **"Storage Browser"**
4. After creating data, you should see entries here

### ✅ Frontend Verification

1. Open your Vercel/Azure Static Web App URL
2. Check browser console (F12) for errors
3. Test all pages:
   - Dashboard
   - Available Slots
   - Book Appointment
   - My Appointments
   - Admin Panel

4. **Test End-to-End Flow:**
   - Create time slots in Admin panel
   - Book an appointment
   - Search appointments
   - Verify data persists in Azure Tables

---

## 🐛 Troubleshooting

### Issue: 500 Internal Server Error

**Solution:**
1. Check App Service logs:
   - Go to **"Monitoring" → "Log stream"**
2. Verify connection string is correct
3. Ensure tables exist in Storage Account
4. Restart App Service

### Issue: CORS Errors

**Solution:**
1. Backend already configured with AllowAny in production
2. Ensure you're using HTTPS URLs
3. Check browser console for exact error

### Issue: API Returns 404

**Solution:**
1. Verify deployment succeeded in GitHub Actions
2. Check App Service URL is correct
3. API is at: `https://your-app.azurewebsites.net/api/...`
4. Swagger at: `https://your-app.azurewebsites.net/swagger`

### Issue: Frontend Can't Connect to Backend

**Solution:**
1. Check `environment.prod.ts` has correct API URL
2. Ensure URL ends with `/api` (no trailing slash)
3. API must be HTTPS for production
4. Redeploy frontend after URL change

### Issue: Data Not Persisting

**Solution:**
1. Verify `UseAzureStorage=true` in App Service configuration
2. Check connection string format is correct
3. Ensure tables exist with exact names: `TimeSlots`, `Appointments`
4. Check App Service logs for storage errors

### Issue: App Service Won't Start

**Solution:**
1. Check .NET runtime version matches (10.0)
2. Verify all environment variables are set
3. Check for startup errors in logs
4. Try restarting: **"Overview" → "Restart"**

---

## 💰 Cost Management

### Free Tier Limits

- **App Service F1**: 
  - 60 CPU minutes/day
  - 1 GB RAM
  - 1 GB storage
  - **Cost**: FREE

- **Storage Account (LRS)**:
  - First 5 GB: FREE
  - Table storage: Very cheap (~$0.07/GB/month)
  - **Cost**: ~$0-2/month

- **Vercel Free Tier**:
  - 100 GB bandwidth/month
  - Unlimited static sites
  - **Cost**: FREE

**Total Monthly Cost: $0-2 (essentially free for development!)** 🎉

### Cost Saving Tips

1. **Use F1 Free App Service** (already configured)
2. **Choose LRS storage** (cheapest redundancy)
3. **Delete unused resources** in Resource Group
4. **Stop App Service** when not testing (can restart anytime)
5. **Use Vercel free tier** for frontend
6. **Monitor usage** in Azure Cost Management

---

## 🔒 Security Best Practices

### Already Implemented

✅ HTTPS enforced  
✅ Environment variables for secrets  
✅ No hardcoded connection strings  
✅ CORS properly configured  
✅ Input validation  
✅ Error handling middleware  

### Additional Recommendations

1. **Enable Application Insights** (free tier available)
2. **Set up Azure Key Vault** for production secrets
3. **Enable App Service authentication** for admin endpoints
4. **Regular security updates** via GitHub Dependabot
5. **Monitor logs** for suspicious activity

---

## 📊 Monitoring

### App Service Metrics

1. Go to App Service → **"Monitoring" → "Metrics"**
2. Useful metrics:
   - CPU Percentage
   - Memory Percentage
   - Http Server Errors
   - Response Time

### Application Logs

1. Enable logging:
   - Go to **"Monitoring" → "App Service logs"**
   - Enable **"Application logging"**
   - Level: Information
   - Save

2. View logs:
   - **"Log stream"** for real-time
   - **"Diagnose and solve problems"** for issues

---

## 🔄 Continuous Deployment

### Backend Updates

1. Make changes to backend code
2. Commit and push to `main` branch
3. GitHub Actions automatically deploys
4. Check Actions tab for deployment status

### Frontend Updates

**Vercel:**
- Push to GitHub → Auto-deploys

**Azure Static Web Apps:**
- Push to GitHub → GitHub Actions deploys

---

## 📝 Environment Variables Reference

### App Service Required Settings

| Name | Value | Description |
|------|-------|-------------|
| `AzureTableStorage__ConnectionString` | Connection string | From Storage Account |
| `ASPNETCORE_ENVIRONMENT` | `Production` | Environment mode |
| `UseAzureStorage` | `true` | Enable Azure storage |

### GitHub Secrets Required

| Name | Value | Description |
|------|-------|-------------|
| `AZURE_WEBAPP_PUBLISH_PROFILE` | XML content | Downloaded from App Service |

---

## ✅ Final Deployment Checklist

### Pre-Deployment

- [ ] Azure account created
- [ ] Resource group created
- [ ] Storage account created with tables
- [ ] App Service created
- [ ] Connection string copied
- [ ] GitHub repository ready

### Backend Deployment

- [ ] Publish profile added to GitHub secrets
- [ ] `azure-backend-deploy.yml` workflow updated
- [ ] Environment variables configured in App Service
- [ ] Pushed to main branch
- [ ] GitHub Actions deployment succeeded
- [ ] Swagger UI accessible
- [ ] API endpoints responding

### Frontend Deployment

- [ ] `environment.prod.ts` updated with API URL
- [ ] Vercel account created (or Azure Static Web App)
- [ ] Frontend deployed successfully
- [ ] Can access frontend URL
- [ ] Browser console shows no errors

### Integration Testing

- [ ] Frontend loads successfully
- [ ] Can view dashboard
- [ ] Can see available slots (may be empty initially)
- [ ] Admin panel loads
- [ ] Can create time slots
- [ ] Slots appear in available slots page
- [ ] Can book an appointment
- [ ] Appointment appears in My Appointments
- [ ] Data persists after refresh
- [ ] Can view data in Azure Storage Browser

### Production Verification

- [ ] SSL/HTTPS working
- [ ] CORS working properly
- [ ] API responses are correct
- [ ] Error handling working
- [ ] Logging functioning
- [ ] No console errors
- [ ] Performance acceptable

---

## 🆘 Support Resources

- **Azure Documentation**: https://docs.microsoft.com/azure
- **Azure Portal**: https://portal.azure.com
- **GitHub Actions Docs**: https://docs.github.com/actions
- **Vercel Docs**: https://vercel.com/docs
- **Project Repository**: https://github.com/Sid770/new_smart_appointment_manage

---

## 🎉 Success Indicators

Your deployment is successful when:

✅ Backend API accessible at `https://your-app.azurewebsites.net/swagger`  
✅ Frontend accessible at Vercel/Azure Static Web Apps URL  
✅ Frontend can fetch data from backend  
✅ Can create time slots in admin panel  
✅ Can book appointments  
✅ Data persists in Azure Table Storage  
✅ No CORS errors in browser console  
✅ App Service status shows "Running"  
✅ GitHub Actions builds show green ✓  

---

**Congratulations! Your Smart Appointment Booking System is now live on Azure! 🚀**

For questions or issues, check the troubleshooting section or Azure documentation.
