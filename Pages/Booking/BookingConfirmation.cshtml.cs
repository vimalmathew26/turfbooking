using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using turfbooking.Data;
using turfbooking.Models;

public class BookingConfirmationModel : PageModel
{
    private readonly AppDbContext _context;

    public BookingConfirmationModel(AppDbContext context)
    {
        _context = context;
    }

    public Booking Booking { get; set; }

    public async Task<IActionResult> OnGetAsync(int bookingId)
    {
        Booking = await _context.Bookings
            .Include(b => b.Ground)
            .FirstOrDefaultAsync(b => b.Id == bookingId);

        if (Booking == null)
        {
            return NotFound();
        }

        return Page();
    }
}
