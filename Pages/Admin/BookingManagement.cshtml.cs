using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using turfbooking.Data;
using turfbooking.Models;

namespace turfbooking.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class BookingManagementModel : PageModel
    {
        private readonly AppDbContext _context;
        public BookingManagementModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet =true)]
        public int? GroundId { get; set; }

        public string GroundName { get; set; }
        public List<Models.Booking> Bookings { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchUsername { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? SearchDate { get; set; }

        
        public async Task OnGetAsync()
        {
            HttpContext.Session.SetString("PreviousPage", Url.Page("/Admin/GroundBookingManagement"));

            GroundName = await _context.Grounds.
                 Where(s => s.Id == GroundId).
                Select(s=>s.GroundName).
               FirstAsync();

            var query = _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Ground)
                .AsQueryable();

            if (!string.IsNullOrEmpty(SearchUsername))
            {
                query = query.Where(b => b.User.Username.Contains(SearchUsername));
            }

            if (SearchDate.HasValue)
            {
                query = query.Where(b => b.BookingDate.Date == SearchDate.Value.Date);
            }

            if(GroundId.HasValue)
            {
                query = query.Where(b => b.GroundId == GroundId.Value);
            }

           
           
            Bookings = await query.ToListAsync();
           /* Bookings = await _context.Bookings

                .Include(b => b.User)
                .Include(b => b.Ground)
                .Where(b => b.GroundId == GroundId)
                .ToListAsync();
            return Page();*/
        }

        public async Task<IActionResult> OnPostCancelBookingAsync(int bookingId,int GroundId)
        {
            var booking = await _context.Bookings
                .Include(b => b.Slot)
                .FirstOrDefaultAsync(b => b.Id == bookingId);
            if (booking == null)
            {
                TempData["ErrorMessage"] = "Booking not found.";
                return RedirectToPage("BookingManagement");

            }
            
            booking.Slot.Status = Slot.SlotStatus.Available;
            booking.Status = BookingStatus.Cancelled;
            booking.Slot.BookingId = null;
            await _context.SaveChangesAsync();
            return RedirectToPage("BookingManagement", new {GroundId, SearchUsername, SearchDate });
        }
    }
}
