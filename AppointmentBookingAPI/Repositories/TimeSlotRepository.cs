using Microsoft.EntityFrameworkCore;
using AppointmentBookingAPI.Data;
using AppointmentBookingAPI.Models;

namespace AppointmentBookingAPI.Repositories
{
    public class TimeSlotRepository : ITimeSlotRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TimeSlotRepository> _logger;
        
        public TimeSlotRepository(ApplicationDbContext context, ILogger<TimeSlotRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        
        public async Task<IEnumerable<TimeSlot>> GetAllAsync()
        {
            return await _context.TimeSlots
                .OrderBy(ts => ts.StartTime)
                .ToListAsync();
        }
        
        public async Task<IEnumerable<TimeSlot>> GetAvailableAsync(DateTime? date = null, string? serviceProvider = null)
        {
            var query = _context.TimeSlots
                .Where(ts => ts.IsAvailable && ts.StartTime > DateTime.UtcNow);
            
            if (date.HasValue)
            {
                var startOfDay = date.Value.Date;
                var endOfDay = startOfDay.AddDays(1);
                query = query.Where(ts => ts.StartTime >= startOfDay && ts.StartTime < endOfDay);
            }
            
            if (!string.IsNullOrEmpty(serviceProvider))
            {
                query = query.Where(ts => ts.ServiceProvider == serviceProvider);
            }
            
            return await query.OrderBy(ts => ts.StartTime).ToListAsync();
        }
        
        public async Task<TimeSlot?> GetByIdAsync(int id)
        {
            return await _context.TimeSlots.FindAsync(id);
        }
        
        public async Task<TimeSlot> CreateAsync(TimeSlot timeSlot)
        {
            _context.TimeSlots.Add(timeSlot);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Created time slot {Id} for {Provider}", timeSlot.Id, timeSlot.ServiceProvider);
            return timeSlot;
        }
        
        public async Task<TimeSlot> UpdateAsync(TimeSlot timeSlot)
        {
            _context.Entry(timeSlot).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            _logger.LogInformation("Updated time slot {Id}", timeSlot.Id);
            return timeSlot;
        }
        
        public async Task<bool> DeleteAsync(int id)
        {
            var timeSlot = await _context.TimeSlots.FindAsync(id);
            if (timeSlot == null)
                return false;
            
            _context.TimeSlots.Remove(timeSlot);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Deleted time slot {Id}", id);
            return true;
        }
        
        public async Task<bool> IsSlotAvailableAsync(int slotId)
        {
            var slot = await _context.TimeSlots.FindAsync(slotId);
            return slot != null && slot.IsAvailable && slot.StartTime > DateTime.UtcNow;
        }
        
        public async Task<bool> HasConflictAsync(DateTime startTime, DateTime endTime, string serviceProvider, int? excludeId = null)
        {
            var query = _context.TimeSlots
                .Where(ts => ts.ServiceProvider == serviceProvider &&
                            ((ts.StartTime >= startTime && ts.StartTime < endTime) ||
                             (ts.EndTime > startTime && ts.EndTime <= endTime) ||
                             (ts.StartTime <= startTime && ts.EndTime >= endTime)));
            
            if (excludeId.HasValue)
            {
                query = query.Where(ts => ts.Id != excludeId.Value);
            }
            
            return await query.AnyAsync();
        }
    }
}
