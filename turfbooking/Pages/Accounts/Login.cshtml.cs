using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using turfbooking.Data;
using turfbooking.Helper;
using turfbooking.Models;

namespace turfbooking.Pages.Accounts
{
    public class LoginModel : PageModel
    {
        private readonly AppDbContext _context;

        public LoginModel(AppDbContext context)
        {
            _context = context;
            Email = string.Empty;
            Password = string.Empty;
        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public void OnGet()
        {
            var returnUrl = Request.Query["ReturnUrl"].ToString();

            if (!string.IsNullOrEmpty(returnUrl) && returnUrl.Contains("AdminDashboard", StringComparison.OrdinalIgnoreCase) || returnUrl.Contains("UserDashboard", StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] = "You are not authorized to access that page.";
            }
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == Email && u.IsActive);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid credentials.");
                return Page();
            }

            if (user == null || !PasswordHelper.VerifyPassword(Password, user.PasswordHash))
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return Page();
            }

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("UserId", user.Id.ToString())
        };

            var identity = new ClaimsIdentity(claims, "UserAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("UserAuth", principal, new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
            });

            if (user.Role == "Admin")
                return RedirectToPage("/Admin/AdminDashboard");
            else
                return RedirectToPage("/Users/UserDashboard");

        }
    }
}
