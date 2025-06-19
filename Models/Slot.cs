using System.ComponentModel.DataAnnotations;
using turfbooking.Models;

namespace turfbooking.Models
{
    public class Slot
    {
        public  int Id { get; set; }
        [Required]
        public  int GroundId { get; set; }
        [Required]
        public  TimeSpan StartTime { get; set; }
        [Required]
        public  TimeSpan EndTime { get; set; }

        public DateTime BookingDate { get; set; }

        [Required]
        public bool IsBooked { get; set; } = false;// "Available", "Booked", "Blocked"
        public required Ground Ground { get; set; }
        // public Booking Booking { get; set; }

        public int? BookingId { get; set; }
        public Booking Booking { get; set; }
    }
}

