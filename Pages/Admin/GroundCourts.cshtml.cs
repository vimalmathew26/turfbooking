using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using turfbooking.Data;
using turfbooking.Models;


namespace turfbooking.Pages.Admin

{
    public class GroundCourtsModel : PageModel
    {

        public readonly AppDbContext _context;

        public GroundCourtsModel(AppDbContext context)
        {
            _context = context;
        }
        [BindProperty(SupportsGet = true)]
        public int GroundId { get; set; }      
        public List<Court> courts { get; set; } = new List<Court>();
        public async Task<IActionResult> OnGetAsync()
        {
            courts=await _context.Courts
                .Where(c => c.GroundId==GroundId)
                .ToListAsync();

            if (!courts.Any())
            {
                ModelState.AddModelError(string.Empty, "No courts found for the selected ground.");
                return Page();
            }
            return Page();
        }
    }
}
