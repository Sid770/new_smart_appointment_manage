using AppointmentBookingAPI.Models;

namespace AppointmentBookingAPI.DTOs
{
    public class AppointmentDto
    {
        public int Id { get; set; }
        public int TimeSlotId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string ClientEmail { get; set; } = string.Empty;
        public string ClientPhone { get; set; } = string.Empty;
        public string ServiceType { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime BookedAt { get; set; }
        public TimeSlotDto? TimeSlot { get; set; }
    }
    
    public class CreateAppointmentDto
    {
        public int TimeSlotId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string ClientEmail { get; set; } = string.Empty;
        public string ClientPhone { get; set; } = string.Empty;
        public string ServiceType { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }
    
    public class UpdateAppointmentStatusDto
    {
        public AppointmentStatus Status { get; set; }
    }
}
