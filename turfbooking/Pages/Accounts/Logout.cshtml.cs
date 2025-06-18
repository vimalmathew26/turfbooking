using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;


namespace turfbooking.Pages.Accounts
{
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnGet()
        {
            await HttpContext.SignOutAsync("UserAuth");
            Response.Cookies.Delete("UserAuth");
            TempData["Message"] = "You have been logged out successfully.";
            return RedirectToPage("/Accounts/Login");
        }
    }
}