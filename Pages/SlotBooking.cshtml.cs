using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using turfbooking.Models;
using turfbooking.Data;


public class SlotBookingModel : PageModel
{
    private readonly AppDbContext _context;

    public SlotBookingModel(AppDbContext context)
    {
        _context = context;
    }

    [BindProperty(SupportsGet = true)]
    public int GroundId { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime Date { get; set; }

    public TimeSpan CurrentTime { get; set; }

    public List<Slot> AvailableSlots { get; set; }

    public async Task OnGetAsync()
    {

        if (Date == default)
        {
            Date = DateTime.Today;
        }
        Next7Days = Enumerable.Range(0, 7)
            .Select(i => DateTime.Today.AddDays(i))
            .ToList();

        //var expiredSlots = await _context.Slots
        //    slot.IsBooked = false;
        //    slot.BookingId = null;
        //}

        //await _context.SaveChangesAsync();

   
        AvailableSlots = await _context.Slots
       .Where(s => s.GroundId == GroundId
&& s.BookingDate.Date == Date.Date)
       .OrderBy(s => s.StartTime)
       .ToListAsync();
        var slot = await _context.Slots
                   .Include(s => s.Ground)
                   .FirstOrDefaultAsync(s => s.Id == slotId);

    }


    public async Task<IActionResult> OnPostBookAsync(int slotId)
    {
        var slot = await _context.Slots.Include(s=>s.Ground).FirstOrDefaultAsync(s => s.Id == slotId);

        if (slot == null || slot.IsBooked)
        {
            return NotFound();
        }
        var userIdClaim = User.FindFirst("UserId");
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            ModelState.AddModelError(string.Empty, "User authentication required.");
            return Page();
        }

        var booking = new Booking
        {
            UserId = userId,
            GroundId = slot.GroundId,
            BookingDate = slot.BookingDate,
            StartTime = slot.StartTime,
            EndTime = slot.EndTime,
            TotalPrice = (decimal)(slot.EndTime - slot.StartTime).TotalHours * slot.Ground.PricePerHour,
            Status = BookingStatus.Confirmed
        };

        _context.Bookings.Add(booking);
        return RedirectToPage("/Booking/BookingConfirmation", new { bookingId = booking.Id });

        slot.IsBooked = true;
        slot.BookingId = booking.Id;

        await _context.SaveChangesAsync();

        return RedirectToPage("BookingConfirmation", new { bookingId = booking.Id });
    }
}