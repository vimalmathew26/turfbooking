using FluentEmail.Smtp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using turfbooking.Data;
using turfbooking.Helper;
using turfbooking.Models;

namespace turfbooking.Pages.Accounts
{
    public class RegisterModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly SendMail _sendMail;

        public RegisterModel(AppDbContext context, SendMail sendMail)
        {
            _context = context;
            _sendMail = sendMail;
            NewUser = new Models.User();
            ConfirmPassword = string.Empty;
        }

        [BindProperty]
        public Models.User NewUser { get; set; }

        [BindProperty]
        public string ConfirmPassword { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Security question is required.")]
        public string SelectedQuestion { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Security answer is required.")]
        public string Answer { get; set; }

        public List<string> SecurityQuestions { get; set; } = new List<string>
        {
            "What is your mother's maiden name?",
            "What was the name of your first pet?",
            "What is your favorite movie?",
            "What was your childhood nickname?",
            "What is the name of your first school?",
            "What is the name of your best friend?",
            "What is the name of your first phone?",
            "What is your favourite book?",
            "What is your dream place?",
            "What is your favourite food?"
        };

        public void OnGet() { }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            if (NewUser.PasswordHash != ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Passwords do not match.");
                return Page();
            }

            if (string.IsNullOrWhiteSpace(SelectedQuestion) || string.IsNullOrWhiteSpace(Answer))
            {
                ModelState.AddModelError(string.Empty, "Security question and answer are required.");
                return Page();
            }

            // Hash password
            var salt = PasswordHelper.GenerateSalt();
            var hashedPassword = PasswordHelper.HashPasswordWithSHA256(NewUser.PasswordHash, salt);
            NewUser.PasswordHash = $"{salt}:{hashedPassword}";

            // Hash security answer
            NewUser.SecurityQuestion = SelectedQuestion;
            var answerSalt = PasswordHelper.GenerateSalt();
            var hashedAnswer = PasswordHelper.HashPasswordWithSHA256(Answer.Trim(), answerSalt);
            NewUser.SecurityAnswer = $"{answerSalt}:{hashedAnswer}";

            NewUser.Role = "User";
            NewUser.IsActive = true;

            _context.Users.Add(NewUser);
            _context.SaveChanges();

            string subject = "Welcome to Turf Booking!";
            string body = $"<p>Dear {NewUser.Username},</p>" +
                          "<p>Thank you for registering with Turf Booking. We are excited to have you on board!</p>" +
                          "<p>You can now log in and start booking your favorite grounds.</p>" +
                          "<p>Best regards,<br/>Turf Booking Team</p>";
            string recipient = NewUser.Email;

            var emailSuccess = await _sendMail.SendAsync(recipient, subject, body);

            if (!emailSuccess)
            {
                TempData["Message"] = "Registration successful, but email could not be sent.";
            }
            else
            {
                TempData["Message"] = "Registration successful. A confirmation email was sent.";
            }

            return RedirectToPage("/Accounts/Login");
        }
    }
}