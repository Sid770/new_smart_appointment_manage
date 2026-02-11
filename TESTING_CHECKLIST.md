# Testing & Verification Checklist

Complete checklist to verify your Smart Appointment Booking System works correctly on localhost and Azure cloud.

---

## 📝 Pre-Deployment Testing (Localhost)

### ✅ Backend API Tests

#### 1. Server Startup
- [ ] Backend starts without errors on `http://localhost:5000`
- [ ] No error messages in console
- [ ] Startup messages appear correctly
- [ ] SQLite database file created (`AppointmentBooking.db`)

#### 2. Swagger UI
- [ ] Swagger accessible at `http://localhost:5000/swagger`
- [ ] All endpoints visible
- [ ] Can expand endpoint documentation
- [ ] Schemas are defined

#### 3. Time Slots API
```bash
# Test available slots
curl http://localhost:5000/api/timeslots/available

# Expected: Returns array of available time slots
```
- [ ] GET `/api/timeslots` returns data
- [ ] GET `/api/timeslots/available` filters correctly
- [ ] POST `/api/timeslots` creates new slot
- [ ] DELETE `/api/timeslots/{id}` removes slot

#### 4. Appointments API
```bash
# Test get all appointments
curl http://localhost:5000/api/appointments

# Expected: Returns array of appointments
```
- [ ] GET `/api/appointments` returns data
- [ ] POST `/api/appointments` books appointment
- [ ] POST `/api/appointments/{id}/cancel` cancels booking

#### 5. Conflict Detection
- [ ] Cannot create overlapping time slots for same provider
- [ ] Can create overlapping slots for different providers
- [ ] Cannot book already booked slot
- [ ] Error messages are clear

#### 6. Unit Tests
```bash
cd AppointmentBookingAPI.Tests
dotnet test
```
- [ ] All tests pass
- [ ] No test failures
- [ ] Coverage includes critical paths

---

### ✅ Frontend Tests

#### 1. Application Startup
- [ ] Frontend starts on `http://localhost:4200`
- [ ] No console errors (check F12 → Console)
- [ ] No 404 errors for assets
- [ ] Page loads within 3 seconds

#### 2. Dashboard Page
- [ ] Dashboard loads correctly
- [ ] Navigation menu visible
- [ ] Statistics cards show data
- [ ] "Available Slots" button works
- [ ] "Book Appointment" button works
- [ ] "Admin" link works

#### 3. Available Slots Page
- [ ] Page loads without errors
- [ ] Time slots display correctly
- [ ] Date filter works
- [ ] Provider filter works
- [ ] "Book Now" buttons visible
- [ ] Clicking "Book Now" navigates correctly

#### 4. Book Appointment Page
- [ ] Form displays correctly
- [ ] All fields are editable
- [ ] Validation works (try empty fields)
- [ ] "Confirm Booking" button works
- [ ] Success message appears
- [ ] Redirects after booking

#### 5. My Appointments Page
- [ ] Search form visible
- [ ] Can enter email address
- [ ] "Search" button works
- [ ] Appointments display after search
- [ ] "Cancel" button works
- [ ] Confirmation dialog appears

#### 6. Admin Panel
- [ ] Admin page loads
- [ ] "Add Time Slot" button works
- [ ] Form fields work correctly
- [ ] Can create new time slot
- [ ] Time slots table displays
- [ ] Filter radio buttons work
- [ ] Delete button works
- [ ] Statistics cards show correct numbers

---

### ✅ Integration Tests (Localhost)

#### 1. Complete Booking Flow
**Steps:**
1. Open frontend at `http://localhost:4200`
2. Go to Admin panel
3. Create a new time slot (tomorrow, 2:00 PM - 3:00 PM)
4. Go to Available Slots
5. Find the slot you created
6. Click "Book Now"
7. Fill in details:
   - Name: Test User
   - Email: test@example.com
   - Phone: 1234567890
   - Notes: Test booking
8. Click "Confirm Booking"
9. Go to My Appointments
10. Search for: test@example.com
11. Verify appointment appears

**Verification:**
- [ ] Slot created successfully
- [ ] Slot appears in available slots
- [ ] Booking succeeds
- [ ] Success message shows
- [ ] Slot disappears from available
- [ ] Appointment appears in search
- [ ] Data persists after page refresh

#### 2. Double Booking Prevention
**Steps:**
1. Note a booked slot ID
2. Try to book the same slot again (use Swagger or different browser)
3. Verify error message

**Verification:**
- [ ] Second booking fails
- [ ] Error message: "Time slot is not available"
- [ ] First booking remains intact

#### 3. Conflict Detection
**Steps:**
1. In Admin, create slot: Tomorrow 3:00 PM - 4:00 PM, Dr. Smith
2. Try to create: Tomorrow 3:30 PM - 4:30 PM, Dr. Smith
3. Verify error message

**Verification:**
- [ ] Second slot creation fails
- [ ] Error message about conflict
- [ ] First slot remains in system

#### 4. Cancel and Rebook
**Steps:**
1. Book a slot
2. Go to My Appointments
3. Cancel the booking
4. Go back to Available Slots
5. Verify slot is available again

**Verification:**
- [ ] Cancellation succeeds
- [ ] Slot becomes available
- [ ] Can rebook the same slot

---

## ☁️ Azure Deployment Verification

### ✅ Azure Resources Setup

#### 1. Resource Group
- [ ] Resource group created: `appointment-booking-rg`
- [ ] Correct region selected
- [ ] All resources in same group

#### 2. Storage Account
- [ ] Storage account created
- [ ] Name is globally unique
- [ ] LRS redundancy selected
- [ ] Tables created: `TimeSlots`, `Appointments`
- [ ] Connection string copied

#### 3. App Service
- [ ] App Service created
- [ ] Linux, .NET 10 runtime
- [ ] F1 (Free) pricing tier
- [ ] Status shows "Running"

#### 4. Environment Variables
- [ ] `AzureTableStorage__ConnectionString` set
- [ ] `ASPNETCORE_ENVIRONMENT` = `Production`
- [ ] `UseAzureStorage` = `true`
- [ ] App Service restarted after config

---

### ✅ Backend Deployment

#### 1. GitHub Actions
- [ ] Publish profile added to secrets
- [ ] Workflow file updated with correct app name
- [ ] Pushed to main branch
- [ ] GitHub Actions workflow triggered
- [ ] Build succeeded (green checkmark)
- [ ] Deployment succeeded

#### 2. API Verification
```bash
# Replace with your actual App Service URL
curl https://appointment-booking-api.azurewebsites.net/api/timeslots
```

**Tests:**
- [ ] Swagger accessible: `https://your-app.azurewebsites.net/swagger`
- [ ] API responds: `https://your-app.azurewebsites.net/api/timeslots`
- [ ] HTTPS redirects work
- [ ] No 500 errors
- [ ] Response time < 3 seconds

#### 3. Logs Check
- [ ] App Service logs show no errors
- [ ] Can see incoming requests
- [ ] Database queries executing
- [ ] No Azure storage errors

---

### ✅ Frontend Deployment

#### 1. Vercel Deployment
- [ ] `environment.prod.ts` updated with Azure API URL
- [ ] Committed and pushed
- [ ] Vercel deployment triggered
- [ ] Build succeeded
- [ ] Site is live

#### 2. Frontend Verification
- [ ] Can access Vercel URL
- [ ] Page loads correctly
- [ ] No console errors
- [ ] CSS/styles load properly
- [ ] Images load if any

---

### ✅ End-to-End Cloud Tests

#### 1. Basic Connectivity
- [ ] Frontend loads from Vercel/Azure
- [ ] Frontend can reach backend API
- [ ] No CORS errors in console
- [ ] API calls succeed (check Network tab)

#### 2. Complete Cloud Workflow
**Steps:**
1. Open production frontend URL
2. Navigate to Admin panel
3. Create a time slot
4. Verify it appears in Available Slots
5. Book an appointment
6. Search for it in My Appointments
7. Refresh the page
8. Verify data still there (Azure Table persistence)

**Verification:**
- [ ] Admin panel works on cloud
- [ ] Can create slots
- [ ] Slots persist in Azure Tables
- [ ] Can book appointments
- [ ] Appointments persist
- [ ] No errors in any step
- [ ] Performance is acceptable

#### 3. Data Persistence
- [ ] Created data visible in Azure Storage Browser
- [ ] Data survives app restart
- [ ] Multiple users can access simultaneously
- [ ] No data loss

#### 4. Table Storage Verification
**In Azure Portal:**
1. Go to Storage Account
2. Navigate to Tables
3. Click on `TimeSlots` table
4. Open Storage Browser

**Verification:**
- [ ] Can see table entities
- [ ] PartitionKey and RowKey set correctly
- [ ] Data matches frontend display
- [ ] Timestamps are correct

---

## 🔍 Performance Testing

### Localhost Performance
- [ ] Page load < 2 seconds
- [ ] API response < 500ms
- [ ] Smooth navigation
- [ ] No UI lag

### Cloud Performance
- [ ] Initial load < 5 seconds
- [ ] API response < 2 seconds
- [ ] Acceptable for free tier
- [ ] No timeout errors

---

## 🐛 Error Scenarios Testing

### Test These Error Cases:

#### 1. Invalid Input
- [ ] Try booking without name → Shows validation error
- [ ] Try booking without email → Shows validation error
- [ ] Try invalid email format → Shows validation error
- [ ] Try booking without phone → Shows validation error

#### 2. Network Errors
- [ ] Stop backend → Frontend shows error message
- [ ] Wrong API URL → Graceful error handling
- [ ] Slow connection → Loading indicators work

#### 3. Business Logic Errors
- [ ] Book non-existent slot → Error message
- [ ] Cancel non-existent appointment → Error message
- [ ] Create slot in past → Validation error
- [ ] Create slot with end before start → Validation error

---

## 📊 Final Production Checklist

### Before Going Live
- [ ] All localhost tests pass
- [ ] All Azure services running
- [ ] GitHub Actions show green
- [ ] Frontend deployed successfully
- [ ] Backend API accessible
- [ ] Swagger UI works
- [ ] End-to-end workflow tested
- [ ] Performance acceptable
- [ ] No console errors
- [ ] No security warnings
- [ ] HTTPS working everywhere

### Security Checklist
- [ ] No credentials in code
- [ ] Connection strings in Azure config only
- [ ] HTTPS everywhere
- [ ] CORS properly configured
- [ ] Input validation working
- [ ] Error messages don't expose internals

### Monitoring Setup
- [ ] Can access App Service logs
- [ ] Log stream working
- [ ] Metrics visible in Azure Portal
- [ ] Alerts configured (optional)

---

## 📈 Success Criteria

Your deployment is **SUCCESSFUL** when:

✅ Backend API accessible via HTTPS  
✅ Swagger UI working  
✅ Frontend accessible via HTTPS  
✅ Can create time slots from cloud admin panel  
✅ Can book appointments from cloud frontend  
✅ Data persists in Azure Table Storage  
✅ Can search and find booked appointments  
✅ Can cancel appointments  
✅ No CORS errors  
✅ No 500 errors  
✅ GitHub Actions builds are green  
✅ Performance is acceptable  

---

## 🎉 Congratulations!

If all items are checked, your Smart Appointment Booking System is:
- ✅ Fully functional on localhost
- ✅ Deployed to Azure cloud
- ✅ Using Azure Table Storage
- ✅ Accessible worldwide
- ✅ Production ready!

---

## 📞 If Something Fails

1. **Check the troubleshooting section** in `AZURE_DEPLOYMENT_GUIDE.md`
2. **Review App Service logs** in Azure Portal
3. **Check browser console** for frontend errors
4. **Verify environment variables** are set correctly
5. **Restart App Service** if needed
6. **Redeploy** via GitHub Actions if necessary

---

**Testing completed on:** _______________  
**Tested by:** _______________  
**All checks passed:** ☐ YES  ☐ NO  
**Production ready:** ☐ YES  ☐ NO  

---

**Next Steps After Successful Testing:**
1. Share production URLs with stakeholders
2. Monitor Azure costs (should be $0-2/month)
3. Set up monitoring and alerts
4. Plan for scaling if needed
5. Consider adding authentication for production use
