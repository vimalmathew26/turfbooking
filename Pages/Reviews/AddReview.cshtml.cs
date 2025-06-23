using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using turfbooking.Data;
using turfbooking.Models;

namespace turfbooking.Pages.Reviews
{
   
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

        public int groundId { get; set; }

        public int UserId { get; set; }


        public async Task<IActionResult> OnGetAsync()
        {
            Ground = await _context.Grounds.FirstOrDefaultAsync(g => g.Id == GroundId);

            if (Ground == null)
            {
                TempData["GroundNotFound"] = "Ground not found!";
                return RedirectToPage("./AddReview"); 
            }



            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int groundId)
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                ModelState.AddModelError(string.Empty, "User authentication required.");
                return Page();
            }
            
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.GroundId == groundId && b.UserId == userId);
            
            if (booking == null)
            {
                ModelState.AddModelError("book", "You First book the ground to add REVIEW");
                return Page();
            }
            Review.BookingId = booking.Id;
            Review.GroundId = groundId;
            Review.UserId = userId;

            _context.Reviews.Add(Review);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Users/GroundDetails", new { id = groundId });
        }
    }
}

