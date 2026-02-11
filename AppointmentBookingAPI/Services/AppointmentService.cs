using AppointmentBookingAPI.DTOs;
using AppointmentBookingAPI.Models;
using AppointmentBookingAPI.Repositories;

namespace AppointmentBookingAPI.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly ITimeSlotRepository _timeSlotRepository;
        private readonly ILogger<AppointmentService> _logger;
        
        public AppointmentService(
            IAppointmentRepository appointmentRepository,
            ITimeSlotRepository timeSlotRepository,
            ILogger<AppointmentService> logger)
        {
            _appointmentRepository = appointmentRepository;
            _timeSlotRepository = timeSlotRepository;
            _logger = logger;
        }
        
        public async Task<IEnumerable<AppointmentDto>> GetAllAppointmentsAsync()
        {
            var appointments = await _appointmentRepository.GetAllAsync();
            return appointments.Select(MapToDto);
        }
        
        public async Task<AppointmentDto?> GetAppointmentByIdAsync(int id)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);
            return appointment != null ? MapToDto(appointment) : null;
        }
        
        public async Task<IEnumerable<AppointmentDto>> GetAppointmentsByStatusAsync(AppointmentStatus status)
        {
            var appointments = await _appointmentRepository.GetByStatusAsync(status);
            return appointments.Select(MapToDto);
        }
        
        public async Task<AppointmentDto> CreateAppointmentAsync(CreateAppointmentDto dto)
        {
            // Validate time slot availability
            if (!await _timeSlotRepository.IsSlotAvailableAsync(dto.TimeSlotId))
            {
                throw new InvalidOperationException("Time slot is not available or does not exist");
            }
            
            // Check for double booking
            if (await _appointmentRepository.HasAppointmentForSlotAsync(dto.TimeSlotId))
            {
                throw new InvalidOperationException("Time slot is already booked. Cannot create duplicate appointment.");
            }
            
            var appointment = new Appointment
            {
                TimeSlotId = dto.TimeSlotId,
                ClientName = dto.ClientName,
                ClientEmail = dto.ClientEmail,
                ClientPhone = dto.ClientPhone,
                ServiceType = dto.ServiceType,
                Notes = dto.Notes,
                Status = AppointmentStatus.Pending,
                BookedAt = DateTime.UtcNow
            };
            
            // Mark time slot as unavailable
            var timeSlot = await _timeSlotRepository.GetByIdAsync(dto.TimeSlotId);
            if (timeSlot != null)
            {
                timeSlot.IsAvailable = false;
                await _timeSlotRepository.UpdateAsync(timeSlot);
            }
            
            var created = await _appointmentRepository.CreateAsync(appointment);
            _logger.LogInformation("Created appointment {Id} for client {Client}", created.Id, dto.ClientName);
            
            return MapToDto(created);
        }
        
        public async Task<AppointmentDto> UpdateAppointmentStatusAsync(int id, UpdateAppointmentStatusDto dto)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);
            if (appointment == null)
            {
                throw new KeyNotFoundException($"Appointment with ID {id} not found");
            }
            
            appointment.Status = dto.Status;
            
            if (dto.Status == AppointmentStatus.Confirmed)
            {
                appointment.ConfirmedAt = DateTime.UtcNow;
            }
            else if (dto.Status == AppointmentStatus.Cancelled)
            {
                appointment.CancelledAt = DateTime.UtcNow;
                
                // Make time slot available again
                var timeSlot = await _timeSlotRepository.GetByIdAsync(appointment.TimeSlotId);
                if (timeSlot != null)
                {
                    timeSlot.IsAvailable = true;
                    await _timeSlotRepository.UpdateAsync(timeSlot);
                }
            }
            
            var updated = await _appointmentRepository.UpdateAsync(appointment);
            _logger.LogInformation("Updated appointment {Id} to status {Status}", id, dto.Status);
            
            return MapToDto(updated);
        }
        
        public async Task<bool> CancelAppointmentAsync(int id)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);
            if (appointment == null)
            {
                return false;
            }
            
            appointment.Status = AppointmentStatus.Cancelled;
            appointment.CancelledAt = DateTime.UtcNow;
            
            // Make time slot available again
            var timeSlot = await _timeSlotRepository.GetByIdAsync(appointment.TimeSlotId);
            if (timeSlot != null)
            {
                timeSlot.IsAvailable = true;
                await _timeSlotRepository.UpdateAsync(timeSlot);
            }
            
            await _appointmentRepository.UpdateAsync(appointment);
            _logger.LogInformation("Cancelled appointment {Id}", id);
            
            return true;
        }
        
        private AppointmentDto MapToDto(Appointment appointment)
        {
            return new AppointmentDto
            {
                Id = appointment.Id,
                TimeSlotId = appointment.TimeSlotId,
                ClientName = appointment.ClientName,
                ClientEmail = appointment.ClientEmail,
                ClientPhone = appointment.ClientPhone,
                ServiceType = appointment.ServiceType,
                Notes = appointment.Notes,
                Status = appointment.Status.ToString(),
                BookedAt = appointment.BookedAt,
                TimeSlot = appointment.TimeSlot != null ? new TimeSlotDto
                {
                    Id = appointment.TimeSlot.Id,
                    StartTime = appointment.TimeSlot.StartTime,
                    EndTime = appointment.TimeSlot.EndTime,
                    ServiceProvider = appointment.TimeSlot.ServiceProvider,
                    IsAvailable = appointment.TimeSlot.IsAvailable
                } : null
            };
        }
    }
}
