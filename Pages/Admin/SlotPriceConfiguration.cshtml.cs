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
        [BindProperty(SupportsGet =true)]
        public int? CourtId { get; set; }

        [BindProperty(SupportsGet =true)]
        public int? GroundId { get; set; }

        [BindProperty]
        public SlotPrice SlotPrice { get; set; }
       
        public async Task<IActionResult> OnGetAsync()
        {
            if (!CourtId.HasValue || !GroundId.HasValue)
            {
                ModelState.AddModelError(string.Empty, "Invalid Court or Ground ID.");
                return Page();
            }
            var previousUrl = Url.Page(
                "/Admin/SlotManagement",
                pageHandler: null,
                values: new { GroundId=GroundId.Value,CourtId=CourtId.Value},
                protocol: Request.Scheme
            );

            HttpContext.Session.SetString("PreviousPage", previousUrl);

            return Page();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            if (!CourtId.HasValue )
            {
                ModelState.AddModelError(string.Empty, "Court ID is missing.");
                return Page();
            }
            if (!GroundId.HasValue)
            {
                ModelState.AddModelError(string.Empty, " Ground ID is missing.");
                return Page();
            }
            var slotprice = new SlotPrice
            {
                Date = SlotPrice.Date,
                StartTime = SlotPrice.StartTime,
                EndTime = SlotPrice.EndTime,
                Price = SlotPrice.Price,
                CourtId = CourtId.Value
            };

            _context.SlotPrices.Add(slotprice);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Admin/SlotManagement", new { GroundId = GroundId, CourtId = CourtId });
        }



    }
}
