namespace AppointmentBookingAPI.DTOs
{
    public class TimeSlotDto
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string ServiceProvider { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
    }
    
    public class CreateTimeSlotDto
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string ServiceProvider { get; set; } = string.Empty;
    }
    
    public class TimeSlotFilterDto
    {
        public DateTime? Date { get; set; }
        public string? ServiceProvider { get; set; }
        public bool? IsAvailable { get; set; }
    }
}
