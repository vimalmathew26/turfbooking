using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.RegularExpressions;
using turfbooking.Data;
using turfbooking.Models;

namespace turfbooking.Pages.Grounds
{
    [Authorize(Roles = "Admin")]
    public class DeleteModel : PageModel
    {
        private readonly AppDbContext _context;

        public DeleteModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Ground Ground { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var ground = await _context.Grounds.FindAsync(id);
            if (ground == null)
            {
                return NotFound();
            }
            Ground = ground;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var groundToDelete = await _context.Grounds.FindAsync(Ground.Id);
            if (groundToDelete == null)
            {
                return NotFound();
            }

            _context.Grounds.Remove(groundToDelete);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Grounds/Index");
        }
    }
}

