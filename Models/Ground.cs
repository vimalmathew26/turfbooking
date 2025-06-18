using System.ComponentModel.DataAnnotations;

namespace TurfBookingApp.Models
{
    public class Ground
    {

        public int Id { get; set; }

        [Required]
        public string GroundName { get; set; }

        public string PhotoPath { get; set; }

        [Required]
        public string Location { get; set; }

        public string Description { get; set; }

        [Required]
        public decimal PricePerHour { get; set; }

        public string SupportedSports { get; set; }

        public bool IsActive { get; set; } = true;

        [Required]
        public TimeSpan OpenTime { get; set; }  // e.g., 07:00

        [Required]
        public TimeSpan CloseTime { get; set; } // e.g., 21:00

        public ICollection<Review> Reviews { get; set; }
        public ICollection<Booking> Bookings { get; set; }
    }
}
