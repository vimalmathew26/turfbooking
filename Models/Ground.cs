using System.ComponentModel.DataAnnotations;

namespace turfbooking.Models
{
    public class Ground
    {

        public int Id { get; set; }

        [Required]
        public string GroundName { get; set; }

        public string PhotoPath { get; set; }

        [Required]
        public string Location { get; set; }

        public string Description { get; set; }

        [Required]
        public decimal PricePerHour { get; set; }

        public string SupportedSports { get; set; }

        public bool IsActive { get; set; } = true;



        public ICollection<Review> Reviews { get; set; }
        public ICollection<Booking> Bookings { get; set; }
    }
}
