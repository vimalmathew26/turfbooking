using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TurfBookingApp.Data;
using TurfBookingApp.Models;
using TurfBookingApp.Helper;
using System.Security.Claims;


namespace TurfBookingApp.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly AppDbContext _context;

        public RegisterModel(AppDbContext context)
        {
            _context = context;
            NewUser = new User();
            ConfirmPassword = string.Empty;
        }

        [BindProperty]
        public User NewUser { get; set; }

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
