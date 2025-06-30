using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using turfbooking.Data;
using turfbooking.Models;
using turfbooking.Helper;

namespace turfbooking.Pages.Booking
{
    [Authorize(Roles = "User")]
    public class SlotBookingModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly DefaultSlots _defaultSlots;

        public SlotBookingModel(AppDbContext context,DefaultSlots defaultSlots)
        {
            _context = context;
            _defaultSlots = defaultSlots;

        }       
        [BindProperty(SupportsGet = true)]
        public int GroundId { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime Date { get; set; }
        public List<DateTime> Next7Days { get; set; }
        public TimeSpan CurrentTime { get; set; }
        public List<Slot> AvailableSlots { get; set; }

        public async Task OnGetAsync()
        {
            var previousUrl = Url.Page(
     "/Users/GroundDetails",
     pageHandler: null,
     values: new { id = GroundId },
     protocol: Request.Scheme
 );

            HttpContext.Session.SetString("PreviousPage",previousUrl);
            await _defaultSlots.SetDefaultSlots(GroundId);
            CurrentTime = DateTime.Now.TimeOfDay;

            if (Date == default)
            {
                Date = DateTime.Today;
            }

            Next7Days = Enumerable.Range(0, 7)
                .Select(i => DateTime.Today.AddDays(i))
                .ToList();
            
            AvailableSlots = await _context.Slots
           .Where(s => s.GroundId == GroundId
                    && s.BookingDate.Date == Date.Date)
           .OrderBy(s => s.StartTime)
           .ToListAsync();           
        }

        public async Task<IActionResult> OnPostBookAsync(int slotId)
        {

            var slot = await _context.Slots
                       .Include(s => s.Ground)
                       .FirstOrDefaultAsync(s => s.Id == slotId);

            if (slot == null || slot.Status == Slot.SlotStatus.Booked)
            {
                ModelState.AddModelError(string.Empty,"Selected Slot Not Available");
                return Page();
            }
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                ModelState.AddModelError(string.Empty, "User authentication required.");
                return Page();
            }

            var booking = new turfbooking.Models.Booking
            {
                UserId = userId,
                GroundId = slot.GroundId,
                BookingDate = slot.BookingDate,
                StartTime = slot.StartTime,
                EndTime = slot.EndTime,
                TotalPrice = (decimal)(slot.EndTime - slot.StartTime).TotalHours * slot.Ground.PricePerHour,
                Status = BookingStatus.Confirmed,
                SlotId = slotId
            };


            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            slot.Status = Slot.SlotStatus.Booked;
            slot.BookingId = booking.Id;

            await _context.SaveChangesAsync();

            return RedirectToPage("/Booking/BookingConfirmation", new { bookingId = booking.Id });
        }
    }
}