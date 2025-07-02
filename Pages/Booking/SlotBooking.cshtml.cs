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
        [BindProperty(SupportsGet =true)]
        public int CourtId {get;set;}

        [BindProperty(SupportsGet =true)]
        public DateTime SelectedDate { get; set; }


        [BindProperty(SupportsGet = true)]
        public int GroundId { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime Date { get; set; }
        public List<DateTime> Next7Days { get; set; }
        public TimeSpan CurrentTime { get; set; }
        public List<Slot> AvailableSlots { get; set; } = new List<Slot>();

        [BindProperty(SupportsGet = true)]
        public int slotId { get; set; }

        public Ground Ground { get; set; }

        public Court Court { get; set; }    

        public async Task<IActionResult> OnGetAsync()
        {
            Ground = await _context.Grounds.FindAsync(GroundId);

            Court=await _context.Courts.FirstOrDefaultAsync(c => c.Id == CourtId);

            if (Ground==null)
            {
                ModelState.AddModelError(string.Empty,"Ground Not Found");
                return Page();
            }
            if (Court == null)
            {
                ModelState.AddModelError(string.Empty, "Court Not Found");
                return Page();
            }

            var previousUrl = Url.Page(
                 "/Users/CourtList",
                 pageHandler: null,
                 values: new { GroundId = GroundId ,CourtId=CourtId},
                 protocol: Request.Scheme
            );

            HttpContext.Session.SetString("PreviousPage",previousUrl);

            await _defaultSlots.SetDefaultSlots(GroundId,CourtId);
            
            CurrentTime = DateTime.Now.TimeOfDay;

            if (Date == default)
            {
                Date = DateTime.Today;
            }

            Next7Days = Enumerable.Range(0, 7)
                       .Select(i => DateTime.Today.AddDays(i))
                       .ToList();
            
            AvailableSlots = await _context.Slots
                            .Where(s => s.GroundId == GroundId&& s.BookingDate.Date == Date.Date && s.CourtId==CourtId)
                            .OrderBy(s => s.StartTime)
                            .ToListAsync();
          
            if (!AvailableSlots.Any())
            {
                ModelState.AddModelError(string.Empty, "No Slots Available for the Selected Date");
                return Page();
           }           
            return Page();
        }
        public async Task<IActionResult> OnPostBookAsync()
        {
         
            var slot = await _context.Slots
                       .Include(s => s.Ground)
                       .Include(c=>c.Court)
                       .FirstOrDefaultAsync(s => s.Id == slotId);

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
                CourtId = slot.CourtId,
                BookingDate = slot.BookingDate,
                StartTime = slot.StartTime,
                EndTime = slot.EndTime,
                TotalPrice = (decimal)(slot.EndTime - slot.StartTime).TotalHours * slot.Court.PricePerHour,
                Status = BookingStatus.Confirmed,
                SlotId = slotId
            };
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            slot.Status = Slot.SlotStatus.Booked;
            

            await _context.SaveChangesAsync();

            return RedirectToPage("/Booking/BookingConfirmation", new { bookingId = booking.Id });
        }
    }
}