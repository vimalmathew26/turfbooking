using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

namespace turfbooking.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminDashboardModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}