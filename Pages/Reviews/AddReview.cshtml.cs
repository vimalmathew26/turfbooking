using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using turfbooking.Data;
using turfbooking.Models;

namespace turfbooking.Pages.Reviews
{
    [Authorize(Roles = "User")]
    public class AddReviewModel : PageModel
    {
        private readonly AppDbContext _context;
        public AddReviewModel(AppDbContext context)
        {
            _context = context;
        }
        public string book { get; set; }
        public Ground Ground { get; set; }
        public List<Review> Reviews { get; set; }

        [BindProperty]
        public Review Review { get; set; }

        [BindProperty(SupportsGet = true)]
        public int GroundId { get; set; }

        public int UserId { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {

            var previousUrl = Url.Page(
     "/Users/GroundDetails",
     pageHandler: null,
     values: new { id = GroundId },
     protocol: Request.Scheme
 );

            HttpContext.Session.SetString("PreviousPage", previousUrl);
            Ground = await _context.Grounds.FirstOrDefaultAsync(g => g.Id == GroundId);

            if (Ground == null)
            {
                TempData["GroundAlert"] = "Ground not found!";
                return Page();
            }
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            Ground = await _context.Grounds.FirstOrDefaultAsync(g => g.Id == GroundId);
            if (Ground == null)
            {
                TempData["GroundAlert"] = "Ground not found!";
                return Page();
            }

            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                ModelState.AddModelError(string.Empty, "User authentication required.");
                return Page();
            }
            
            if (!ModelState.IsValid)
            {
                return Page();
            }
            else
            {
                var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.GroundId == GroundId && b.UserId == userId);

                if (booking == null)
                {
                    ModelState.AddModelError(string.Empty, "You must book the ground before adding a review.");

                    return Page();
                }
                Review.BookingId = booking.Id;
                Review.GroundId = GroundId;
                Review.UserId = userId;
                _context.Reviews.Add(Review);
                await _context.SaveChangesAsync();

                return RedirectToPage("/Users/GroundDetails", new { id = GroundId });
            }
        }
    }
}

