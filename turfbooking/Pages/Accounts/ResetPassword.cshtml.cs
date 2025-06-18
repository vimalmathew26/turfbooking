using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using turfbooking.Data;
using turfbooking.Helper;
using turfbooking.Models;

namespace turfbooking.Pages.Accounts
{
    public class ResetPasswordModel : PageModel
    {
        private readonly AppDbContext _context;

        public ResetPasswordModel(AppDbContext context)
        {
            _context = context;
            CurrentPassword = string.Empty;
            NewPassword = string.Empty;
            ConfirmPassword = string.Empty;
        }

        [BindProperty]
        public string CurrentPassword { get; set; }

        [BindProperty]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string NewPassword { get; set; }

        [BindProperty]
        public string ConfirmPassword { get; set; }

        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        public IActionResult OnGet()
        {
            if (User.FindFirst("UserId") == null)
                return RedirectToPage("/Accounts/Login");

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return RedirectToPage("/Accounts/Login");

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return RedirectToPage("/Accounts/Login");

            var parts = user.PasswordHash.Split(':');
            if (parts.Length != 2)
            {
                ErrorMessage = "Stored password format invalid.";
                return Page();
            }

            var salt = parts[0];
            var storedHash = parts[1];
            var currentHash = PasswordHelper.HashPasswordWithSHA256(CurrentPassword, salt);

            if (storedHash != currentHash)
            {
                ErrorMessage = "Current password is incorrect.";
                return Page();
            }

            if (NewPassword != ConfirmPassword)
            {
                ErrorMessage = "New password and confirmation do not match.";
                return Page();
            }

            var newSalt = PasswordHelper.GenerateSalt();
            var newHash = PasswordHelper.HashPasswordWithSHA256(NewPassword, newSalt);
            user.PasswordHash = $"{newSalt}:{newHash}";

            _context.SaveChanges();

            TempData["Message"] = "Password reset successful. Please login.";
            return RedirectToPage("/Accounts/Login");
        }

    }
}
