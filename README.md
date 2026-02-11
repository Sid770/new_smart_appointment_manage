# Smart Appointment Booking System

An enterprise-grade appointment booking system that prevents double booking and manages schedules efficiently.

## 🚀 Quick Start

```bash
# Terminal 1: Start Backend
cd AppointmentBookingAPI
dotnet run

# Terminal 2: Start Frontend
npm start
```

✅ **Backend:** http://localhost:5000  
✅ **Frontend:** http://localhost:4200  
✅ **Swagger:** http://localhost:5000/swagger

## 📋 Features

### Core Functionality
- ✅ View available time slots with real-time updates
- ✅ Book appointments with conflict detection
- ✅ Prevent double booking automatically
- ✅ Search and manage your appointments
- ✅ Admin panel for slot management
- ✅ Professional enterprise UI/UX

### Technical Features
- Clean Architecture (Repository + Service Pattern)
- SOLID principles
- Comprehensive unit tests
- Input validation
- Error handling middleware
- Request logging
- Swagger API documentation
- CORS configuration

## 🏗️ Architecture

**Frontend:** Angular 18 (Standalone Components)  
**Backend:** .NET 8 Web API  
**Database:** In-Memory (for local development)  
**Testing:** XUnit with Moq  
**UI Framework:** Custom CSS with modern design

## 📁 Project Structure

```
hcl3/
├── AppointmentBookingAPI/          # .NET Web API
│   ├── Controllers/                # API endpoints
│   ├── Services/                   # Business logic
│   ├── Repositories/               # Data access
│   ├── Models/                     # Domain models
│   ├── DTOs/                       # Data transfer objects
│   ├── Middleware/                 # Custom middleware
│   └── Data/                       # Database context
│
├── AppointmentBookingAPI.Tests/    # Unit tests
│   ├── AppointmentServiceTests.cs  # Booking workflow tests
│   └── TimeSlotServiceTests.cs     # Conflict detection tests
│
└── src/app/                        # Angular Frontend
    ├── models/                     # TypeScript models
    ├── services/                   # API services
    ├── pages/                      # Page components
    │   ├── dashboard/
    │   ├── available-slots/
    │   ├── book-appointment/
    │   ├── my-appointments/
    │   └── admin/
    └── environments/               # Environment config
```

## 🧪 Testing

```bash
# Run backend unit tests
cd AppointmentBookingAPI.Tests
dotnet test

# Test coverage includes:
# - Double booking prevention
# - Time slot conflict detection
# - Appointment status transitions
# - Booking workflow lifecycle
```

## 📖 Documentation

- **[QUICKSTART.md](QUICKSTART.md)** - 5-minute setup guide
- **[SETUP_AND_RUN.md](SETUP_AND_RUN.md)** - Complete documentation

## 🎯 Key Highlights

1. **Prevents Double Booking** - Automatic conflict detection
2. **Real-time Updates** - Instant availability changes
3. **Admin Dashboard** - Full slot management with statistics
4. **Search & Filter** - Find appointments by email, filter by status
5. **Responsive Design** - Works on desktop, tablet, and mobile
6. **Enterprise UI** - Professional SaaS-style interface

## 🔍 API Endpoints

### Time Slots
- `GET /api/timeslots` - Get all time slots
- `GET /api/timeslots/available` - Get available slots
- `POST /api/timeslots` - Create slot (Admin)
- `DELETE /api/timeslots/{id}` - Delete slot (Admin)

### Appointments
- `GET /api/appointments` - Get all appointments
- `POST /api/appointments` - Book appointment
- `PUT /api/appointments/{id}/cancel` - Cancel appointment
- `PUT /api/appointments/{id}/status` - Update status

## 📊 Sample Data

The system includes pre-loaded test data:
- 5+ available time slots (tomorrow and day after)
- 2 sample booked appointments
- Multiple service providers

Search for these emails to see sample appointments:
- alice.johnson@email.com
- bob.williams@email.com

## ✅ Verification

Quick checks:
- [ ] Backend runs on http://localhost:5000
- [ ] Frontend runs on http://localhost:4200
- [ ] Swagger UI accessible at /swagger
- [ ] Can view available slots
- [ ] Can book appointments
- [ ] Double booking is prevented
- [ ] Admin panel works
- [ ] Unit tests pass

## 🛠️ Tech Stack Details

**Frontend:**
- Angular 18 (Latest Stable)
- Standalone Components
- Signals & Modern APIs
- TypeScript 5+
- Reactive Forms
- HttpClient
- Router with Lazy Loading

**Backend:**
- .NET 8 Web API
- Entity Framework Core
- In-Memory Database
- Dependency Injection
- Middleware Pipeline
- Swagger/OpenAPI
- CORS Support

**Testing:**
- XUnit
- Moq (Mocking Framework)
- In-Memory Database for Tests

## 🚧 Troubleshooting

**Port conflicts:**
```powershell
# PowerShell - Kill process on port 5000
Get-Process -Id (Get-NetTCPConnection -LocalPort 5000).OwningProcess | Stop-Process
```

**Module errors:**
```bash
rm -rf node_modules package-lock.json
npm install
```

## 📝 License

Educational and demonstration purposes.

---

**Built with ❤️ for HCL Hackathon - Problem #3: Smart Appointment Booking System**
