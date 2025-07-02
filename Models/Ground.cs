using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace turfbooking.Models
{
    public class Ground
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ground name is required.")]
        [StringLength(100, ErrorMessage = "Ground name cannot exceed 100 characters.")]
        public  string GroundName { get; set; }
        public required string? PhotoPath { get; set; }

        [Required(ErrorMessage = "Location is required.")]
        [StringLength(100, ErrorMessage = "Location cannot exceed 100 characters.")]
        public string Location { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; }

        //[Required(ErrorMessage = "Price per hour is required.")]
        //[Range(50, 10000, ErrorMessage = "Price per hour must be between ₹50 and ₹10,000.")]
        public decimal PricePerHour { get; set; }

        [StringLength(200, ErrorMessage = "Supported sports cannot exceed 200 characters.")]
        public string SupportedSports { get; set; }

        public bool IsActive { get; set; } = true;

        public TimeSpan SlotDuration { get; set; }
        
        public DateTime StartTime { get; set; }
        
        public DateTime EndTime { get; set; }
        [ValidateNever]
        public ICollection<Booking>? Bookings { get; set; } = new List<Booking>();
        [ValidateNever]
        public ICollection<Review>? Reviews { get; set; } = new List<Review>();
        [ValidateNever]
        public ICollection<Court>? Courts { get; set; } = new List<Court>();


        public Ground()
        {
            GroundName = string.Empty;
            PhotoPath = string.Empty;
            Location = string.Empty;
            Description = string.Empty;
            SupportedSports = string.Empty;
            Bookings = new List<Booking>();
            Reviews = new List<Review>();
            Courts = new List<Court>();
            
        }
    }
}