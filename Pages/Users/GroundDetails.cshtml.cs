using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using turfbooking.Data;
using turfbooking.Models;

namespace turfbooking.Pages.Users
{
    [Authorize(Roles = "User")]
    public class GroundDetailsModel : PageModel
    {
        private readonly AppDbContext _context;

        public GroundDetailsModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Review> Reviews { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public required Ground Ground { get; set; }

        [BindProperty(SupportsGet = true)]
        public double AverageRating { get; set; }

        [BindProperty(SupportsGet = true)]
        public int id { get; set; }




        public async Task<IActionResult> OnGetAsync()
        {
            HttpContext.Session.SetString("PreviousPage", Url.Page("/Users/GroundList"));
            Ground = await _context.Grounds.FirstOrDefaultAsync(g => g.Id == id && g.IsActive);

            if (Ground == null)
            {
                ModelState.AddModelError(string.Empty,"Ground Not Found");
                return Page();
            }
                

            Reviews = await _context.Reviews
             .Where(r => r.GroundId == id && r.IsVisible)
             .ToListAsync();

            AverageRating = Reviews.Any() ? Reviews.Average(r => r.Rating) : 0;


            return Page();


        }
    }
}

