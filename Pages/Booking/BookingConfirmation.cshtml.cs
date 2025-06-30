using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using turfbooking.Data;
using turfbooking.Models;

namespace turfbooking.Pages.Booking
{
    [Authorize(Roles = "User")]
    public class BookingConfirmationModel : PageModel
    {
        private readonly AppDbContext _context;

        public BookingConfirmationModel(AppDbContext context)
        {
            _context = context;
        }

        public Models.Booking Booking { get; set; }

        public async Task<IActionResult> OnGetAsync(int bookingId)
        {
            Booking = await _context.Bookings
                .Include(b => b.Ground)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (Booking==null)
            {
                ModelState.AddModelError(string.Empty,"The Booking was Unsuccessfull !! Please Try Again");
                return Page();
            }

            return Page();
        }
    }
}
