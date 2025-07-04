using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using turfbooking.Data;
using turfbooking.Helper;
using turfbooking.Models;

namespace turfbooking.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class AddSlotModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly DefaultSlots _defaultSlots;

        public AddSlotModel(AppDbContext context,DefaultSlots defaultSlots)
        {
            _context = context;
            _defaultSlots = defaultSlots;
        }
        [BindProperty(SupportsGet = true)]
        public int? CourtId { get; set; }  

        [BindProperty(SupportsGet = true)]
        public int? GroundId { get; set; }

        [BindProperty]
        public Slot Slot { get; set; }

        public Court Court { get; set; }

        public Ground Ground { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!GroundId.HasValue)
            {
                ModelState.AddModelError(string.Empty, "The Ground Not Found");
                return Page();
            }
            var previousUrl = Url.Page(
                "/Admin/SlotManagement",
                pageHandler: null,
                values: new { GroundId = GroundId.Value,CourtId=CourtId.Value },
                protocol: Request.Scheme
            );
            HttpContext.Session.SetString("PreviousPage", previousUrl);

            Ground = await _context.Grounds.FindAsync(GroundId.Value);
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!GroundId.HasValue)
            {
                ModelState.AddModelError(string.Empty, "The Ground Not Found");
                return Page();
            }
            if (!CourtId.HasValue)
            {
                ModelState.AddModelError(string.Empty, "The Court Not Found");
                return Page();
            }
            Court = await _context.Courts.FirstOrDefaultAsync(c => c.Id == CourtId.Value);

            Ground = await _context.Grounds.FindAsync(GroundId.Value);

            if (Slot.BookingDate < DateTime.Today)
            {
                ModelState.AddModelError("Slot.BookingDate", "Cant add slot on previous day");
            }
            if (Slot.EndTime <= Slot.StartTime)
            {
                ModelState.AddModelError("Slot.EndTime", "Cant add EndTIme before StartTime");
            }
            if (!ModelState.IsValid)
            {
                return Page();
            }           
            if ((Slot.StartTime <= Court.StartTime.TimeOfDay || Slot.EndTime <= Court.StartTime.TimeOfDay) && (Slot.StartTime >= Court.EndTime.TimeOfDay || Slot.EndTime >=Court.EndTime.TimeOfDay)) {
                if ((Slot.EndTime-Slot.StartTime).TotalHours>=1) 
                {
                    Slot.Status = Slot.SlotStatus.Available;
                    Slot.GroundId = GroundId.Value;
                    Slot.CourtId = CourtId.Value;
                    _context.Slots.Add(Slot);
                    await _context.SaveChangesAsync();
              
                }
                else
                {
                    ModelState.AddModelError(string.Empty,"Only Allowed to add a slot with minimum duration of 1 hours");
                    return Page();
                }
                
            }
            else
            {
                ModelState.AddModelError(string.Empty,"Can't Add Ground Slot Between Default Time Slot");
                return Page();
            }
                return RedirectToPage("/Admin/SlotManagement", new { GroundId = GroundId ,CourtId=CourtId });

        }

    }
}