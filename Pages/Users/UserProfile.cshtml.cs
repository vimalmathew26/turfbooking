using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using turfbooking.Data;
using turfbooking.Models;

namespace turfbooking.Pages.Users
{
    [Authorize(Roles = "User")]
    public class UserProfileModel : PageModel
    {
        private readonly AppDbContext _context;

        public UserProfileModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public UserInputModel Input { get; set; } = new();

        public string? SuccessMessage { get; set; }

        public class UserInputModel
        {
            [Required(ErrorMessage = "Username is required.")]
            [StringLength(50)]
            public string Username { get; set; }

            [Required(ErrorMessage = "Email is required.")]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [RegularExpression(@"^\+?\d{10,13}$", ErrorMessage = "Phone number must be 10–13 digits (optional +).")]
            public string PhoneNumber { get; set; }
        }

        public IActionResult OnGet()
        {
            HttpContext.Session.SetString("PreviousPage", Url.Page("/Users/UserDashboard"));

            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return RedirectToPage("/Login");

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return RedirectToPage("/Login");

            Input = new UserInputModel
            {
                Username = user.Username,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return RedirectToPage("/Login");

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return RedirectToPage("/Login");

            user.Username = Input.Username;
            user.Email = Input.Email;
            user.PhoneNumber = Input.PhoneNumber;

            _context.SaveChanges();

            SuccessMessage = "Profile updated successfully!";
            return Page();
        }
    }
}
