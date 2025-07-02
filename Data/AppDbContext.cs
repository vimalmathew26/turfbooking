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

            modelBuilder.Entity<Slot>()
                .HasOne(s => s.Ground)
                .WithMany()
                .HasForeignKey(s => s.GroundId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Slot)
                .WithOne(s => s.Booking)
                .HasForeignKey<Booking>(b => b.SlotId)
                .OnDelete(DeleteBehavior.Restrict); // 👈 OR .NoAction

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Ground)
                .WithMany(g => g.Bookings)
                .HasForeignKey(b => b.GroundId)
                .OnDelete(DeleteBehavior.Restrict); // 👈 OR .NoAction


           



        }
    }
}