using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace turfbooking.Models
{
    public class Slot
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ground is required.")]
        public int GroundId { get; set; }

        [Required(ErrorMessage = "Start time is required.")]
        [Display(Name = "Start Time")]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "Booking date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Booking Date")]
        public DateTime BookingDate { get; set; }

        [Required(ErrorMessage = "End time is required.")]
        [Display(Name = "End Time")]
        public TimeSpan EndTime { get; set; }

        [Display(Name = "Slot Status")]
        public SlotStatus Status { get; set; } = SlotStatus.Available;

        [ValidateNever]
        public Ground Ground { get; set; }

        public int? BookingId { get; set; }

        [ValidateNever]
        public Booking Booking { get; set; }
        public int courtId { get; set; }
        public Court Court { get; set; }

        public enum SlotStatus
        {
            Booked,
            Available,
            Blocked
        }
    }
}
