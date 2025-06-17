using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using TurfBookingApp.Models;

namespace TurfBookingApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

    }
}

