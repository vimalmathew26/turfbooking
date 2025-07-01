using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace turfbooking.Models
{
    public class Booking
    {
        public int Id { get; set; }
       
        public int UserId { get; set; }
    
        public int GroundId { get; set; }
        
        public int SlotId { get; set; }

        public DateTime BookingDate { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public decimal TotalPrice { get; set; }

        public BookingStatus Status { get; set; } = BookingStatus.Confirmed;

        public User User { get; set; }
        public Ground Ground { get; set; }

        public Slot Slot { get; set; }

        public int courtId { get; set; }
        public Court Court { get; set; }
    }

    public enum BookingStatus
    {
        Confirmed,
        Cancelled
    }


}

