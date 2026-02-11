using AppointmentBookingAPI.Models;

namespace AppointmentBookingAPI.Repositories
{
    public interface ITimeSlotRepository
    {
        Task<IEnumerable<TimeSlot>> GetAllAsync();
        Task<IEnumerable<TimeSlot>> GetAvailableAsync(DateTime? date = null, string? serviceProvider = null);
        Task<TimeSlot?> GetByIdAsync(int id);
        Task<TimeSlot> CreateAsync(TimeSlot timeSlot);
        Task<TimeSlot> UpdateAsync(TimeSlot timeSlot);
        Task<bool> DeleteAsync(int id);
        Task<bool> IsSlotAvailableAsync(int slotId);
        Task<bool> HasConflictAsync(DateTime startTime, DateTime endTime, string serviceProvider, int? excludeId = null);
    }
}
