using Microsoft.EntityFrameworkCore;
using ConsultationAPI.Models;
using ConsultationAPI.Enums;

namespace ConsultationAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Slot> Slots { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройка для Slot
            modelBuilder.Entity<Slot>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.MeetingType)
                    .HasConversion<int>();
                
                entity.Property(e => e.Status)
                    .HasConversion<int>();
            });

            // Настройка для Booking
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Status)
                    .HasConversion<int>();
            });

            // Связь между Booking и Slot
            modelBuilder.Entity<Booking>()
                .HasOne<Slot>()
                .WithMany()
                .HasForeignKey(b => b.SlotId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}