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
        public int GroundId { get; set; }

        [BindProperty]
        public Slot Slot { get; set; }


        public Ground Ground { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var previousUrl = Url.Page(
                "/Admin/SlotManagement",
                pageHandler: null,
                values: new { GroundId = GroundId },
                protocol: Request.Scheme
            );

            HttpContext.Session.SetString("PreviousPage", previousUrl);
            Ground = await _context.Grounds.FindAsync(GroundId);
            if (Ground == null)
            {
                ModelState.AddModelError(string.Empty, "The Ground Not Found");
                return Page();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Ground = await _context.Grounds.FindAsync(GroundId);

            if (Ground == null)
            {
                ModelState.AddModelError(string.Empty, "The Ground Not Found");
                return Page();
            }

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
            if ((Slot.StartTime <= Ground.StartTime.TimeOfDay || Slot.StartTime >= Ground.EndTime.TimeOfDay) && (Slot.EndTime<=Ground.StartTime.TimeOfDay || Slot.EndTime >=Ground.EndTime.TimeOfDay)) {
                if ((Slot.EndTime-Slot.StartTime).TotalHours>=1) 
                {
                    Slot.Status = Slot.SlotStatus.Available;
                    Slot.GroundId = GroundId;
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
                return RedirectToPage("/Admin/SlotManagement", new { GroundId = GroundId });

        }

    }
}