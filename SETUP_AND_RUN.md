# Smart Appointment Booking System

## 📋 Overview

An enterprise-grade appointment booking system built with Angular and .NET Web API that prevents double booking and manages time slots efficiently.

### Core Features
- ✅ View available time slots
- ✅ Book appointments with conflict detection
- ✅ Prevent double booking
- ✅ Admin panel for slot management
- ✅ Real-time availability updates
- ✅ Professional enterprise UI/UX

### Tech Stack
- **Frontend**: Angular 18 (Standalone Components)
- **Backend**: .NET 8 Web API
- **Database**: In-Memory (for local development)
- **Architecture**: Clean Architecture with Repository + Service Pattern
- **API Documentation**: Swagger/OpenAPI

---

## 🏗️ Project Structure

```
hcl3/
├── AppointmentBookingAPI/              # .NET Web API Backend
│   ├── Controllers/                    # API Controllers
│   │   ├── AppointmentsController.cs
│   │   └── TimeSlotsController.cs
│   ├── Models/                         # Domain Models
│   │   ├── Appointment.cs
│   │   └── TimeSlot.cs
│   ├── DTOs/                           # Data Transfer Objects
│   │   ├── AppointmentDto.cs
│   │   ├── TimeSlotDto.cs
│   │   └── ApiResponse.cs
│   ├── Services/                       # Business Logic Layer
│   │   ├── IAppointmentService.cs
│   │   ├── AppointmentService.cs
│   │   ├── ITimeSlotService.cs
│   │   └── TimeSlotService.cs
│   ├── Repositories/                   # Data Access Layer
│   │   ├── IAppointmentRepository.cs
│   │   ├── AppointmentRepository.cs
│   │   ├── ITimeSlotRepository.cs
│   │   └── TimeSlotRepository.cs
│   ├── Data/                           # Database Context
│   │   └── ApplicationDbContext.cs
│   ├── Middleware/                     # Custom Middleware
│   │   ├── ExceptionHandlingMiddleware.cs
│   │   └── RequestLoggingMiddleware.cs
│   ├── Validators/                     # DTO Validators
│   │   └── DtoValidators.cs
│   └── Program.cs                      # Application Entry Point
│
├── AppointmentBookingAPI.Tests/        # Unit Tests
│   ├── AppointmentServiceTests.cs      # Service Tests
│   └── TimeSlotServiceTests.cs         # Conflict Detection Tests
│
└── src/                                # Angular Frontend
    └── app/
        ├── models/                     # TypeScript Models
        │   ├── appointment.model.ts
        │   ├── time-slot.model.ts
        │   └── api-response.model.ts
        ├── services/                   # API Services
        │   ├── appointment.service.ts
        │   └── time-slot.service.ts
        ├── pages/                      # Page Components
        │   ├── dashboard/
        │   ├── available-slots/
        │   ├── book-appointment/
        │   ├── my-appointments/
        │   └── admin/
        └── environments/               # Environment Config
            ├── environment.ts
            └── environment.prod.ts
```

---

## 🚀 Setup Instructions

### Prerequisites

1. **Node.js** (v18 or higher)
   - Download from: https://nodejs.org/

2. **.NET 8 SDK**
   - Download from: https://dotnet.microsoft.com/download

3. **Angular CLI**
   ```bash
   npm install -g @angular/cli
   ```

### Backend Setup

1. **Navigate to API Directory**
   ```bash
   cd "AppointmentBookingAPI"
   ```

2. **Restore NuGet Packages**
   ```bash
   dotnet restore
   ```

3. **Build the Project**
   ```bash
   dotnet build
   ```

4. **Run the API** (Default: http://localhost:5000)
   ```bash
   dotnet run
   ```

   The API will be available at:
   - HTTP: `http://localhost:5000`
   - Swagger UI: `http://localhost:5000/swagger`

### Frontend Setup

1. **Navigate to Project Root**
   ```bash
   cd ..
   ```

2. **Install Dependencies**
   ```bash
   npm install
   ```

3. **Run Development Server**
   ```bash
   npm start
   ```

   The Angular app will open at: `http://localhost:4200`

### Run Both Simultaneously

**Option 1: Using Two Terminals**

Terminal 1 (Backend):
```bash
cd AppointmentBookingAPI
dotnet run
```

Terminal 2 (Frontend):
```bash
npm start
```

**Option 2: Using VS Code Tasks**

Press `Ctrl+Shift+P` → Type "Tasks: Run Task" → Select "npm: start"

---

## 🧪 Running Tests

### Backend Unit Tests

```bash
cd AppointmentBookingAPI.Tests
dotnet test --verbosity normal
```

**Test Coverage:**
- ✅ Conflict detection for overlapping time slots
- ✅ Double booking prevention
- ✅ Appointment status transitions
- ✅ Time slot availability logic
- ✅ Booking workflow lifecycle

### Run Specific Test Class

```bash
dotnet test --filter "FullyQualifiedName~TimeSlotServiceTests"
```

---

## 📊 Sample Test Data

The system automatically seeds sample data on startup. Here's what's available:

### Sample Time Slots

| Date | Time | Provider | Status |
|------|------|----------|--------|
| Tomorrow | 09:00 - 10:00 | Dr. Sarah Smith | Available |
| Tomorrow | 10:00 - 11:00 | Dr. Sarah Smith | Available |
| Tomorrow | 14:00 - 15:00 | Dr. John Davis | Available |
| Tomorrow | 15:00 - 16:00 | Dr. John Davis | Available |
| Day After | 09:00 - 10:00 | Dr. Emily Johnson | Available |

### Sample Appointments

| Client | Date | Time | Status | Provider |
|--------|------|------|--------|----------|
| Alice Johnson | Tomorrow | 11:00-12:00 | Confirmed | Dr. Sarah Smith |
| Bob Williams | Day After | 10:00-11:00 | Confirmed | Dr. Emily Johnson |

---

## 🔍 API Endpoints

### Time Slots

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/timeslots` | Get all time slots |
| GET | `/api/timeslots/available` | Get available slots (with optional filters) |
| GET | `/api/timeslots/{id}` | Get slot by ID |
| POST | `/api/timeslots` | Create new time slot (Admin) |
| PUT | `/api/timeslots/{id}` | Update time slot (Admin) |
| DELETE | `/api/timeslots/{id}` | Delete time slot (Admin) |
| PUT | `/api/timeslots/{id}/make-available` | Free up booked slot (Admin) |

### Appointments

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/appointments` | Get all appointments |
| GET | `/api/appointments/{id}` | Get appointment by ID |
| GET | `/api/appointments/status/{status}` | Get appointments by status |
| POST | `/api/appointments` | Book new appointment |
| PUT | `/api/appointments/{id}/cancel` | Cancel appointment |
| PUT | `/api/appointments/{id}/status` | Update appointment status |

**Query Parameters for Available Slots:**
- `?date=2026-02-15` - Filter by specific date
- `?serviceProvider=Dr. Smith` - Filter by provider
- `?isAvailable=true` - Only show available slots

---

## 🖥️ Application Usage

### User Flow

1. **Dashboard** (`/dashboard`)
   - View system overview
   - Quick stats on available slots
   - Navigation to all features

2. **Browse Available Slots** (`/available-slots`)
   - Filter by date and provider
   - View real-time availability
   - Click "Book Now" to reserve

3. **Book Appointment** (`/book-appointment/:slotId`)
   - Fill in client details
   - Add optional notes
   - Confirm booking
   - System prevents double booking

4. **My Appointments** (`/my-appointments`)
   - Search appointments by email
   - View booking details
   - Cancel appointments
   - View booking status

5. **Admin Panel** (`/admin`)
   - Create new time slots
   - View all slots (available & booked)
   - Filter by status
   - Delete available slots
   - Make booked slots available again
   - View utilization statistics

---

## ✅ Verification Checklist

### Backend Verification

- [ ] API runs without errors on `http://localhost:5000`
- [ ] Swagger UI accessible at `/swagger`
- [ ] All endpoints return proper responses
- [ ] CORS is configured correctly
- [ ] Error handling middleware catches exceptions
- [ ] Request logging appears in console
- [ ] Sample data is seeded on startup

### Frontend Verification

- [ ] Angular app runs on `http://localhost:4200`
- [ ] Dashboard loads with statistics
- [ ] Available slots page shows time slots
- [ ] Booking form validates inputs
- [ ] Appointments can be searched by email
- [ ] Admin panel displays all slots
- [ ] UI is responsive on mobile devices
- [ ] Loading indicators show during API calls
- [ ] Error messages display when API fails

### Integration Verification

- [ ] Frontend successfully calls backend API
- [ ] Time slots display in Available Slots page
- [ ] Booking appointment updates slot availability
- [ ] Cannot double-book the same slot
- [ ] Cancelled appointments free up slots
- [ ] Admin can create new slots
- [ ] Real-time updates reflect across pages

### Conflict Detection Tests

- [ ] Cannot book unavailable slot
- [ ] Cannot create overlapping slots for same provider
- [ ] Can create same-time slots for different providers
- [ ] Cannot delete slot with active appointment
- [ ] Making slot available cancels appointment

---

## 🎨 UI Features

### Enterprise-Grade Design
- Clean, modern dashboard layout
- Professional color scheme (Blue/Purple gradient)
- Responsive mobile-first design
- Loading states and animations
- Success/Error toast notifications
- Status badges with color coding
- Smooth transitions and hover effects

### Status Badge Colors
- **Available**: Green
- **Booked**: Red
- **Confirmed**: Blue
- **Completed**: Purple
- **Cancelled**: Gray

---

## 🐛 Troubleshooting

### Backend Issues

**Port Already in Use**
```bash
# Find and kill process on port 5000 (PowerShell)
Get-Process -Id (Get-NetTCPConnection -LocalPort 5000).OwningProcess | Stop-Process
```

**Build Errors**
```bash
dotnet clean
dotnet restore
dotnet build
```

### Frontend Issues

**Port 4200 in Use**
```bash
# Kill Angular process
Get-Process -Name node | Where-Object {$_.Path -like "*node.exe"} | Stop-Process
```

**Module Not Found**
```bash
rm -rf node_modules package-lock.json
npm install
```

**API Connection Failed**
- Verify backend is running
- Check `environment.ts` API URL is correct
- Ensure CORS is configured in backend

---

## 📝 Sample API Requests

### Create Time Slot (POST /api/timeslots)
```json
{
  "startTime": "2026-02-15T09:00:00Z",
  "endTime": "2026-02-15T10:00:00Z",
  "serviceProvider": "Dr. Smith"
}
```

### Book Appointment (POST /api/appointments)
```json
{
  "timeSlotId": 1,
  "clientName": "John Doe",
  "clientEmail": "john@example.com",
  "clientPhone": "1234567890",
  "notes": "First visit"
}
```

### Update Status (PUT /api/appointments/1/status)
```json
{
  "status": "Completed"
}
```

---

## 🔐 Security Features

- Input validation on all DTOs
- SQL Injection protection (Entity Framework)
- Error messages don't expose sensitive data
- CORS configured for localhost development
- Request/Response logging for audit trail

---

## 📈 Performance Features

- In-memory database for fast local development
- Async/await throughout for scalability
- Lazy loading for Angular routes
- Standalone components for smaller bundles
- Efficient change detection with OnPush (where applicable)

---

## 🚧 Future Enhancements

- [ ] User authentication with JWT
- [ ] Email notifications for bookings
- [ ] SMS reminders
- [ ] Calendar integration
- [ ] Payment processing
- [ ] Multi-language support
- [ ] Dark mode
- [ ] Export appointments to PDF
- [ ] SQL Server/PostgreSQL support
- [ ] Azure/AWS deployment

---

## 📞 Support

For issues or questions:
1. Check the troubleshooting section
2. Verify all prerequisites are installed
3. Review console logs for errors
4. Check Swagger UI for API details

---

## 📄 License

This project is for educational and demonstration purposes.

---

**Built with ❤️ using Angular 18 and .NET 8**
