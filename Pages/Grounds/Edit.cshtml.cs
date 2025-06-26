using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using turfbooking.Data;
using turfbooking.Helper;
using turfbooking.Models;

namespace turfbooking.Pages.Grounds
{
    [Authorize(Roles = "Admin")]
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

        public string[] SelectedSports { get; set; } = Array.Empty<string>();

        [BindProperty]
        [Range(1, 12, ErrorMessage = "Slot duration must be between 1 and 12 hours.")]
        public int SlotDurationHours { get; set; }

        [BindProperty]
        [Range(0, 59, ErrorMessage = "Minutes must be between 0 and 59.")]
        public int SlotDurationMinutes { get; set; }

       


        public async Task<IActionResult> OnGetAsync(int id)
        {

            var ground = await _context.Grounds.FindAsync(id);
            if (ground == null)
                return NotFound();

            Ground = ground;

            SlotDurationHours = Ground.SlotDuration.Hours;
            SlotDurationMinutes = Ground.SlotDuration.Minutes;



            if (!string.IsNullOrWhiteSpace(Ground.SupportedSports))
                SelectedSports = Ground.SupportedSports.Split(",", StringSplitOptions.TrimEntries);

      
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var groundInDb = await _context.Grounds.FindAsync(Ground.Id);
            if (groundInDb == null)
                return NotFound();
            if (Photo == null)
            {
                Ground.PhotoPath = groundInDb.PhotoPath;
            }

            if (!ModelState.IsValid)
            {
                Console.WriteLine("Error");
                return Page();
            }

            Ground.SlotDuration = new TimeSpan(SlotDurationHours, SlotDurationMinutes, 0);


            // Parse StartTime and EndTime as in Create
            groundInDb.StartTime = DateTime.Today.Add(TimeSpan.Parse(Request.Form["Ground.StartTime"]));
            groundInDb.EndTime = DateTime.Today.Add(TimeSpan.Parse(Request.Form["Ground.EndTime"]));

            groundInDb.GroundName = Ground.GroundName;
            groundInDb.Location = Ground.Location;
            groundInDb.Description = Ground.Description;
            groundInDb.PricePerHour = Ground.PricePerHour;
            groundInDb.SupportedSports = Ground.SupportedSports;
            groundInDb.IsActive = Ground.IsActive;
            groundInDb.SlotDuration = Ground.SlotDuration;
            groundInDb.StartTime = Ground.StartTime;
            groundInDb.EndTime = Ground.EndTime;

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

            var slotHelper = new DefaultSlots(_context);
            await slotHelper.SetDefaultSlots(Ground.Id);

            return RedirectToPage("/Grounds/Index");
        }
    }
}
