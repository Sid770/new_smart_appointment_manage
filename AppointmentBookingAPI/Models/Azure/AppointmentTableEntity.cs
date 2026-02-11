using Azure;
using Azure.Data.Tables;
using AppointmentBookingAPI.Models;

namespace AppointmentBookingAPI.Models.Azure
{
    /// <summary>
    /// Azure Table Storage entity for Appointment
    /// PartitionKey: ClientEmail
    /// RowKey: Id (GUID)
    /// </summary>
    public class AppointmentTableEntity : ITableEntity
    {
        public string PartitionKey { get; set; } = default!;
        public string RowKey { get; set; } = default!;
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        // Appointment properties
        public int TimeSlotId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string ClientEmail { get; set; } = string.Empty;
        public string ClientPhone { get; set; } = string.Empty;
        public string ServiceType { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime BookedAt { get; set; }

        public AppointmentTableEntity()
        {
            PartitionKey = "DefaultClient";
            RowKey = Guid.NewGuid().ToString();
        }

        public AppointmentTableEntity(Appointment appointment)
        {
            PartitionKey = appointment.ClientEmail ?? "DefaultClient";
            RowKey = appointment.Id.ToString();
            TimeSlotId = appointment.TimeSlotId;
            ClientName = appointment.ClientName;
            ClientEmail = appointment.ClientEmail;
            ClientPhone = appointment.ClientPhone ?? string.Empty;
            ServiceType = appointment.ServiceType ?? string.Empty;
            Notes = appointment.Notes ?? string.Empty;
            Status = appointment.Status.ToString();
            BookedAt = appointment.BookedAt;
        }

        public Appointment ToAppointment()
        {
            return new Appointment
            {
                Id = int.TryParse(RowKey, out var id) ? id : 0,
                TimeSlotId = TimeSlotId,
                ClientName = ClientName,
                ClientEmail = ClientEmail,
                ClientPhone = ClientPhone,
                ServiceType = ServiceType,
                Notes = Notes,
                Status = Enum.TryParse<AppointmentStatus>(Status, out var status) 
                    ? status 
                    : AppointmentStatus.Pending,
                BookedAt = BookedAt
            };
        }
    }
}
