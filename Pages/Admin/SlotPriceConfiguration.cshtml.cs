using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using turfbooking.Data;
using turfbooking.Helper;
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
        [BindProperty(SupportsGet = true)]
        public int CourtId { get; set; }

        [BindProperty(SupportsGet = true)]
        public SlotPrice SlotPrice { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            return Page();
        }
        public async Task<IActionResult> OnPostSaveAsync()
        {
            //// Use correct CourtId (bound from page)
            var slotprice = new SlotPrice
            {
                Date = SlotPrice.Date,
                StartTime = SlotPrice.StartTime,
                EndTime = SlotPrice.EndTime,
                Price = SlotPrice.Price,
                CourtId = 3 // ? FIXED
            };

            _context.SlotPrices.Add(slotprice);
            await _context.SaveChangesAsync();

          

            //// Fetch updated slot price list, including the one we just added
            //var slotprices = await _context.SlotPrices
            //    .Where(sp => sp.CourtId == CourtId)
            //    .ToListAsync();

            //var slots = await _context.Slots
            //    .Where(s => s.CourtId == CourtId)
            //    .ToListAsync();

            //foreach (var slot in slots)
            //{
            //    bool matched = false;

            //    // First: check for exact date match
            //    foreach (var slotpr in slotprices.Where(sp => sp.Date != null))
            //    {
            //        if (slot.BookingDate.Date == slotpr.Date.Value.Date &&
            //            slot.StartTime >= slotpr.StartTime &&
            //            slot.EndTime <= slotpr.EndTime)
            //        {
            //            slot.price = slotpr.Price;
            //            matched = true;
            //            break;
            //        }
            //    }

            //    // Second: fallback to null-date slot prices
            //    if (!matched)
            //    {
            //        foreach (var slotpr in slotprices.Where(sp => sp.Date == null))
            //        {
            //            if (slot.StartTime >= slotpr.StartTime &&
            //                slot.EndTime <= slotpr.EndTime)
            //            {
            //                slot.price = slotpr.Price;
            //                break;
            //            }
            //        }
            //    }

            //    _context.Slots.Update(slot); // ? Make sure all updated slots are tracked
            //}

            //await _context.SaveChangesAsync(); // ? Save all at oncec


         
            return RedirectToPage(); // ? Refresh
        }


    }
}
