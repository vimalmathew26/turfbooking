using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace turfbooking.Models
{
    public class Slot
    {
        public int Id { get; set; }

        
        public int GroundId { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }
        [Required]
        public DateTime BookingDate { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        public SlotStatus Status { get; set; } = SlotStatus.Available;

        [ValidateNever]
        public Ground Ground { get; set; }

        public int? BookingId { get; set; }
        [ValidateNever]
        public Booking Booking { get; set; }

       
        public enum SlotStatus
        {
            Booked,
            Available,
            Blocked
        }

    }
}
