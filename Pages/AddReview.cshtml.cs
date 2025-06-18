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

        public async Task<IActionResult> OnGetAsync(int groundId)
        {
          Ground=await _context.Grounds
                .Include(g => g.Reviews)
                .FirstOrDefaultAsync(g => g.Id == groundId);
            if (Ground == null)
            {
                return NotFound();
            }   

            GroundId = groundId;

            return Page();
        }
    }
}
