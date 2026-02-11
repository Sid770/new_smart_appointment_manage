using AppointmentBookingAPI.DTOs;
using AppointmentBookingAPI.Models;
using AppointmentBookingAPI.Repositories;

namespace AppointmentBookingAPI.Services
{
    public class TimeSlotService : ITimeSlotService
    {
        private readonly ITimeSlotRepository _repository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly ILogger<TimeSlotService> _logger;
        
        public TimeSlotService(
            ITimeSlotRepository repository,
            IAppointmentRepository appointmentRepository,
            ILogger<TimeSlotService> logger)
        {
            _repository = repository;
            _appointmentRepository = appointmentRepository;
            _logger = logger;
        }
        
        public async Task<IEnumerable<TimeSlotDto>> GetAllTimeSlotsAsync()
        {
            var slots = await _repository.GetAllAsync();
            return slots.Select(MapToDto);
        }
        
        public async Task<IEnumerable<TimeSlotDto>> GetAvailableTimeSlotsAsync(TimeSlotFilterDto filter)
        {
            var slots = await _repository.GetAvailableAsync(filter.Date, filter.ServiceProvider);
            return slots.Select(MapToDto);
        }
        
        public async Task<TimeSlotDto?> GetTimeSlotByIdAsync(int id)
        {
            var slot = await _repository.GetByIdAsync(id);
            return slot != null ? MapToDto(slot) : null;
        }
        
        public async Task<TimeSlotDto> CreateTimeSlotAsync(CreateTimeSlotDto dto)
        {
            // Validate no conflicts
            if (await _repository.HasConflictAsync(dto.StartTime, dto.EndTime, dto.ServiceProvider))
            {
                throw new InvalidOperationException("Time slot conflicts with existing slot for this provider");
            }
            
            var timeSlot = new TimeSlot
            {
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                ServiceProvider = dto.ServiceProvider,
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow
            };
            
            var created = await _repository.CreateAsync(timeSlot);
            _logger.LogInformation("Created time slot {Id}", created.Id);
            return MapToDto(created);
        }
        
        public async Task<TimeSlotDto> UpdateTimeSlotAsync(int id, CreateTimeSlotDto dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
            {
                throw new KeyNotFoundException($"Time slot with ID {id} not found");
            }
            
            // Check for conflicts excluding current slot
            if (await _repository.HasConflictAsync(dto.StartTime, dto.EndTime, dto.ServiceProvider, id))
            {
                throw new InvalidOperationException("Time slot conflicts with existing slot for this provider");
            }
            
            existing.StartTime = dto.StartTime;
            existing.EndTime = dto.EndTime;
            existing.ServiceProvider = dto.ServiceProvider;
            
            var updated = await _repository.UpdateAsync(existing);
            return MapToDto(updated);
        }
        
        public async Task<bool> DeleteTimeSlotAsync(int id)
        {
            // Check if there are any appointments for this slot
            if (await _appointmentRepository.HasAppointmentForSlotAsync(id))
            {
                throw new InvalidOperationException("Cannot delete time slot with existing appointments");
            }
            
            return await _repository.DeleteAsync(id);
        }
        
        public async Task<bool> ValidateTimeSlotAsync(CreateTimeSlotDto dto, int? excludeId = null)
        {
            // Validate start time is before end time
            if (dto.StartTime >= dto.EndTime)
            {
                return false;
            }
            
            // Validate start time is in the future
            if (dto.StartTime <= DateTime.UtcNow)
            {
                return false;
            }
            
            // Check for conflicts
            if (await _repository.HasConflictAsync(dto.StartTime, dto.EndTime, dto.ServiceProvider, excludeId))
            {
                return false;
            }
            
            return true;
        }
        
        public async Task<bool> MakeSlotAvailableAsync(int id)
        {
            var slot = await _repository.GetByIdAsync(id);
            if (slot == null)
            {
                return false;
            }
            
            // Cancel any appointments for this slot
            var appointments = await _appointmentRepository.GetByTimeSlotAsync(id);
            foreach (var appointment in appointments)
            {
                appointment.Status = AppointmentStatus.Cancelled;
                await _appointmentRepository.UpdateAsync(appointment);
            }
            
            slot.IsAvailable = true;
            
            await _repository.UpdateAsync(slot);
            _logger.LogInformation("Made time slot {Id} available again", id);
            return true;
        }
        
        private TimeSlotDto MapToDto(TimeSlot slot)
        {
            return new TimeSlotDto
            {
                Id = slot.Id,
                StartTime = slot.StartTime,
                EndTime = slot.EndTime,
                ServiceProvider = slot.ServiceProvider,
                IsAvailable = slot.IsAvailable
            };
        }
    }
}
