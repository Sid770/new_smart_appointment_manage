using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using AppointmentBookingAPI.Services;
using AppointmentBookingAPI.Repositories;
using AppointmentBookingAPI.Models;
using AppointmentBookingAPI.DTOs;
using AppointmentBookingAPI.Data;

namespace AppointmentBookingAPI.Tests
{
    /// <summary>
    /// Unit tests for AppointmentService focusing on booking workflow and status transitions
    /// </summary>
    public class AppointmentServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly AppointmentRepository _repository;
        private readonly Mock<ITimeSlotRepository> _mockTimeSlotRepo;
        private readonly Mock<ILogger<AppointmentService>> _mockLogger;
        private readonly AppointmentService _service;

        public AppointmentServiceTests()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            _context = new ApplicationDbContext(options);
            _repository = new AppointmentRepository(_context, Mock.Of<ILogger<AppointmentRepository>>());
            _mockTimeSlotRepo = new Mock<ITimeSlotRepository>();
            _mockLogger = new Mock<ILogger<AppointmentService>>();
            
            _service = new AppointmentService(_repository, _mockTimeSlotRepo.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task BookAppointment_WithAvailableSlot_ShouldSucceed()
        {
            // Arrange
            var timeSlot = new TimeSlot
            {
                Id = 1,
                StartTime = DateTime.UtcNow.AddHours(2),
                EndTime = DateTime.UtcNow.AddHours(3),
                IsAvailable = true,
                ServiceProvider = "Dr. Smith"
            };

            _mockTimeSlotRepo.Setup(x => x.GetByIdAsync(timeSlot.Id))
                .ReturnsAsync(timeSlot);

            var dto = new CreateAppointmentDto
            {
                TimeSlotId = timeSlot.Id,
                ClientName = "John Doe",
                ClientEmail = "john@example.com",
                ClientPhone = "1234567890",
                Notes = "Test booking"
            };

            // Act
            var result = await _service.BookAppointmentAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.ClientName, result.ClientName);
            Assert.Equal("Confirmed", result.Status);
            Assert.False(timeSlot.IsAvailable); // Slot should be marked as unavailable
        }

        [Fact]
        public async Task BookAppointment_WithUnavailableSlot_ShouldThrowException()
        {
            // Arrange
            var timeSlot = new TimeSlot
            {
                Id = 1,
                StartTime = DateTime.UtcNow.AddHours(2),
                EndTime = DateTime.UtcNow.AddHours(3),
                IsAvailable = false, // Already booked
                ServiceProvider = "Dr. Smith"
            };

            _mockTimeSlotRepo.Setup(x => x.GetByIdAsync(timeSlot.Id))
                .ReturnsAsync(timeSlot);

            var dto = new CreateAppointmentDto
            {
                TimeSlotId = timeSlot.Id,
                ClientName = "John Doe",
                ClientEmail = "john@example.com",
                ClientPhone = "1234567890"
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _service.BookAppointmentAsync(dto));
            
            Assert.Contains("not available", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task BookAppointment_WithNonexistentSlot_ShouldThrowException()
        {
            // Arrange
            _mockTimeSlotRepo.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((TimeSlot?)null);

            var dto = new CreateAppointmentDto
            {
                TimeSlotId = 999,
                ClientName = "John Doe",
                ClientEmail = "john@example.com",
                ClientPhone = "1234567890"
            };

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => 
                _service.BookAppointmentAsync(dto));
        }

        [Fact]
        public async Task CancelAppointment_ShouldUpdateStatusAndFreeSlot()
        {
            // Arrange
            var timeSlot = new TimeSlot
            {
                Id = 1,
                StartTime = DateTime.UtcNow.AddHours(2),
                EndTime = DateTime.UtcNow.AddHours(3),
                IsAvailable = false,
                ServiceProvider = "Dr. Smith"
            };
            _context.TimeSlots.Add(timeSlot);

            var appointment = new Appointment
            {
                TimeSlotId = timeSlot.Id,
                ClientName = "John Doe",
                ClientEmail = "john@example.com",
                Status = AppointmentStatus.Confirmed,
                BookedAt = DateTime.UtcNow,
                TimeSlot = timeSlot
            };
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            _mockTimeSlotRepo.Setup(x => x.GetByIdAsync(timeSlot.Id))
                .ReturnsAsync(timeSlot);

            // Act
            var result = await _service.CancelAppointmentAsync(appointment.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Cancelled", result.Status);
            Assert.True(timeSlot.IsAvailable); // Slot should be freed
        }

        [Fact]
        public async Task UpdateAppointmentStatus_FromPendingToConfirmed_ShouldSucceed()
        {
            // Arrange
            var appointment = new Appointment
            {
                TimeSlotId = 1,
                ClientName = "John Doe",
                ClientEmail = "john@example.com",
                Status = AppointmentStatus.Pending,
                BookedAt = DateTime.UtcNow
            };
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.UpdateAppointmentStatusAsync(appointment.Id, AppointmentStatus.Confirmed);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(AppointmentStatus.Confirmed, result.Status);
        }

        [Fact]
        public async Task UpdateAppointmentStatus_FromConfirmedToCompleted_ShouldSucceed()
        {
            // Arrange
            var appointment = new Appointment
            {
                TimeSlotId = 1,
                ClientName = "John Doe",
                ClientEmail = "john@example.com",
                Status = AppointmentStatus.Confirmed,
                BookedAt = DateTime.UtcNow
            };
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.UpdateAppointmentStatusAsync(appointment.Id, AppointmentStatus.Completed);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(AppointmentStatus.Completed, result.Status);
        }

        [Fact]
        public async Task GetAppointmentsByStatus_ShouldReturnOnlyMatchingStatus()
        {
            // Arrange
            _context.Appointments.AddRange(
                new Appointment { ClientName = "John", Status = AppointmentStatus.Confirmed, BookedAt = DateTime.UtcNow },
                new Appointment { ClientName = "Jane", Status = AppointmentStatus.Pending, BookedAt = DateTime.UtcNow },
                new Appointment { ClientName = "Bob", Status = AppointmentStatus.Confirmed, BookedAt = DateTime.UtcNow }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAppointmentsByStatusAsync(AppointmentStatus.Confirmed);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, apt => Assert.Equal("Confirmed", apt.Status));
        }

        [Fact]
        public async Task PreventDoubleBooking_SimultaneousRequests_ShouldOnlyAllowOne()
        {
            // Arrange
            var timeSlot = new TimeSlot
            {
                Id = 1,
                StartTime = DateTime.UtcNow.AddHours(2),
                EndTime = DateTime.UtcNow.AddHours(3),
                IsAvailable = true,
                ServiceProvider = "Dr. Smith"
            };

            _mockTimeSlotRepo.Setup(x => x.GetByIdAsync(timeSlot.Id))
                .ReturnsAsync(timeSlot);

            var dto1 = new CreateAppointmentDto
            {
                TimeSlotId = timeSlot.Id,
                ClientName = "Client 1",
                ClientEmail = "client1@example.com",
                ClientPhone = "1111111111"
            };

            var dto2 = new CreateAppointmentDto
            {
                TimeSlotId = timeSlot.Id,
                ClientName = "Client 2",
                ClientEmail = "client2@example.com",
                ClientPhone = "2222222222"
            };

            // Act
            var result1 = await _service.BookAppointmentAsync(dto1);
            
            // The slot should now be unavailable
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _service.BookAppointmentAsync(dto2));

            // Assert
            Assert.NotNull(result1);
            Assert.Contains("not available", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task BookingWorkflow_CompleteLifecycle_ShouldTransitionCorrectly()
        {
            // Arrange
            var timeSlot = new TimeSlot
            {
                Id = 1,
                StartTime = DateTime.UtcNow.AddHours(2),
                EndTime = DateTime.UtcNow.AddHours(3),
                IsAvailable = true,
                ServiceProvider = "Dr. Smith"
            };
            _context.TimeSlots.Add(timeSlot);
            await _context.SaveChangesAsync();

            _mockTimeSlotRepo.Setup(x => x.GetByIdAsync(timeSlot.Id))
                .ReturnsAsync(timeSlot);

            var dto = new CreateAppointmentDto
            {
                TimeSlotId = timeSlot.Id,
                ClientName = "John Doe",
                ClientEmail = "john@example.com",
                ClientPhone = "1234567890"
            };

            // Act & Assert - Step 1: Book appointment (Pending -> Confirmed)
            var booked = await _service.BookAppointmentAsync(dto);
            Assert.Equal("Confirmed", booked.Status);
            Assert.False(timeSlot.IsAvailable);

            // Step 2: Complete appointment
            var completed = await _service.UpdateAppointmentStatusAsync(booked.Id, AppointmentStatus.Completed);
            Assert.Equal("Completed", completed.Status);

            // Verify the full lifecycle
            var finalAppointment = await _service.GetAppointmentByIdAsync(booked.Id);
            Assert.NotNull(finalAppointment);
            Assert.Equal("Completed", finalAppointment.Status);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
