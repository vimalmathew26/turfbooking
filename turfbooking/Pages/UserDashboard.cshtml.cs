using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using TurfBookingApp.Data;
using TurfBookingApp.Models;

namespace TurfBookingApp.Pages
{
    public class UserDashboardModel : PageModel
    {
        private readonly AppDbContext _context;

        public UserDashboardModel(AppDbContext context)
        {
            _context = context;
        }

        public User? CurrentUser { get; set; }

        public IActionResult OnGet()
        {
            var userIdClaim = User.FindFirst("UserId");

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return RedirectToPage("/Login");
            }

            CurrentUser = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (CurrentUser == null)
            {
                return RedirectToPage("/Login");
            }

            if (!CurrentUser.IsActive)
            {
                // Not allowed until security question is set
                return RedirectToPage("/SetupSecurity");
            }

            return Page();
        }
    }
}
