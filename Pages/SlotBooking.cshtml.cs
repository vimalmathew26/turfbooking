using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using turfbooking.Models;

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

        CurrentTime = DateTime.Now.TimeOfDay;

        Date = new DateTime(2025, 6, 19);

        GroundId = 1;

        var now = DateTime.Now;
         
        var expiredSlots = await _context.Slots
            .Where(s => s.IsBooked && s.BookingDate.Date == now.Date)
            .ToListAsync();
        
        var pastSlots = expiredSlots
            .Where(s => s.BookingDate.Add(s.EndTime) < now)
            .ToList();


        foreach (var slot in pastSlots)
        {
            slot.IsBooked = false;
            slot.BookingId = null;
        }

        await _context.SaveChangesAsync();

   
        AvailableSlots = await _context.Slots
        .Where(s => s.GroundId == GroundId
                 && s.BookingDate.Date == Date.Date)
        .OrderBy(s => s.StartTime)
        .ToListAsync();
    }


    public async Task<IActionResult> OnPostBookAsync(int slotId)
    {
        var slot = await _context.Slots.Include(s=>s.Ground).FirstOrDefaultAsync(s => s.Id == slotId);
        if (slot == null || slot.IsBooked)
            return NotFound();

        var booking = new Booking
        {
            UserId = 2, 
            GroundId = slot.GroundId,
            BookingDate = slot.BookingDate,
            StartTime = slot.StartTime,
            EndTime = slot.EndTime,
            TotalPrice = (decimal)(slot.EndTime - slot.StartTime).TotalHours * slot.Ground.PricePerHour,
            Status = BookingStatus.Confirmed
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        slot.IsBooked = true;
        slot.BookingId = booking.Id;

        await _context.SaveChangesAsync();

        return RedirectToPage("BookingConfirmation", new { bookingId = booking.Id });
    }
}
