using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TurfBookingApp.Models;

public class MyBookingsModel : PageModel
{
    private readonly AppDbContext _context;

    public MyBookingsModel(AppDbContext context)
    {
        _context = context;
    }

    public List<Booking> Bookings { get; set; }

    public async Task OnGetAsync()
    {
        int userId = 5; 

        Bookings = await _context.Bookings
            .Where(b => b.UserId == userId)
            .Include(b => b.Ground)
            .OrderByDescending(b => b.BookingDate)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostCancelAsync(int bookingId)
    {
        var booking = await _context.Bookings
            .Include(b => b.Ground)
            .FirstOrDefaultAsync(b => b.Id == bookingId);

        if (booking == null || booking.Status == BookingStatus.Cancelled)
            return NotFound();

        var slotDateTime = booking.BookingDate.Add(booking.StartTime);
        if (DateTime.Now >=  slotDateTime.AddHours(-24))
        {
            TempData["ErrorMessage"] = "Cannot cancel the booking. Cancellations must be made at least 24 hours in advance.";
            return RedirectToPage(); 
        }

        booking.Status = BookingStatus.Cancelled;

        var slot = await _context.Slots.FirstOrDefaultAsync(s => s.BookingId == booking.Id);
        if (slot != null)
        {
            slot.IsBooked = false;
            slot.BookingId = null;
        }

        await _context.SaveChangesAsync();

        return RedirectToPage();
    }



}
