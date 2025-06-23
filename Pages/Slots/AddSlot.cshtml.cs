using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using turfbooking.Models;
using turfbooking.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding;


namespace turfbooking.Pages.Admin
{

    public class AddSlotModel : PageModel
    {
        private readonly AppDbContext _context;

        public AddSlotModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int GroundId { get; set; }



        [BindProperty]
        public Slot Slot { get; set; }


        public Ground Ground { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {

            Ground = await _context.Grounds.FindAsync(GroundId);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Ground = await _context.Grounds.FindAsync(GroundId);

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
            Slot.Status = Slot.SlotStatus.Available;
            Slot.GroundId = GroundId;
            _context.Slots.Add(Slot);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Slots/AddSlot", new { GroundId = GroundId });

        }

    }
}