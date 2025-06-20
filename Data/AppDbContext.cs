using Microsoft.EntityFrameworkCore;
using turfbooking.Models;

namespace turfbooking.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Ground> Grounds { get; set; }
        public DbSet<Slot> Slots { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Ground>()
                .HasMany(g => g.Reviews)
                .WithOne(r => r.Ground)
                .HasForeignKey(r => r.GroundId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ground>()
                .HasMany(g => g.Bookings)
                .WithOne(b => b.Ground)
                .HasForeignKey(b => b.GroundId)
                .OnDelete(DeleteBehavior.Restrict);

           

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Slot)
                .WithOne(s => s.Booking)
                .HasForeignKey<Slot>(s => s.BookingId)
                .OnDelete(DeleteBehavior.SetNull);

            // User relationships
            modelBuilder.Entity<User>()
                .HasMany(u => u.Bookings)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}