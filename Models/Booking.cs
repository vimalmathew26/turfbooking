using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace turfbooking.Models
{
    public class Booking
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int GroundId { get; set; }

        [Required]
        public DateTime BookingDate { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        public decimal TotalPrice { get; set; }

        public BookingStatus Status { get; set; } = BookingStatus.Confirmed;

        public User User { get; set; }
        public Ground Ground { get; set; }
 

        public Slot Slot { get; set; }
    }

    public enum BookingStatus
    {
        Confirmed,
        Cancelled,
        Completed
    }

   
}

