using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using turfbooking.Data;
using turfbooking.Models;

namespace turfbooking.Pages.Grounds
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<Ground> Grounds { get; set; } = new List<Ground>();

        //public async Task OnGetAsync()
        //{
        //Grounds = await _context.Grounds
        //    .Where(g => g.IsActive) // Only show active grounds
        //    .ToListAsync();
        public async Task OnGetAsync(bool showInactive = false)
        {
            HttpContext.Session.SetString("PreviousPage", Url.Page("/Admin/AdminDashboard"));
            //Grounds = await _context.Grounds
            //    .Where(g => showInactive || g.IsActive)
            //    .ToListAsync();
            Grounds = await _context.Grounds.ToListAsync();
        }
    }

}
