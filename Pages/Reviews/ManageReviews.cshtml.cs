using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using turfbooking.Data;
using turfbooking.Models;

namespace turfbooking.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class ManageReviewsModel : PageModel
    {
        private readonly AppDbContext _context;

        public ManageReviewsModel(AppDbContext context)
        {
            _context = context;
        }

        public List<ReviewViewModel> Reviews { get; set; } = new();

        public class ReviewViewModel
        {
            public int Id { get; set; }
            public string UserName { get; set; }
            public string GroundName { get; set; }
            public int Rating { get; set; }
            public string Comment { get; set; }
            public bool IsVisible { get; set; }
        }

        public void OnGet()
        {
            HttpContext.Session.SetString("PreviousPage", Url.Page("/Admin/AdminDashboard"));
            Reviews = _context.Reviews
                .Include(r => r.Ground)
                .Join(_context.Users,
                      review => review.UserId,
                      user => user.Id,
                      (review, user) => new ReviewViewModel
                      {
                          Id = review.Id,
                          UserName = user.Username,
                          GroundName = review.Ground.GroundName,
                          Rating = review.Rating,
                          Comment = review.Comment,
                          IsVisible = review.IsVisible
                      })
                .ToList();
        }

        public async Task<IActionResult> OnPostToggleVisibilityAsync(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                review.IsVisible = !review.IsVisible;
                await _context.SaveChangesAsync();
                TempData["Message"] = "Review visibility updated.";
            }
            return RedirectToPage();
        }
    }
}
