using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using turfbooking.Data;
using turfbooking.Models;

namespace turfbooking.Pages.Grounds
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public CreateModel(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

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
            SlotDuration = TimeSpan.FromMinutes(60), // Default: 1 hour
            StartTime = DateTime.Today.AddHours(6),  // Default: 6 AM today
            EndTime = DateTime.Today.AddHours(22)    // Default: 10 PM today
        };

        [BindProperty]
        [Required(ErrorMessage = "Photo is required")]
        [Display(Name = "Upload Photo")]
        public required IFormFile Photo { get; set; }

        [BindProperty]
        [Range(1, 12, ErrorMessage = "Slot duration must be between 1 and 12 hours.")]
        public int SlotDurationHours { get; set; }

        public IActionResult OnGet()
        {
            SlotDurationHours = (int)Ground.SlotDuration.TotalHours;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Ground.SlotDuration = TimeSpan.FromHours(SlotDurationHours);

            // For StartTime and EndTime, if you only want the time part:
            Ground.StartTime = DateTime.Today.Add(TimeSpan.Parse(Request.Form["Ground.StartTime"]));
            Ground.EndTime = DateTime.Today.Add(TimeSpan.Parse(Request.Form["Ground.EndTime"]));

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("Photo", "Photo is required.");
                return Page();
            }

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsFolder); // ? ENSURE folder exists

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(Photo.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            try
            {
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await Photo.CopyToAsync(fileStream);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("?? Upload error: " + ex.Message); 
                ModelState.AddModelError("Photo", "An error occurred while uploading the photo. Please try again.");
                return Page();
            }

            Ground.PhotoPath = "/uploads/" + uniqueFileName;

            _context.Grounds.Add(Ground);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Grounds/Index");
        }
    }
}
