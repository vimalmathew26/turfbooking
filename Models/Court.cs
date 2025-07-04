using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace turfbooking.Models
{
    public class Court
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Court name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Court name must be between 2 and 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Price per hour is required.")]
        [Range(200.00, 5000.00, ErrorMessage = "Price per hour must be between ₹200 and ₹5000")]
        public decimal PricePerHour { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Required]
        [Range(1, 12, ErrorMessage = "Duration must be between 1 and 12 hours.")]
        public TimeSpan Duration { get; set; }
        public int GroundId { get; set; }
        public Ground? Ground { get; set; }

    }
}