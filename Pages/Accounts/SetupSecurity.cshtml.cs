using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using turfbooking.Data;
using turfbooking.Helper;

public class SetupSecurityModel : PageModel
{
    private readonly AppDbContext _context;

    public SetupSecurityModel(AppDbContext context)
    {
        _context = context;
        SelectedQuestion = string.Empty;
        Answer = string.Empty;
        
    }

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
        "what is your favourite food?"
    };

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        if (string.IsNullOrWhiteSpace(SelectedQuestion) || string.IsNullOrWhiteSpace(Answer))
        {
            ModelState.AddModelError(string.Empty, "Both fields are required.");
            return Page();
        }

        var email = TempData["email"] as string;
        if (email == null) return RedirectToPage("/Accounts/Login");

        var user = _context.Users.FirstOrDefault(u => u.Email == email);
        Console.WriteLine(user);
        if (user == null) return RedirectToPage("/Accounts/Login");

        user.SecurityQuestion = SelectedQuestion;
        user.SecurityAnswer = Answer.Trim();
        var answerSalt = PasswordHelper.GenerateSalt();
        var hashedAnswer = PasswordHelper.HashPasswordWithSHA256(user.SecurityAnswer, answerSalt);
        user.SecurityAnswer = $"{answerSalt}:{hashedAnswer}";
        user.IsActive = true;

        _context.SaveChanges();


        TempData["Message"] = "Security have been set up successfully.";
        return RedirectToPage("/Accounts/Login");
    }
}
