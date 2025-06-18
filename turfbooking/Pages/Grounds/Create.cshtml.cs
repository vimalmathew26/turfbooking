using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using turfbooking.Data;
using turfbooking.Models;

namespace turfbooking.Pages.Grounds
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public CreateModel(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // ? Initialize Ground with required default values here
        [BindProperty]
        public Ground Ground { get; set; } = new Ground
        {
            GroundName = string.Empty,
            Location = string.Empty,
            Description = string.Empty,
            PricePerHour = 0,
            SupportedSports = string.Empty,
            IsActive = true,
            PhotoPath = string.Empty,


        };

        [BindProperty]
        [Required(ErrorMessage = "Photo is required")]
        [Display(Name = "Upload Photo")]
        public required IFormFile Photo { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || Photo == null || Photo.Length == 0)
            {
                if (Photo == null || Photo.Length == 0)
                {
                    ModelState.AddModelError("Photo", "Photo is required.");
                }
                return Page();
            }

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsFolder); // Ensure folder exists

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(Photo.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await Photo.CopyToAsync(fileStream);
            }

            Ground.PhotoPath = "/uploads/" + uniqueFileName;

            _context.Grounds.Add(Ground);

            await _context.SaveChangesAsync();

            return RedirectToPage("/Grounds/Index");
        }
    }
}
