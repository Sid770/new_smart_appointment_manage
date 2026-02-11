using Azure;
using Azure.Data.Tables;
using AppointmentBookingAPI.Models;

namespace AppointmentBookingAPI.Models.Azure
{
    /// <summary>
    /// Azure Table Storage entity for TimeSlot
    /// PartitionKey: ServiceProvider
    /// RowKey: Id (GUID)
    /// </summary>
    public class TimeSlotTableEntity : ITableEntity
    {
        public string PartitionKey { get; set; } = default!;
        public string RowKey { get; set; } = default!;
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        // TimeSlot properties
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string ServiceProvider { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
        public DateTime CreatedAt { get; set; }

        public TimeSlotTableEntity()
        {
            PartitionKey = "DefaultProvider";
            RowKey = Guid.NewGuid().ToString();
        }

        public TimeSlotTableEntity(TimeSlot timeSlot)
        {
            PartitionKey = timeSlot.ServiceProvider ?? "DefaultProvider";
            RowKey = timeSlot.Id.ToString();
            StartTime = timeSlot.StartTime;
            EndTime = timeSlot.EndTime;
            ServiceProvider = timeSlot.ServiceProvider ?? "DefaultProvider";
            IsAvailable = timeSlot.IsAvailable;
            CreatedAt = timeSlot.CreatedAt;
        }

        public TimeSlot ToTimeSlot()
        {
            return new TimeSlot
            {
                Id = int.TryParse(RowKey, out var id) ? id : 0,
                StartTime = StartTime,
                EndTime = EndTime,
                ServiceProvider = ServiceProvider,
                IsAvailable = IsAvailable,
                CreatedAt = CreatedAt
            };
        }
    }
}
