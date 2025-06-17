using System.ComponentModel.DataAnnotations;

namespace TurfBookingApp.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^\+?\d{10,13}$", ErrorMessage = "Please enter a valid phone number (10 to 13 digits, optional + at start).")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string PasswordHash { get; set; }

        [Required(ErrorMessage = "Role is required.")]
        public string Role { get; set; }

        public bool IsActive { get; set; } = true;

        public string SecurityQuestion { get; set; }

        public string SecurityAnswer { get; set; }

        public User()
        {
            Username = "";
            Email = "";
            PhoneNumber = "";
            PasswordHash = "";
            Role = "User";
            SecurityQuestion = "";
            SecurityAnswer = "";
        }
    }
}
