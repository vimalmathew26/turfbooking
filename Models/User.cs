using System.ComponentModel.DataAnnotations;

namespace turfbooking.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required, Phone]
        public string PhoneNumber { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        [Required]
        public string Role { get; set; } 
        public bool IsActive { get; set; } = true;
        public ICollection<Booking> Bookings { get; set; }
    }
}
