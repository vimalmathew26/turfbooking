using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using turfbooking.Data;
using turfbooking.Helper;
using turfbooking.Models;

namespace turfbooking.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class BookingManagementModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly SendMail _sendMail;
        public BookingManagementModel(AppDbContext context, SendMail sendMail)
        {
            _context = context;
            _sendMail = sendMail;
        }

        [BindProperty(SupportsGet =true)]
        public int? GroundId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? CourtId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? bookingId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchUsername { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? SearchDate { get; set; }
        public List<Models.Booking> Bookings { get; set; }
        public Ground Ground { get; set; }
    
        public async Task<IActionResult> OnGetAsync()
        {
            if (!GroundId.HasValue )
            {
                ModelState.AddModelError(string.Empty, "The Ground Not Found");
                return Page();
            }
            if (!CourtId.HasValue)
            {
                ModelState.AddModelError(string.Empty, "The Court Not Found");
                return Page();
            }

            var previousUrl = Url.Page(
                "/Admin/GroundCourts",
                pageHandler: null,
                values: new { GroundId = GroundId.Value, CourtId = CourtId.Value },
                protocol: Request.Scheme
            );
            HttpContext.Session.SetString("PreviousPage", previousUrl);

            Ground = await _context.Grounds
                    .FirstOrDefaultAsync(s=>s.Id==GroundId.Value);

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
            
             query = query.Where(b => b.GroundId == GroundId && b.CourtId==CourtId);
                      
            Bookings = await query.ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostCancelBookingAsync()
        {
            if (!bookingId.HasValue)
            {
                ModelState.AddModelError(string.Empty, "Booking ID is required.");
                return Page();
            }
            var booking = await _context.Bookings
                .Include(b => b.Slot)
                .Include(b => b.Ground)
                .Include(b => b.Court)
                .FirstOrDefaultAsync(b => b.Id == bookingId);
           
            User BookedUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == booking.UserId);
            var fullDateTime = booking.Slot.BookingDate;
            string formattedDate = fullDateTime.ToString("dd/MM/yyyy");
            string subject = "Booking cancelled by Admin";
            string body = $"<p>Hi {BookedUser.Username},</p>" +
                          "<p>Your Booking have been cancelled by the Admin.</p>" +
                          "<p>Contact Admin to know the exact issue.</p>" +
                          "<p>Cancelled booking details:</p>" +
                           $"<p>Ground: {booking.Ground.GroundName} </p>" +
                          $"<p>Court: {booking.Court.Name} </p>" +
                          $"<p>Date: {formattedDate} </p>" +
                          $"<p>Time: {booking.Slot.StartTime} - {booking.Slot.EndTime} </p>" +
                          $"<p>Price: {booking.TotalPrice} </p>";
            string recipient = BookedUser.Email;

            var emailSuccess = await _sendMail.SendAsync(recipient, subject, body);

            if (!emailSuccess)
            {
                TempData["Message"] = "Booking cancellation successful, but email could not be sent.";
            }
            else
            {
                TempData["Message"] = "Booking cancellation successful. A confirmation email was sent.";
            }

            booking.Slot.Status = Slot.SlotStatus.Available;
            booking.Status = BookingStatus.Cancelled;
            booking.SlotId = null;
            await _context.SaveChangesAsync();
            return RedirectToPage("BookingManagement", new {GroundId, SearchUsername, SearchDate ,CourtId});
        }
    }
}
