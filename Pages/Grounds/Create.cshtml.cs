using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using turfbooking.Data;
using turfbooking.Helper;
using turfbooking.Models;

namespace turfbooking.Pages.Grounds
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public class CourtInputModel
        {
            [Required]
            public string Name { get; set; } = string.Empty;
            [Required]
            public int DurationHours { get; set; } = 1;
            [Required]
            public int DurationMinutes { get; set; } = 0;
            [Required]
            public string StartTime { get; set; } = "06:00"; // Default start time
            [Required]
            public string EndTime { get; set; } = "22:00"; // Default end time
        }

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
            SlotDuration = TimeSpan.FromMinutes(60),
            StartTime = DateTime.Today.AddHours(6),
            EndTime = DateTime.Today.AddHours(22)
        };

        [BindProperty]
        [Required(ErrorMessage = "Photo is required")]
        [Display(Name = "Upload Photo")]
        public required IFormFile Photo { get; set; }

        [BindProperty]
        public List<CourtInputModel> Courts { get; set; } = new();
        public IActionResult OnGet()
        {
            HttpContext.Session.SetString("PreviousPage", Url.Page("/Grounds/Index"));
            return Page();
        }
       
        public async Task<IActionResult> OnPostAsync()
        {
            var slotHelper = new DefaultSlots(_context);
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Please correct the errors in the form.");
                return Page();
            }

            var uniqueSports = Courts
               .Select(c => c.Name.Trim())
               .Where(name => !string.IsNullOrWhiteSpace(name))
               .Distinct(StringComparer.OrdinalIgnoreCase);

            Ground.SupportedSports = string.Join(", ", uniqueSports);

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsFolder);

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

            foreach (var courtInput in Courts)
            {
                var court = new Court
                {
                    Name = courtInput.Name,
                    StartTime = DateTime.Today.Add(TimeSpan.Parse(courtInput.StartTime)),
                    EndTime = DateTime.Today.Add(TimeSpan.Parse(courtInput.EndTime)),
                    Duration = new TimeSpan(courtInput.DurationHours, courtInput.DurationMinutes, 0),
                    GroundId = Ground.Id
                };
                
                _context.Courts.Add(court);
                await _context.SaveChangesAsync();
                await slotHelper.SetDefaultSlots(Ground.Id, court.Id);

            }
            




            return RedirectToPage("/Grounds/Index");
        }
    }
}
