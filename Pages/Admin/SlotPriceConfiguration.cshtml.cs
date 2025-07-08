using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using turfbooking.Data;
using turfbooking.Models;

namespace turfbooking.Pages.Admin
{
    public class SlotPriceConfigurationModel : PageModel
    {
        private readonly AppDbContext _context;

        public SlotPriceConfigurationModel(AppDbContext context)
        {
            _context = context;
        }

        // Keep SupportsGet = true so the page loads initially
        [BindProperty(SupportsGet = true)]
        public int CourtId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int GroundId { get; set; }

        [BindProperty]
        public SlotPrice SlotPrice { get; set; }

        public List<SlotPrice> SlotPrices { get; set; } = new();
        public Court court { get; set; }

        // A helper method to load data, avoiding code duplication
        private async Task LoadDataAsync()
        {
            court = await _context.Courts
                .Include(c => c.Ground)
                .FirstOrDefaultAsync(c => c.Id == CourtId && c.GroundId == GroundId);

            if (court != null)
            {
                SlotPrices = await _context.SlotPrices
                    .Where(s => s.CourtId == CourtId)
                    .OrderByDescending(s => s.Date) // Or any preferred order
                    .ToListAsync();
            }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadDataAsync();

            if (court == null)
            {
                TempData["ErrorMessage"] = "Court or Ground not found.";
                // Redirect to a safer page if the context is lost
                return RedirectToPage("/Admin/GroundManagement");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            // The form now posts to ?handler=Save, so this is called for new items
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Validation failed. Please check your input.";
                // *** FIX 3: If validation fails, reload data and return the Page() to show errors ***
                await LoadDataAsync();
                return Page();
            }

            // If the "Apply to all dates" checkbox is checked, SlotPrice.Date will be null.
            // If it's unchecked but no date is selected, ModelState would be invalid (if required).
            if (SlotPrice.Date == DateTime.MinValue)
            {
                SlotPrice.Date = null;
            }

            SlotPrice.CourtId = CourtId;
            // Set ID to 0 to ensure EF Core treats it as a new entity
            SlotPrice.Id = 0;

            _context.SlotPrices.Add(SlotPrice);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Slot price added successfully.";
            return RedirectToPage(new { GroundId = this.GroundId, CourtId = this.CourtId });
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            // The form now posts to ?handler=Edit, so this is called for existing items
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Validation failed. Please check your input.";
                await LoadDataAsync();
                return Page();
            }

            if (SlotPrice.Date == DateTime.MinValue)
            {
                SlotPrice.Date = null;
            }

            // Ensure the CourtId is set correctly before updating
            SlotPrice.CourtId = CourtId;

            _context.Attach(SlotPrice).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Slot price updated successfully.";
            }
            catch (DbUpdateConcurrencyException)
            {
                TempData["ErrorMessage"] = "The slot price was modified by another user. Please try again.";
            }

            return RedirectToPage(new { GroundId = this.GroundId, CourtId = this.CourtId });
        }

        // *** FIX 1 (Backend): Update handler to accept IDs from the route ***
        public async Task<IActionResult> OnPostDeleteAsync(int id, int groundId, int courtId)
        {
            var slot = await _context.SlotPrices.FindAsync(id);

            if (slot == null)
            {
                TempData["ErrorMessage"] = "Slot price not found.";
            }
            else
            {
                _context.SlotPrices.Remove(slot);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Slot price deleted successfully.";
            }

            // Use the IDs passed into the handler for the redirect
            return RedirectToPage(new { GroundId = groundId, CourtId = courtId });
        }
    }
}