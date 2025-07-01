using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using turfbooking.Data;
using turfbooking.Models;

namespace turfbooking.Pages.Users
{
    
    public class CourtListModel : PageModel
    {

        public readonly AppDbContext _context;

        public CourtListModel(AppDbContext context)
        {
            _context = context;
        }
        [BindProperty(SupportsGet = true)]
        public int GroundId { get; set; }
        public List<Court> courts { get; set; } = new List<Court>();

        public Slot Slot { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            HttpContext.Session.SetString("PreviousPage", Url.Page("/Users/GroundList"));
            courts = await _context.Courts
                .Where(c => c.GroundId == GroundId)
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
