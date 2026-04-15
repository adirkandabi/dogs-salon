using DogsSalon.Models;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;


namespace DogsSalon.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<DogSize> DogSizes { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<AppointmentSummaryView> AppointmentSummaries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            modelBuilder.Entity<AppointmentSummaryView>()
                .HasNoKey()
                .ToView("vw_AppointmentSummaries");

            // Seed Data for dog sizes
            modelBuilder.Entity<DogSize>().HasData(
                new DogSize { Id = 1, SizeName = "Small", DurationMinutes = 30, BasePrice = 100 },
                new DogSize { Id = 2, SizeName = "Medium", DurationMinutes = 60, BasePrice = 150 },
                new DogSize { Id = 3, SizeName = "Large", DurationMinutes = 90, BasePrice = 200 }
            );
        }
    }
}
