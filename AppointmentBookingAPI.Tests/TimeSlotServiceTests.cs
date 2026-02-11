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
    /// Unit tests for TimeSlotService focusing on conflict detection and double booking prevention
    /// </summary>
    public class TimeSlotServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly TimeSlotRepository _repository;
        private readonly Mock<IAppointmentRepository> _mockAppointmentRepo;
        private readonly Mock<ILogger<TimeSlotService>> _mockLogger;
        private readonly TimeSlotService _service;

        public TimeSlotServiceTests()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            _context = new ApplicationDbContext(options);
            _repository = new TimeSlotRepository(_context, Mock.Of<ILogger<TimeSlotRepository>>());
            _mockAppointmentRepo = new Mock<IAppointmentRepository>();
            _mockLogger = new Mock<ILogger<TimeSlotService>>();
            
            _service = new TimeSlotService(_repository, _mockAppointmentRepo.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task CreateTimeSlot_WithValidData_ShouldSucceed()
        {
            // Arrange
            var dto = new CreateTimeSlotDto
            {
                StartTime = DateTime.UtcNow.AddHours(2),
                EndTime = DateTime.UtcNow.AddHours(3),
                ServiceProvider = "Dr. Smith"
            };

            // Act
            var result = await _service.CreateTimeSlotAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.ServiceProvider, result.ServiceProvider);
            Assert.True(result.IsAvailable);
        }

        [Fact]
        public async Task CreateTimeSlot_WithConflictingTime_ShouldThrowException()
        {
            // Arrange
            var startTime = DateTime.UtcNow.AddHours(2);
            var endTime = DateTime.UtcNow.AddHours(3);
            
            await _service.CreateTimeSlotAsync(new CreateTimeSlotDto
            {
                StartTime = startTime,
                EndTime = endTime,
                ServiceProvider = "Dr. Smith"
            });

            var conflictingDto = new CreateTimeSlotDto
            {
                StartTime = startTime.AddMinutes(30), // Overlaps with existing slot
                EndTime = endTime.AddMinutes(30),
                ServiceProvider = "Dr. Smith"
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _service.CreateTimeSlotAsync(conflictingDto));
        }

        [Fact]
        public async Task CreateTimeSlot_SameTimeForDifferentProviders_ShouldSucceed()
        {
            // Arrange
            var startTime = DateTime.UtcNow.AddHours(2);
            var endTime = DateTime.UtcNow.AddHours(3);
            
            var dto1 = new CreateTimeSlotDto
            {
                StartTime = startTime,
                EndTime = endTime,
                ServiceProvider = "Dr. Smith"
            };

            var dto2 = new CreateTimeSlotDto
            {
                StartTime = startTime,
                EndTime = endTime,
                ServiceProvider = "Dr. Jones"
            };

            // Act
            var result1 = await _service.CreateTimeSlotAsync(dto1);
            var result2 = await _service.CreateTimeSlotAsync(dto2);

            // Assert
            Assert.NotNull(result1);
            Assert.NotNull(result2);
            Assert.NotEqual(result1.Id, result2.Id);
        }

        [Fact]
        public async Task ValidateTimeSlot_StartTimeAfterEndTime_ShouldReturnFalse()
        {
            // Arrange
            var dto = new CreateTimeSlotDto
            {
                StartTime = DateTime.UtcNow.AddHours(3),
                EndTime = DateTime.UtcNow.AddHours(2), // Invalid: end before start
                ServiceProvider = "Dr. Smith"
            };

            // Act
            var result = await _service.ValidateTimeSlotAsync(dto);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ValidateTimeSlot_StartTimeInPast_ShouldReturnFalse()
        {
            // Arrange
            var dto = new CreateTimeSlotDto
            {
                StartTime = DateTime.UtcNow.AddHours(-1), // In the past
                EndTime = DateTime.UtcNow.AddHours(1),
                ServiceProvider = "Dr. Smith"
            };

            // Act
            var result = await _service.ValidateTimeSlotAsync(dto);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteTimeSlot_WithNoAppointments_ShouldSucceed()
        {
            // Arrange
            var dto = new CreateTimeSlotDto
            {
                StartTime = DateTime.UtcNow.AddHours(2),
                EndTime = DateTime.UtcNow.AddHours(3),
                ServiceProvider = "Dr. Smith"
            };
            var created = await _service.CreateTimeSlotAsync(dto);

            _mockAppointmentRepo.Setup(x => x.HasAppointmentForSlotAsync(created.Id))
                .ReturnsAsync(false);

            // Act
            var result = await _service.DeleteTimeSlotAsync(created.Id);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteTimeSlot_WithExistingAppointments_ShouldThrowException()
        {
            // Arrange
            var dto = new CreateTimeSlotDto
            {
                StartTime = DateTime.UtcNow.AddHours(2),
                EndTime = DateTime.UtcNow.AddHours(3),
                ServiceProvider = "Dr. Smith"
            };
            var created = await _service.CreateTimeSlotAsync(dto);

            _mockAppointmentRepo.Setup(x => x.HasAppointmentForSlotAsync(created.Id))
                .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _service.DeleteTimeSlotAsync(created.Id));
        }

        [Fact]
        public async Task MakeSlotAvailable_ShouldCancelAppointmentsAndFreeSlot()
        {
            // Arrange
            var slot = new TimeSlot
            {
                StartTime = DateTime.UtcNow.AddHours(2),
                EndTime = DateTime.UtcNow.AddHours(3),
                ServiceProvider = "Dr. Smith",
                IsAvailable = false,
                BookedByName = "John Doe",
                BookedByEmail = "john@example.com"
            };
            _context.TimeSlots.Add(slot);
            await _context.SaveChangesAsync();

            var appointment = new Appointment
            {
                TimeSlotId = slot.Id,
                ClientName = "John Doe",
                ClientEmail = "john@example.com",
                Status = AppointmentStatus.Confirmed
            };

            _mockAppointmentRepo.Setup(x => x.GetByTimeSlotAsync(slot.Id))
                .ReturnsAsync(new[] { appointment });

            // Act
            var result = await _service.MakeSlotAvailableAsync(slot.Id);

            // Assert
            Assert.True(result);
            var updatedSlot = await _context.TimeSlots.FindAsync(slot.Id);
            Assert.NotNull(updatedSlot);
            Assert.True(updatedSlot.IsAvailable);
            Assert.Null(updatedSlot.BookedByName);
        }

        [Fact]
        public async Task GetAvailableTimeSlots_ShouldOnlyReturnAvailableSlots()
        {
            // Arrange
            var now = DateTime.UtcNow;
            _context.TimeSlots.AddRange(
                new TimeSlot { StartTime = now.AddHours(1), EndTime = now.AddHours(2), IsAvailable = true },
                new TimeSlot { StartTime = now.AddHours(3), EndTime = now.AddHours(4), IsAvailable = false },
                new TimeSlot { StartTime = now.AddHours(5), EndTime = now.AddHours(6), IsAvailable = true }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAvailableTimeSlotsAsync(new TimeSlotFilterDto());

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, slot => Assert.True(slot.IsAvailable));
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
