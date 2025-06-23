using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using turfbooking.Data;
using turfbooking.Models;

namespace turfbooking.Pages.Users
{
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


        public async Task<IActionResult> OnGetAsync(int id)
        {
            //var ground = await _context.Grounds.FirstOrDefaultAsync(g => g.Id == id && g.IsActive);
            ////.Include(g => g.Bookings)
            ////.ThenInclude(b => b.Review)
            ////.FirstOrDefaultAsync(g => g.Id == id && g.IsActive);

            //if (ground == null)
            //    return NotFound();

            ////var ratings = Ground.Bookings
            ////    .Where(b => b.Review != null)
            ////    .Select(b => b.Review.Rating)
            ////    .ToList();

            ////AverageRating = ratings.Count > 0 ? ratings.Average() : 0;

            //return Page();

            Ground = await _context.Grounds.FirstOrDefaultAsync(g => g.Id == id && g.IsActive);

            if (Ground == null)
                return NotFound();

            Reviews = await _context.Reviews
             .Where(r => r.GroundId == id && r.IsVisible)
             .ToListAsync();

            return Page();


        }
    }
}

