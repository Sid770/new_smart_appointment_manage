using AppointmentBookingAPI.DTOs;
using AppointmentBookingAPI.Models;

namespace AppointmentBookingAPI.Services
{
    public interface ITimeSlotService
    {
        Task<IEnumerable<TimeSlotDto>> GetAllTimeSlotsAsync();
        Task<IEnumerable<TimeSlotDto>> GetAvailableTimeSlotsAsync(TimeSlotFilterDto filter);
        Task<TimeSlotDto?> GetTimeSlotByIdAsync(int id);
        Task<TimeSlotDto> CreateTimeSlotAsync(CreateTimeSlotDto dto);
        Task<TimeSlotDto> UpdateTimeSlotAsync(int id, CreateTimeSlotDto dto);
        Task<bool> DeleteTimeSlotAsync(int id);
        Task<bool> ValidateTimeSlotAsync(CreateTimeSlotDto dto, int? excludeId = null);
        Task<bool> MakeSlotAvailableAsync(int id);
    }
}
