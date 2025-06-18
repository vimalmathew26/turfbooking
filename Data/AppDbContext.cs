using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using turfbooking.Models;

namespace turfbooking.Data
{
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Ground> Grounds { get; set; }
        public DbSet<Slot> Slots { get; set; }
    public DbSet<Review> Reviews { get; set; }

    }



}