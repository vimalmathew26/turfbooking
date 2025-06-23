using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using turfbooking.Data;
using turfbooking.Models;

namespace turfbooking.Pages.Users
{
    [Authorize(Roles = "User")]
    public class UserDashboardModel : PageModel
    {
        private readonly AppDbContext _context;


        public UserDashboardModel(AppDbContext context)
        {
            _context = context;
        }

        public User? CurrentUser { get; set; }
        public List<Ground> GroundsWithPhotos { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdClaim = User.FindFirst("UserId");

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return RedirectToPage("/Accounts/Login");
            }

            CurrentUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (CurrentUser == null)
            {
                return RedirectToPage("/Accounts/Login");
            }

            if (!CurrentUser.IsActive)
            {
                return RedirectToPage("/Accounts/SetupSecurity");
            }

            GroundsWithPhotos = await _context.Grounds
               .Where(g => g.IsActive && !string.IsNullOrEmpty(g.PhotoPath))
               .ToListAsync();

            return Page();
        }
    }
}