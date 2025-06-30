using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using turfbooking.Data;
using turfbooking.Models;


namespace turfbooking.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class ManageUsersModel : PageModel
    {
        private readonly AppDbContext _context;

        public ManageUsersModel(AppDbContext context)
        {
            _context = context;
            Users = new List<User>();
        }

        public List<User> Users { get; set; }

        public void OnGet()
        {
            HttpContext.Session.SetString("PreviousPage", Url.Page("/Admin/AdminDashboard"));
            Users = _context.Users.ToList();
        }

        public async Task<IActionResult> OnPostDeactivateAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                user.IsActive = false;
                await _context.SaveChangesAsync();
                TempData["Message"] = $"{user.Username} has been deactivated.";
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostActivateAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                user.IsActive = true;
                await _context.SaveChangesAsync();
                TempData["Message"] = $"{user.Username} has been activated.";
            }
            return RedirectToPage();
        }
    }
}
