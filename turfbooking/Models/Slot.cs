using System.ComponentModel.DataAnnotations;
using turfbooking.Models;

namespace turfbooking.Models
{
    public class Slot
    {
        public required int Id { get; set; }
        [Required]
        public required int GroundId { get; set; }
        [Required]
        public required DateTime StartTime { get; set; }
        [Required]
        public required DateTime EndTime { get; set; }

        [Required]
        public required string Status { get; set; } // "Available", "Booked", "Blocked"
        public required Ground Ground { get; set; }
        // public Booking Booking { get; set; }
    }
}

