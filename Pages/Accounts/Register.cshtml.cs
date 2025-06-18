using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using turfbooking.Data;
using turfbooking.Models;
using turfbooking.Helper;
using System.Security.Claims;


namespace turfbooking.Pages.Accounts
{
    public class RegisterModel : PageModel
    {
        private readonly AppDbContext _context;

        public RegisterModel(AppDbContext context)
        {
            _context = context;
            NewUser = new Models.User(); // Fix: Specify the namespace explicitly to resolve ambiguity
            ConfirmPassword = string.Empty;
        }

        [BindProperty]
        public Models.User NewUser { get; set; } // Fix: Specify the namespace explicitly to resolve ambiguity

        [BindProperty]
        public string ConfirmPassword { get; set; }

        public void OnGet() { }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            if (NewUser.PasswordHash != ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Passwords do not match.");
                return Page();
            }

            var salt = PasswordHelper.GenerateSalt();
            var hashedPassword = PasswordHelper.HashPasswordWithSHA256(NewUser.PasswordHash, salt);
            NewUser.PasswordHash = $"{salt}:{hashedPassword}";
            NewUser.Role = "User";
            NewUser.IsActive = false;

            _context.Users.Add(NewUser);
            _context.SaveChanges();

            TempData["email"] = NewUser.Email;

            return RedirectToPage("/Accounts/SetupSecurity");
        }
    }
}
