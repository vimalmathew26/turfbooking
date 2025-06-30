using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using turfbooking.Data;
using turfbooking.Models;

namespace turfbooking.Pages.Booking
{
    [Authorize(Roles = "User")]
    public class MyBookingsModel : PageModel
    {
        private readonly AppDbContext _context;

        public MyBookingsModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Models.Booking> Bookings { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            HttpContext.Session.SetString("PreviousPage", Url.Page("/Users/UserDashboard"));

            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                ModelState.AddModelError(string.Empty, "User authentication required.");
                return Page();
            }

            Bookings = await _context.Bookings
                .Where(b => b.UserId == userId)
                .Include(b => b.Ground)
                .Include(b=>b.Slot)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            if (!Bookings.Any())
            {
                ModelState.AddModelError(string.Empty, "No Bookings Found");
                return Page();
            }

            return Page();
        }


        public async Task<IActionResult> OnPostCancelAsync(int bookingId)
        {
            var booking = await _context.Bookings
                .Include(b => b.Ground)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null || booking.Status == BookingStatus.Cancelled)
            {
                ModelState.AddModelError(string.Empty, "Booking Not Found.");
                return Page();
            }
            var slotDateTime = booking.BookingDate.Add(booking.StartTime);
            if (DateTime.Now >= slotDateTime.AddHours(-24))
            {
                TempData["ErrorMessage"] = "Cannot cancel the booking. Cancellations must be made at least 24 hours in advance.";
                return RedirectToPage();
            }

            booking.Status = BookingStatus.Cancelled;

            var slot = await _context.Slots.FirstOrDefaultAsync(s => s.BookingId == booking.Id);
            if (slot != null)
            {
                slot.Status = Slot.SlotStatus.Available;
                slot.BookingId = null;
            }

            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

    }
}
