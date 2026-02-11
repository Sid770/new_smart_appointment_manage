using Microsoft.EntityFrameworkCore;
using AppointmentBookingAPI.Data;
using AppointmentBookingAPI.Models;

namespace AppointmentBookingAPI.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AppointmentRepository> _logger;
        
        public AppointmentRepository(ApplicationDbContext context, ILogger<AppointmentRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        
        public async Task<IEnumerable<Appointment>> GetAllAsync()
        {
            return await _context.Appointments
                .Include(a => a.TimeSlot)
                .OrderByDescending(a => a.BookedAt)
                .ToListAsync();
        }
        
        public async Task<Appointment?> GetByIdAsync(int id)
        {
            return await _context.Appointments
                .Include(a => a.TimeSlot)
                .FirstOrDefaultAsync(a => a.Id == id);
        }
        
        public async Task<IEnumerable<Appointment>> GetByStatusAsync(AppointmentStatus status)
        {
            return await _context.Appointments
                .Include(a => a.TimeSlot)
                .Where(a => a.Status == status)
                .OrderByDescending(a => a.BookedAt)
                .ToListAsync();
        }
        
        public async Task<IEnumerable<Appointment>> GetByTimeSlotAsync(int timeSlotId)
        {
            return await _context.Appointments
                .Include(a => a.TimeSlot)
                .Where(a => a.TimeSlotId == timeSlotId)
                .ToListAsync();
        }
        
        public async Task<Appointment> CreateAsync(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Created appointment {Id} for {Client}", appointment.Id, appointment.ClientName);
            return await GetByIdAsync(appointment.Id) ?? appointment;
        }
        
        public async Task<Appointment> UpdateAsync(Appointment appointment)
        {
            _context.Entry(appointment).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            _logger.LogInformation("Updated appointment {Id} to status {Status}", appointment.Id, appointment.Status);
            return appointment;
        }
        
        public async Task<bool> DeleteAsync(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
                return false;
            
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Deleted appointment {Id}", id);
            return true;
        }
        
        public async Task<bool> HasAppointmentForSlotAsync(int timeSlotId)
        {
            return await _context.Appointments
                .AnyAsync(a => a.TimeSlotId == timeSlotId && 
                              a.Status != AppointmentStatus.Cancelled);
        }
    }
}
