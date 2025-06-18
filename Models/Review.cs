using System.ComponentModel.DataAnnotations;

namespace turfbooking.Models
{
    public class Review
    {
        public int Id { get; set; }
        [Required]
        public int BookingId { get; set; }
        [Range(1, 5)]
        public int Rating { get; set; }
        [MaxLength(500)]
        public string Comment { get; set; }
        public bool IsVisible { get; set; } = true;
        public Booking Booking { get; set; }
    }
}
