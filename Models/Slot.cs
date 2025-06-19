using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace turfbooking.Models
{
    public class Slot
    {
        public int Id { get; set; }


        public int GroundId { get; set; }


        public TimeSpan StartTime { get; set; }

        public DateTime BookingDate { get; set; }


        public TimeSpan EndTime { get; set; }

        public bool IsBooked { get; set; } = false;

        public Ground Ground { get; set; }


        [ForeignKey("Booking")]
        public int? BookingId { get; set; }
        public Booking Booking { get; set; }


    }
}
