using System.ComponentModel.DataAnnotations;

namespace turfbooking.Models
{
    public class Slot
    {
        public int Id { get; set; }

        [Required]
        public int GroundId { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        [Required]
        public DateTime BookingDate { get; set; }

        public bool IsBooked { get; set; } = false;

        public int? BookingId { get; set; }
        public Ground? Ground { get; set; }
        public Booking? Booking { get; set; }

        public Slot()
        {
            BookingDate = DateTime.Today;
            StartTime = TimeSpan.Zero;
            EndTime = TimeSpan.Zero;
        }
    }
}