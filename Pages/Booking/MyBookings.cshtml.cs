using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using turfbooking.Data;
using turfbooking.Helper;
using turfbooking.Models;

namespace turfbooking.Pages.Booking
{
    [Authorize(Roles = "User")]
    public class MyBookingsModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly SendMail _sendMail;

        public MyBookingsModel(AppDbContext context, SendMail sendMail)
        {
            _context = context;
            _sendMail = sendMail;
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
                .Include(b=>b.Slot)
                .Include(b=>b.Court)
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
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                ModelState.AddModelError(string.Empty, "User authentication required.");
                return Page();
            }
            User CurrentUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            var fullDateTime = booking.Slot.BookingDate;
            string formattedDate = fullDateTime.ToString("dd/MM/yyyy");
            string subject = "Booking cancelled.";
            string body = $"<p>Hi {CurrentUser.Username},</p>" +
                          "<p>Your Booking have been cancelled successfully.</p>" +
                          "<p>Cancelled booking details:</p>" +
                           $"<p>Ground: {booking.Ground.GroundName} </p>" +
                          $"<p>Court: {booking.Court.Name} </p>" +
                          $"<p>Date: {formattedDate} </p>" +
                          $"<p>Time: {booking.Slot.StartTime} - {booking.Slot.EndTime} </p>" +
                          $"<p>Price: {booking.TotalPrice} </p>";
            string recipient = CurrentUser.Email;

            var emailSuccess = await _sendMail.SendAsync(recipient, subject, body);

            if (!emailSuccess)
            {
                TempData["Message"] = "Booking cancellation successful, but email could not be sent.";
            }
            else
            {
                TempData["Message"] = "Booking cancellation successful. A confirmation email was sent.";
            }

            booking.Status = BookingStatus.Cancelled;

            var slot = await _context.Slots.FirstOrDefaultAsync(s => s.Booking != null && s.Booking.Id == booking.Id);
            if (slot != null)
            {
                slot.Status = Slot.SlotStatus.Available;
                booking.SlotId = null;
            }

            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

    }
}
