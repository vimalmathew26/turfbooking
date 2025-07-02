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

        [BindProperty(SupportsGet =true)]
        public int? CourtId { set; get; } 

        [BindProperty(SupportsGet = true)]
        public DateTime? SelectedDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? GroundId { get; set; }

        [BindProperty(SupportsGet =true)]
        public int? SlotId { get; set; }
        public Ground Ground { get; set; }
        public List<Slot> Slots { get; set; }
        public List<Court> Courts { get; set; }
        public List<DateTime> SlotDates { get; set; }=new List<DateTime>();       
        public async Task<IActionResult> OnGetAsync()
        {
            var previousUrl = Url.Page(
                "/Admin/GroundCourts",
                pageHandler: null,
                values: new { GroundId = GroundId },
                protocol: Request.Scheme
            );
            HttpContext.Session.SetString("PreviousPage", previousUrl);

            if (GroundId==null)
            {
                ModelState.AddModelError(string.Empty, "Ground Not Found.");
                return Page();
            }
            if (CourtId==null)
            {
                ModelState.AddModelError(string.Empty, "Court Not Found.");
                return Page();
            }
            await _defaultSlots.SetDefaultSlots(GroundId.Value,CourtId.Value);

            Ground = await _context.Grounds.FindAsync(GroundId);

            SlotDates = await _context.Slots
                        .Where(s => s.GroundId == GroundId && s.CourtId==CourtId)
                        .Select(s => s.BookingDate.Date)
                        .Distinct()
                        .OrderBy(s => s)
                        .ToListAsync();

            if (SelectedDate==null && SlotDates.Any())
            {
                SelectedDate = SlotDates.First();
            }
            if (SelectedDate!=null)
            {
                Slots = await _context.Slots
                       .Include(s => s.Ground)
                       .Include(s => s.Booking)
                       .ThenInclude(s => s.User)
                       .Where(s => s.BookingDate.Date == SelectedDate.Value.Date && s.GroundId == GroundId && s.CourtId==CourtId)
                       .ToListAsync();
            }           
            return Page();
        }
        public async Task<IActionResult> OnPostBlockAsync()
        {
            var slot = await _context.Slots.FindAsync(SlotId);
                      
            if (slot != null && (slot.Status == Slot.SlotStatus.Available))
            {
                slot.Status = Slot.SlotStatus.Blocked;
                await _context.SaveChangesAsync();
            }
            return RedirectToPage("./SlotManagement", new { GroundId, SelectedDate, CourtId });
        }
        public async Task<IActionResult> OnPostEnableAsync()
        {
            var slot = await _context.Slots.FindAsync(SlotId);
             
            if (slot != null && slot.Status == Slot.SlotStatus.Blocked)
            {
                slot.Status = Slot.SlotStatus.Available;
                await _context.SaveChangesAsync();
            }
            return RedirectToPage("./SlotManagement", new { GroundId, SelectedDate, CourtId});
        }
        //public async Task<IActionResult> OnPostDeleteAsync()
        //{
        //    var slot = await _context.Slots.FindAsync(SlotId);
                         
        //    if (slot != null)
        //    {
        //        _context.Slots.Remove(slot);
        //        await _context.SaveChangesAsync();
        //    }
        //    return RedirectToPage("./SlotManagement", new { GroundId, SelectedDate ,CourtId});
        //}
    }
}