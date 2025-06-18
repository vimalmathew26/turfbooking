using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using turfbooking.Data;
using turfbooking.Models;

namespace turfbooking.Pages.Grounds
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public EditModel(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [BindProperty]
        public required Ground Ground { get; set; }

        [BindProperty]
        public IFormFile? Photo { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var ground = await _context.Grounds.FindAsync(id);
            if (ground == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var groundInDb = await _context.Grounds.FindAsync(Ground.Id);
            if (groundInDb == null)
                return NotFound();

            if (!ModelState.IsValid)
                return Page();

            groundInDb.GroundName = Ground.GroundName;
            groundInDb.Location = Ground.Location;
            groundInDb.Description = Ground.Description;
            groundInDb.PricePerHour = Ground.PricePerHour;
            groundInDb.SupportedSports = Ground.SupportedSports;
            groundInDb.IsActive = Ground.IsActive;

            if (Photo != null && Photo.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(Photo.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Photo.CopyToAsync(stream);
                }

                groundInDb.PhotoPath = "/uploads/" + uniqueFileName;
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("/Grounds/Index");
        }
    }
}
