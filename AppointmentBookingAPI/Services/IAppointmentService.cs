using AppointmentBookingAPI.DTOs;
using AppointmentBookingAPI.Models;

namespace AppointmentBookingAPI.Services
{
    public interface IAppointmentService
    {
        Task<IEnumerable<AppointmentDto>> GetAllAppointmentsAsync();
        Task<AppointmentDto?> GetAppointmentByIdAsync(int id);
        Task<IEnumerable<AppointmentDto>> GetAppointmentsByStatusAsync(AppointmentStatus status);
        Task<AppointmentDto> CreateAppointmentAsync(CreateAppointmentDto dto);
        Task<AppointmentDto> UpdateAppointmentStatusAsync(int id, UpdateAppointmentStatusDto dto);
        Task<bool> CancelAppointmentAsync(int id);
    }
}
