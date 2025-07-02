using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using turfbooking.Data;
using turfbooking.Models;

namespace turfbooking.Pages.Grounds
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public class CourtInputModel
        {
            public int Id { get; set; } = 0; // 0 for new courts, existing ID for existing courts
            [Required]
            public string Name { get; set; } = string.Empty;
            [Required]
            public int DurationHours { get; set; } = 1;
            [Required]
            public int DurationMinutes { get; set; } = 0;
            [Required]
            public string StartTime { get; set; } = "06:00";
            [Required]
            public string EndTime { get; set; } = "22:00";
        }

        public EditModel(AppDbContext context, IWebHostEnvironment environment)
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
        [Display(Name = "Upload Photo")]
        public IFormFile? Photo { get; set; }

        [BindProperty]
        public List<CourtInputModel> Courts { get; set; } = new();

        private readonly List<string> PredefinedSports = new()
        {
            "Football", "Cricket", "Badminton", "Volleyball", "Tennis",
            "Basketball", "Hockey", "Kabaddi", "Table Tennis", "Kho-Kho"
        };

        public bool IsCustomSport(string sportName)
        {
            return !string.IsNullOrEmpty(sportName) && !PredefinedSports.Contains(sportName);
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ground = await _context.Grounds
                .Include(g => g.Courts)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ground == null)
            {
                return NotFound();
            }

            Ground = ground;

            // Convert existing courts to input models
            Courts = ground.Courts.Select(c => new CourtInputModel
            {
                Id = c.Id,
                Name = c.Name,
                DurationHours = c.Duration.Hours,
                DurationMinutes = c.Duration.Minutes,
                StartTime = c.StartTime.ToString("HH:mm"),
                EndTime = c.EndTime.ToString("HH:mm")
            }).ToList();

            // Ensure at least one court exists
            if (!Courts.Any())
            {
                Courts.Add(new CourtInputModel());
            }

            HttpContext.Session.SetString("PreviousPage", Url.Page("/Grounds/Index"));
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Please correct the errors in the form.");
                return Page();
            }

            // Get the existing ground with courts
            var existingGround = await _context.Grounds
                .Include(g => g.Courts)
                .FirstOrDefaultAsync(g => g.Id == Ground.Id);

            if (existingGround == null)
            {
                return NotFound();
            }

            // Handle photo upload if new photo is provided
            if (Photo != null)
            {
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

                    // Delete old photo if it exists
                    if (!string.IsNullOrEmpty(existingGround.PhotoPath) && existingGround.PhotoPath != "/images/placeholder.png")
                    {
                        var oldPhotoPath = Path.Combine(_environment.WebRootPath, existingGround.PhotoPath.TrimStart('/'));
                        if (System.IO.File.Exists(oldPhotoPath))
                        {
                            System.IO.File.Delete(oldPhotoPath);
                        }
                    }

                    existingGround.PhotoPath = "/uploads/" + uniqueFileName;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("?? Upload error: " + ex.Message);
                    ModelState.AddModelError("Photo", "An error occurred while uploading the photo. Please try again.");
                    return Page();
                }
            }

            // Update ground properties
            existingGround.GroundName = Ground.GroundName;
            existingGround.Location = Ground.Location;
            existingGround.Description = Ground.Description;
            existingGround.PricePerHour = Ground.PricePerHour;
            existingGround.IsActive = Ground.IsActive;

            var uniqueSports = Courts
                .Select(c => c.Name.Trim())
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Distinct(StringComparer.OrdinalIgnoreCase);

            existingGround.SupportedSports = string.Join(", ", uniqueSports);

            // Handle courts updates
            var existingCourtIds = existingGround.Courts.Select(c => c.Id).ToList();
            var submittedCourtIds = Courts.Where(c => c.Id > 0).Select(c => c.Id).ToList();

            // Remove courts that are no longer in the submitted list
            var courtsToRemove = existingGround.Courts.Where(c => !submittedCourtIds.Contains(c.Id)).ToList();
            foreach (var court in courtsToRemove)
            {
                _context.Courts.Remove(court);
            }

            // Update existing courts and add new courts
            foreach (var courtInput in Courts)
            {
                if (courtInput.Id > 0)
                {
                    // Update existing court
                    var existingCourt = existingGround.Courts.FirstOrDefault(c => c.Id == courtInput.Id);
                    if (existingCourt != null)
                    {
                        existingCourt.Name = courtInput.Name;
                        existingCourt.StartTime = DateTime.Today.Add(TimeSpan.Parse(courtInput.StartTime));
                        existingCourt.EndTime = DateTime.Today.Add(TimeSpan.Parse(courtInput.EndTime));
                        existingCourt.Duration = new TimeSpan(courtInput.DurationHours, courtInput.DurationMinutes, 0);
                    }
                }
                else
                {
                    // Add new court
                    var newCourt = new Court
                    {
                        Name = courtInput.Name,
                        StartTime = DateTime.Today.Add(TimeSpan.Parse(courtInput.StartTime)),
                        EndTime = DateTime.Today.Add(TimeSpan.Parse(courtInput.EndTime)),
                        Duration = new TimeSpan(courtInput.DurationHours, courtInput.DurationMinutes, 0),
                        GroundId = existingGround.Id
                    };

                    _context.Courts.Add(newCourt);
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GroundExists(Ground.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("/Grounds/Index");
        }

        private bool GroundExists(int id)
        {
            return _context.Grounds.Any(e => e.Id == id);
        }
    }
}