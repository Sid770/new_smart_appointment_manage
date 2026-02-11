using Microsoft.EntityFrameworkCore;
using AppointmentBookingAPI.Models;

namespace AppointmentBookingAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        
        public DbSet<TimeSlot> TimeSlots { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configure TimeSlot
            modelBuilder.Entity<TimeSlot>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ServiceProvider).IsRequired().HasMaxLength(100);
            });
            
            // Configure Appointment
            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ClientName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ClientEmail).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ClientPhone).IsRequired().HasMaxLength(20);
                entity.Property(e => e.ServiceType).IsRequired().HasMaxLength(100);
                
                entity.HasOne(e => e.TimeSlot)
                    .WithMany()
                    .HasForeignKey(e => e.TimeSlotId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            
            // Seed initial data
            SeedData(modelBuilder);
        }
        
        private void SeedData(ModelBuilder modelBuilder)
        {
            var baseDate = DateTime.Today.AddDays(1); // Start from tomorrow
            var slots = new List<TimeSlot>();
            int id = 1;
            
            // Create slots for next 7 days
            for (int day = 0; day < 7; day++)
            {
                var currentDate = baseDate.AddDays(day);
                
                // Create slots from 9 AM to 6 PM (9 slots per day)
                for (int hour = 9; hour < 18; hour++)
                {
                    slots.Add(new TimeSlot
                    {
                        Id = id++,
                        StartTime = currentDate.AddHours(hour),
                        EndTime = currentDate.AddHours(hour + 1),
                        ServiceProvider = hour < 13 ? "Dr. Smith" : "Dr. Johnson",
                        IsAvailable = true,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }
            
            modelBuilder.Entity<TimeSlot>().HasData(slots);
        }
    }
}
