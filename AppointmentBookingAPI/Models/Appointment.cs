namespace AppointmentBookingAPI.Models
{
    /// <summary>
    /// Represents an appointment booking
    /// </summary>
    public class Appointment
    {
        public int Id { get; set; }
        
        public int TimeSlotId { get; set; }
        
        public string ClientName { get; set; } = string.Empty;
        
        public string ClientEmail { get; set; } = string.Empty;
        
        public string ClientPhone { get; set; } = string.Empty;
        
        public string ServiceType { get; set; } = string.Empty;
        
        public string Notes { get; set; } = string.Empty;
        
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
        
        public DateTime BookedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? ConfirmedAt { get; set; }
        
        public DateTime? CancelledAt { get; set; }
        
        // Navigation property
        public TimeSlot? TimeSlot { get; set; }
    }
    
    public enum AppointmentStatus
    {
        Pending,
        Confirmed,
        Cancelled,
        Completed
    }
}
