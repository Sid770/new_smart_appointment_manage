using AppointmentBookingAPI.Models;

namespace AppointmentBookingAPI.Repositories
{
    public interface IAppointmentRepository
    {
        Task<IEnumerable<Appointment>> GetAllAsync();
        Task<Appointment?> GetByIdAsync(int id);
        Task<IEnumerable<Appointment>> GetByStatusAsync(AppointmentStatus status);
        Task<IEnumerable<Appointment>> GetByTimeSlotAsync(int timeSlotId);
        Task<Appointment> CreateAsync(Appointment appointment);
        Task<Appointment> UpdateAsync(Appointment appointment);
        Task<bool> DeleteAsync(int id);
        Task<bool> HasAppointmentForSlotAsync(int timeSlotId);
    }
}
