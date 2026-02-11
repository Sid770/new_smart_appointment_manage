namespace AppointmentBookingAPI.Models
{
    /// <summary>
    /// Represents a time slot for appointments
    /// </summary>
    public class TimeSlot
    {
        public int Id { get; set; }
        
        public DateTime StartTime { get; set; }
        
        public DateTime EndTime { get; set; }
        
        public string ServiceProvider { get; set; } = string.Empty;
        
        public bool IsAvailable { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
