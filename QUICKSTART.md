# Quick Start Guide - Smart Appointment Booking System

## ⚡ Get Started in 5 Minutes

### Step 1: Start the Backend (Terminal 1)
```powershell
cd AppointmentBookingAPI
dotnet run
```
✅ Wait for: "Now listening on: http://localhost:5000"

### Step 2: Start the Frontend (Terminal 2)
```powershell
npm start
```
✅ Browser opens automatically at: http://localhost:4200

### Step 3: Explore the Application

1. **View Dashboard** - http://localhost:4200/dashboard
   - See system statistics
   - Quick navigation

2. **Browse Available Slots** - Click "Available Slots"
   - View time slots by date and provider
   - Click "Book Now" on any available slot

3. **Book an Appointment**
   - Fill in your details:
     - Name: Test User
     - Email: test@example.com
     - Phone: 1234567890
   - Click "Confirm Booking"
   - ✅ Booking confirmed!

4. **View Your Appointments** - Click "My Appointments"
   - Enter your email: test@example.com
   - Click "Search"
   - See all your bookings
   - Can cancel if needed

5. **Admin Panel** - Click "Admin"
   - **Create New Slot:**
     - Click "+ Add Time Slot"
     - Select tomorrow's date
     - Choose times (e.g., 16:00 - 17:00)
     - Click "Create Slot"
   - **Manage Slots:**
     - View all slots
     - Filter by status
     - Delete unused slots
     - Free up booked slots

---

## 🧪 Test the System

### Test Scenario 1: Prevent Double Booking
1. Go to Available Slots
2. Note a slot ID (e.g., Slot #1)
3. Book it with email: user1@test.com
4. Try to book the SAME slot again with email: user2@test.com
5. ✅ System prevents double booking with error message

### Test Scenario 2: Conflict Detection
1. Go to Admin Panel
2. Create a slot: Tomorrow 10:00 - 11:00, Provider: Dr. Smith
3. Try to create overlapping slot: Tomorrow 10:30 - 11:30, Provider: Dr. Smith
4. ✅ System shows conflict error

### Test Scenario 3: Cancel and Rebook
1. Book a slot with email: cancel@test.com
2. Go to My Appointments, search for: cancel@test.com
3. Click "Cancel" on your appointment
4. Go back to Available Slots
5. ✅ The slot is now available again

---

## 📊 Sample Test Data

The system comes with pre-loaded data:

**Available Time Slots:**
- Tomorrow 09:00-10:00 (Dr. Sarah Smith)
- Tomorrow 10:00-11:00 (Dr. Sarah Smith)
- Tomorrow 14:00-15:00 (Dr. John Davis)
- Tomorrow 15:00-16:00 (Dr. John Davis)

**Existing Appointments:**
Search for these emails to see booked appointments:
- alice.johnson@email.com
- bob.williams@email.com

---

## 🔍 API Testing (Swagger)

1. Open: http://localhost:5000/swagger
2. Try these endpoints:

**Get Available Slots:**
- Expand: `GET /api/timeslots/available`
- Click "Try it out"
- Click "Execute"
- ✅ See all available slots

**Book Appointment:**
- Expand: `POST /api/appointments`
- Click "Try it out"
- Use this JSON:
```json
{
  "timeSlotId": 1,
  "clientName": "Quick Test",
  "clientEmail": "quick@test.com",
  "clientPhone": "9999999999",
  "notes": "Testing from Swagger"
}
```
- Click "Execute"
- ✅ Booking created!

---

## ✅ Verification Checklist

Quick checks to ensure everything works:

- [ ] Backend URL works: http://localhost:5000/swagger
- [ ] Frontend loads: http://localhost:4200
- [ ] Dashboard shows statistics (4 slots available, etc.)
- [ ] Available Slots page displays time slots
- [ ] Can book an appointment successfully
- [ ] Booking prevents double-booking
- [ ] My Appointments search works
- [ ] Admin panel loads
- [ ] Can create new time slot in admin
- [ ] Slot conflicts are detected

---

## 🚨 Common Issues

**"Port 5000 already in use"**
```powershell
Get-Process -Id (Get-NetTCPConnection -LocalPort 5000).OwningProcess | Stop-Process
```

**"npm start" fails**
```powershell
npm install
npm start
```

**Frontend can't reach backend**
- Make sure backend is running (check Terminal 1)
- Backend should show: "Now listening on: http://localhost:5000"

**No data showing**
- Data is seeded automatically on backend startup
- If still no data, restart backend (Ctrl+C, then `dotnet run`)

---

## 🎯 Key Features to Try

1. **Real-time Availability** - Book a slot, refresh Available Slots page - it's gone!
2. **Search Appointments** - Enter any email to find bookings
3. **Admin Stats** - Check utilization percentage in Admin panel
4. **Filter Slots** - Use radio buttons in Admin to filter by status
5. **Responsive Design** - Resize browser window, works on mobile too!

---

## 📱 Mobile Testing

Open on your phone:
1. Find your computer's IP: `ipconfig` → IPv4 Address
2. Make sure phone is on same WiFi
3. Open: `http://YOUR-IP:4200`

---

## 🎉 You're All Set!

The system is ready for:
- Booking appointments
- Preventing conflicts
- Managing time slots
- Viewing statistics

For detailed documentation, see `SETUP_AND_RUN.md`

---

**Need Help?**
- Check browser console (F12) for errors
- Check backend terminal for API logs
- Review Swagger UI for API details
