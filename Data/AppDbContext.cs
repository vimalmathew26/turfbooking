using Microsoft.EntityFrameworkCore;
using TurfBookingApp.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Slot> Slots { get; set; }
    public DbSet<Ground> Grounds { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        
        modelBuilder.Entity<Slot>()
            .HasOne(s => s.Booking)
            .WithOne()
            .HasForeignKey<Slot>(s => s.BookingId);
    }



}