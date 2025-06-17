using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TurfBookingApp.Data;
using TurfBookingApp.Helper;
using TurfBookingApp.Models;

namespace TurfBookingApp.Pages
{
    public class AnswerSecurityQuestionModel : PageModel
    {
        private readonly AppDbContext _context;

        public AnswerSecurityQuestionModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        public string Answer { get; set; } = string.Empty;

        [BindProperty]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string NewPassword { get; set; } = string.Empty;

        [BindProperty]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string? Question { get; set; }
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        public IActionResult OnGet()
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == Email);
            if (user == null)
            {
                return RedirectToPage("/ForgotPassword");
            }

            Question = user.SecurityQuestion;
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                var u = _context.Users.FirstOrDefault(u => u.Email == Email);
                Question = u?.SecurityQuestion ?? "N/A";
                return Page();
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == Email);
            if (user == null)
            {
                ErrorMessage = "User not found.";
                return Page();
            }

            // Verify hashed security answer
            var parts = user.SecurityAnswer.Split(':');
            if (parts.Length != 2)
            {
                ErrorMessage = "Stored security answer format is invalid.";
                Question = user.SecurityQuestion;
                return Page();
            }

            var salt = parts[0];
            var storedHash = parts[1];
            var inputHash = PasswordHelper.HashPasswordWithSHA256(Answer.Trim(), salt);

            if (storedHash != inputHash)
            {
                ErrorMessage = "Incorrect security answer.";
                Question = user.SecurityQuestion;
                return Page();
            }

            if (NewPassword != ConfirmPassword)
            {
                ErrorMessage = "Passwords do not match.";
                Question = user.SecurityQuestion;
                return Page();
            }

            var newSalt = PasswordHelper.GenerateSalt();
            var newHash = PasswordHelper.HashPasswordWithSHA256(NewPassword, newSalt);
            user.PasswordHash = $"{newSalt}:{newHash}";

            _context.SaveChanges();

            TempData["Message"] = "Password reset successful. Please login.";
            return RedirectToPage("/Login");
        }
    }
}
