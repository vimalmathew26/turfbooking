using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using turfbooking.Data;
using turfbooking.Models;

namespace turfbooking.Pages.Accounts
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly AppDbContext _context;

        public ForgotPasswordModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        public string? ErrorMessage { get; set; }

        public IActionResult OnPost()
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == Email);
            if (user == null)
            {
                ErrorMessage = "No user found with this email.";
                return Page();
            }

            return RedirectToPage("/Accounts/AnswerSecurityQuestion", new { email = Email });
        }
    }
}
