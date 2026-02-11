using Azure.Data.Tables;
using AppointmentBookingAPI.Models;
using AppointmentBookingAPI.Models.Azure;

namespace AppointmentBookingAPI.Repositories.Azure
{
    /// <summary>
    /// Azure Table Storage repository for Appointment
    /// </summary>
    public class AzureAppointmentRepository : IAppointmentRepository
    {
        private readonly TableClient _tableClient;
        private readonly ILogger<AzureAppointmentRepository> _logger;
        private const string TableName = "Appointments";

        public AzureAppointmentRepository(
            TableServiceClient tableServiceClient,
            ILogger<AzureAppointmentRepository> logger)
        {
            _tableClient = tableServiceClient.GetTableClient(TableName);
            _tableClient.CreateIfNotExists();
            _logger = logger;
        }

        public async Task<IEnumerable<Appointment>> GetAllAsync()
        {
            var entities = new List<Appointment>();
            
            await foreach (var entity in _tableClient.QueryAsync<AppointmentTableEntity>())
            {
                entities.Add(entity.ToAppointment());
            }

            return entities.OrderByDescending(a => a.BookedAt);
        }

        public async Task<Appointment?> GetByIdAsync(int id)
        {
            var query = _tableClient.QueryAsync<AppointmentTableEntity>(
                filter: $"RowKey eq '{id}'");

            await foreach (var entity in query)
            {
                return entity.ToAppointment();
            }

            return null;
        }

        public async Task<IEnumerable<Appointment>> GetByStatusAsync(AppointmentStatus status)
        {
            var filter = $"Status eq '{status}'";
            var entities = new List<Appointment>();

            await foreach (var entity in _tableClient.QueryAsync<AppointmentTableEntity>(filter: filter))
            {
                entities.Add(entity.ToAppointment());
            }

            return entities.OrderByDescending(a => a.BookedAt);
        }

        public async Task<IEnumerable<Appointment>> GetByTimeSlotAsync(int timeSlotId)
        {
            var filter = $"TimeSlotId eq {timeSlotId}";
            var entities = new List<Appointment>();

            await foreach (var entity in _tableClient.QueryAsync<AppointmentTableEntity>(filter: filter))
            {
                entities.Add(entity.ToAppointment());
            }

            return entities;
        }

        public async Task<Appointment> CreateAsync(Appointment appointment)
        {
            // Generate a unique ID if not set
            if (appointment.Id == 0)
            {
                appointment.Id = Math.Abs(Guid.NewGuid().GetHashCode());
            }

            var entity = new AppointmentTableEntity(appointment);
            await _tableClient.AddEntityAsync(entity);
            
            _logger.LogInformation("Created appointment {Id} for {Client} in Azure Table Storage", 
                appointment.Id, appointment.ClientName);
            return appointment;
        }

        public async Task<Appointment> UpdateAsync(Appointment appointment)
        {
            var entity = new AppointmentTableEntity(appointment);
            await _tableClient.UpdateEntityAsync(entity, entity.ETag, TableUpdateMode.Replace);
            
            _logger.LogInformation("Updated appointment {Id} to status {Status} in Azure Table Storage", 
                appointment.Id, appointment.Status);
            return appointment;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var existingAppointment = await GetByIdAsync(id);
                if (existingAppointment == null)
                    return false;

                await _tableClient.DeleteEntityAsync(
                    existingAppointment.ClientEmail ?? "DefaultClient",
                    id.ToString());
                
                _logger.LogInformation("Deleted appointment {Id} from Azure Table Storage", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting appointment {Id}", id);
                return false;
            }
        }

        public async Task<bool> HasAppointmentForSlotAsync(int timeSlotId)
        {
            var filter = $"TimeSlotId eq {timeSlotId} and Status ne 'Cancelled'";
            
            await foreach (var _ in _tableClient.QueryAsync<AppointmentTableEntity>(filter: filter, maxPerPage: 1))
            {
                return true;
            }

            return false;
        }
    }
}
