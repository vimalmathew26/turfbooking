using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using turfbooking.Data;
using turfbooking.Models;

namespace turfbooking.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class GroundBookingManagementModel : PageModel
    {     
         private readonly AppDbContext _context;
         public GroundBookingManagementModel(AppDbContext context)
         {
            _context = context;
         }
         public IList<Ground> Grounds { get; set; } = new List<Ground>();
         public async Task OnGetAsync()

         {
            HttpContext.Session.SetString("PreviousPage", Url.Page("/Admin/AdminDashboard"));
            Grounds = await _context.Grounds.ToListAsync();
         }
    }

}



