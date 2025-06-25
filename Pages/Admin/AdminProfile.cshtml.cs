using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using turfbooking.Data;
using turfbooking.Models;

namespace turfbooking.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminProfileModel : PageModel
    {
        private readonly AppDbContext _context;

        public AdminProfileModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public AdminInputModel Input { get; set; } = new();

        public string? SuccessMessage { get; set; }

        public class AdminInputModel
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
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return RedirectToPage("/Accounts/Login");

            var admin = _context.Users.FirstOrDefault(u => u.Id == userId && u.Role == "Admin");
            if (admin == null)
                return RedirectToPage("/Accounts/Login");

            Input = new AdminInputModel
            {
                Username = admin.Username,
                Email = admin.Email,
                PhoneNumber = admin.PhoneNumber
            };

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return RedirectToPage("/Accounts/Login");

            var admin = _context.Users.FirstOrDefault(u => u.Id == userId && u.Role == "Admin");
            if (admin == null)
                return RedirectToPage("/Accounts/Login");

            admin.Username = Input.Username;
            admin.Email = Input.Email;
            admin.PhoneNumber = Input.PhoneNumber;

            _context.SaveChanges();

            SuccessMessage = "Profile updated successfully!";
            return Page();
        }
    }

}