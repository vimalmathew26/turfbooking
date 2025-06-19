using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using turfbooking.Models;
using Microsoft.EntityFrameworkCore;

namespace turfbooking.Pages
{
    public class AddReviewModel : PageModel
    {
        private readonly AppDbContext _context;

        public AddReviewModel(AppDbContext context)
        {
            _context = context;
        }

        public Ground Ground { get; set; }
        public List<Review> Reviews { get; set; }
        public int GroundId { get; set; }
        [BindProperty]
        public Review Review { get; set; }

        public async Task<IActionResult> OnGetAsync(int groundId)
        {
            Ground = await _context.Grounds
                .Include(g => g.Reviews)
                .FirstOrDefaultAsync(g => g.Id == 1);
            if (Ground == null)
            {
                return NotFound();
            }   

            GroundId = 1;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            GroundId = 1;
            Ground = await _context.Grounds
                .Include(g => g.Reviews)
                .FirstOrDefaultAsync(g => g.Id == GroundId);


            if (Ground==null)
            {
                return NotFound();
            }
            int userId = 2;
            var booking = await _context.Bookings

        .FirstOrDefaultAsync(b => b.GroundId == GroundId && b.UserId == userId);

            if (booking == null)
            {
                ModelState.AddModelError(string.Empty, "You must have a booking to review this ground.");
                return Page();
            }
            Review.BookingId = booking.Id;
            Review.GroundId = GroundId;

            _context.Reviews.Add(Review);
            await _context.SaveChangesAsync();

            return RedirectToPage("./AddReview", new { groundId = GroundId });
        }
    }
}

