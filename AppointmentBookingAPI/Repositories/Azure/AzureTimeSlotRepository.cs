using Azure.Data.Tables;
using AppointmentBookingAPI.Models;
using AppointmentBookingAPI.Models.Azure;

namespace AppointmentBookingAPI.Repositories.Azure
{
    /// <summary>
    /// Azure Table Storage repository for TimeSlot
    /// </summary>
    public class AzureTimeSlotRepository : ITimeSlotRepository
    {
        private readonly TableClient _tableClient;
        private readonly ILogger<AzureTimeSlotRepository> _logger;
        private const string TableName = "TimeSlots";

        public AzureTimeSlotRepository(
            TableServiceClient tableServiceClient,
            ILogger<AzureTimeSlotRepository> logger)
        {
            _tableClient = tableServiceClient.GetTableClient(TableName);
            _tableClient.CreateIfNotExists();
            _logger = logger;
        }

        public async Task<IEnumerable<TimeSlot>> GetAllAsync()
        {
            var entities = new List<TimeSlot>();
            
            await foreach (var entity in _tableClient.QueryAsync<TimeSlotTableEntity>())
            {
                entities.Add(entity.ToTimeSlot());
            }

            return entities.OrderBy(t => t.StartTime);
        }

        public async Task<TimeSlot?> GetByIdAsync(int id)
        {
            var query = _tableClient.QueryAsync<TimeSlotTableEntity>(
                filter: $"RowKey eq '{id}'");

            await foreach (var entity in query)
            {
                return entity.ToTimeSlot();
            }

            return null;
        }

        public async Task<IEnumerable<TimeSlot>> GetAvailableAsync(DateTime? date = null, string? serviceProvider = null)
        {
            var filters = new List<string>
            {
                "IsAvailable eq true",
                $"StartTime gt datetime'{DateTime.UtcNow:yyyy-MM-ddTHH:mm:ssZ}'"
            };

            if (!string.IsNullOrEmpty(serviceProvider))
            {
                filters.Add($"PartitionKey eq '{serviceProvider}'");
            }

            var filterString = string.Join(" and ", filters);
            var entities = new List<TimeSlot>();

            await foreach (var entity in _tableClient.QueryAsync<TimeSlotTableEntity>(filter: filterString))
            {
                var timeSlot = entity.ToTimeSlot();
                
                if (date.HasValue)
                {
                    if (timeSlot.StartTime.Date == date.Value.Date)
                    {
                        entities.Add(timeSlot);
                    }
                }
                else
                {
                    entities.Add(timeSlot);
                }
            }

            return entities.OrderBy(t => t.StartTime);
        }

        public async Task<TimeSlot> CreateAsync(TimeSlot timeSlot)
        {
            // Generate a unique ID if not set
            if (timeSlot.Id == 0)
            {
                timeSlot.Id = Math.Abs(Guid.NewGuid().GetHashCode());
            }

            var entity = new TimeSlotTableEntity(timeSlot);
            await _tableClient.AddEntityAsync(entity);
            
            _logger.LogInformation("Created time slot {Id} in Azure Table Storage", timeSlot.Id);
            return timeSlot;
        }

        public async Task<TimeSlot> UpdateAsync(TimeSlot timeSlot)
        {
            var entity = new TimeSlotTableEntity(timeSlot);
            await _tableClient.UpdateEntityAsync(entity, entity.ETag, TableUpdateMode.Replace);
            
            _logger.LogInformation("Updated time slot {Id} in Azure Table Storage", timeSlot.Id);
            return timeSlot;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var existingSlot = await GetByIdAsync(id);
                if (existingSlot == null)
                    return false;

                await _tableClient.DeleteEntityAsync(
                    existingSlot.ServiceProvider ?? "DefaultProvider",
                    id.ToString());
                
                _logger.LogInformation("Deleted time slot {Id} from Azure Table Storage", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting time slot {Id}", id);
                return false;
            }
        }

        public async Task<bool> HasConflictAsync(
            DateTime startTime,
            DateTime endTime,
            string serviceProvider,
            int? excludeId = null)
        {
            var filter = $"PartitionKey eq '{serviceProvider}' and IsAvailable eq true";
            
            await foreach (var entity in _tableClient.QueryAsync<TimeSlotTableEntity>(filter: filter))
            {
                var slot = entity.ToTimeSlot();
                
                if (excludeId.HasValue && slot.Id == excludeId.Value)
                    continue;

                // Check for overlap
                if ((startTime >= slot.StartTime && startTime < slot.EndTime) ||
                    (endTime > slot.StartTime && endTime <= slot.EndTime) ||
                    (startTime <= slot.StartTime && endTime >= slot.EndTime))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
