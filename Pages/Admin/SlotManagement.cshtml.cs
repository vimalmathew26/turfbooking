using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using turfbooking.Data;
using turfbooking.Helper;
using turfbooking.Models;

namespace turfbooking.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class SlotManagementModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly DefaultSlots _defaultSlots;

        public SlotManagementModel(AppDbContext context, DefaultSlots defaultSlots)
        {
            _context = context;
            _defaultSlots = defaultSlots;
        }
       

        [BindProperty(SupportsGet = true)]
        public DateTime? SelectedDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public int GroundId { get; set; }

        public Ground Ground { get; set; }

        public List<Slot>? Slots { get; set; }

        public List<DateTime>? SlotDates { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
           
            HttpContext.Session.SetString("PreviousPage", "/Admin/GroundSlot");

            await _defaultSlots.SetDefaultSlots(GroundId);
            Ground = await _context.Grounds.FindAsync(GroundId);

            SlotDates = await _context.Slots
                      .Where(s => s.GroundId == GroundId)
                      .Select(s => s.BookingDate.Date)
                      .Distinct()
                      .OrderBy(s => s)
                      .ToListAsync();

            SlotDates ??= new List<DateTime>();

            if (!SelectedDate.HasValue && SlotDates.Any())
            {
                SelectedDate = SlotDates.First();
            }
            if (SelectedDate.HasValue)
            {
                Slots = await _context.Slots
                   .Include(s => s.Ground)
                   .Include(s => s.Booking)
                   .ThenInclude(s => s.User)
                   .Where(s => s.BookingDate.Date == SelectedDate.Value.Date && s.GroundId == GroundId)
                   .ToListAsync();
            }           



            return Page();
        }

        public async Task<IActionResult> OnPostBlockAsync(int id, int GroundId, DateTime SelectedDate)
        {
            var slot = await _context.Slots.FindAsync(id);
            if (slot != null && (slot.Status == Slot.SlotStatus.Available || slot.Status == Slot.SlotStatus.Booked ))
            {
                slot.Status = Slot.SlotStatus.Blocked;
                await _context.SaveChangesAsync();
            }
            return RedirectToPage("./SlotManagement", new { GroundId, SelectedDate });
        }


        public async Task<IActionResult> OnPostEnableAsync(int id, int GroundId, DateTime SelectedDate)
        {
            var slot = await _context.Slots.FindAsync(id);
            if (slot != null && slot.Status == Slot.SlotStatus.Blocked)
            {
                slot.Status = Slot.SlotStatus.Available;
                await _context.SaveChangesAsync();
            }
            return RedirectToPage("./SlotManagement", new { GroundId, SelectedDate });
        }


        public async Task<IActionResult> OnPostDeleteAsync(int id, int GroundId)
        {
            var slot = await _context.Slots.FindAsync(id);
            if (slot != null)
            {
                _context.Slots.Remove(slot);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage("./SlotManagement", new { GroundId, SelectedDate });
        }
    }
}