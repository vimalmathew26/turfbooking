using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using turfbooking.Data;
using turfbooking.Models;
using turfbooking.Helper;


namespace turfbooking.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class ManageUsersModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly SendMail _sendMail;

        public ManageUsersModel(AppDbContext context, SendMail sendMail)
        {
            _context = context;
            Users = new List<User>();
            _sendMail = sendMail;
        }

        public List<User> Users { get; set; }

        public void OnGet()
        {
            HttpContext.Session.SetString("PreviousPage", Url.Page("/Admin/AdminDashboard"));
            Users = _context.Users.ToList();
        }

        public async Task<IActionResult> OnPostDeactivateAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                user.IsActive = false;
                await _context.SaveChangesAsync();

                string subject = "Account Deactivated.";
                string body = $"<p>Hello {user.Username},</p>" +
                          "<p>Your account have been deactivated following your recent activities on the TurfBooking site.</p>" +
                          "<p>Contact Admin to resolve this issue.</p>" +
                          "<p>Best regards,<br/>Turf Booking Team</p>";
                string recipient = user.Email;

                var emailSuccess = await _sendMail.SendAsync(recipient, subject, body);

                if (!emailSuccess)
                {
                    TempData["Message"] = "Deactivation successful, but email could not be sent.";
                }
                else
                {
                    TempData["Message"] = "Deactivation successful. An email was sent.";
                }

                TempData["Message"] = $"{user.Username} has been deactivated.";
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostActivateAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                user.IsActive = true;
                await _context.SaveChangesAsync();
                string subject = "Account Activated.";
                string body = $"<p>Hello {user.Username},</p>" +
                          "<p>Your account have been activated on the TurfBooking site.</p>" +
                          "<p>Enjoy using our site.</p>" +
                          "<p>Best regards,<br/>Turf Booking Team</p>";
                string recipient = user.Email;

                var emailSuccess = await _sendMail.SendAsync(recipient, subject, body);

                if (!emailSuccess)
                {
                    TempData["Message"] = "Activation successful, but email could not be sent.";
                }
                else
                {
                    TempData["Message"] = "Activation successful. An email was sent.";
                }
                TempData["Message"] = $"{user.Username} has been activated.";
            }
            return RedirectToPage();
        }
    }
}
