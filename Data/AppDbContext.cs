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
        public DbSet<Court> Courts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Ground relationships
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

            // Court → Booking: enable cascade delete
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Court)
                .WithMany()
                .HasForeignKey(b => b.courtId)
                .OnDelete(DeleteBehavior.Restrict);

            // Booking → Slot: disable cascade to prevent circular path
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Slot)
                .WithOne(s => s.Booking)
                .HasForeignKey<Booking>(b => b.SlotId)
                .OnDelete(DeleteBehavior.NoAction);

            // Slot → Booking: also disable
            modelBuilder.Entity<Slot>()
                .HasOne(s => s.Booking)
                .WithOne(b => b.Slot)
                .HasForeignKey<Slot>(s => s.BookingId)
                .OnDelete(DeleteBehavior.NoAction);

            // Slot → Court: disable cascade
            modelBuilder.Entity<Slot>()
                .HasOne(s => s.Court)
                .WithMany()
                .HasForeignKey(s => s.courtId)
                .OnDelete(DeleteBehavior.NoAction);

            // Booking → User and Ground: safe to keep cascade OFF
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Ground)
                .WithMany()
                .HasForeignKey(b => b.GroundId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}