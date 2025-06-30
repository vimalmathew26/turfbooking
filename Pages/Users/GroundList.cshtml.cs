using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using turfbooking.Data;
using turfbooking.Models;

namespace turfbooking.Pages.Users
{
    [Authorize(Roles = "User")]
    public class GroundListModel : PageModel
    {
        private readonly AppDbContext _context;

        public GroundListModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public IList<Ground> Grounds { get; set; } = new List<Ground>();

        [BindProperty(SupportsGet = true)]
        public string? Location { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SportType { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? MinPrice { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? MaxPrice { get; set; }

        public async Task OnGetAsync()
        {
            HttpContext.Session.SetString("PreviousPage", Url.Page("/Users/UserDashboard"));
            var query = _context.Grounds.Where(g => g.IsActive).AsQueryable();

            if (!string.IsNullOrEmpty(Location))
                query = query.Where(g => g.Location.Contains(Location));

            if (!string.IsNullOrEmpty(SportType))
                query = query.Where(g => g.SupportedSports.Contains(SportType));

            if (MinPrice.HasValue)
                query = query.Where(g => g.PricePerHour >= MinPrice.Value);

            if (MaxPrice.HasValue)
                query = query.Where(g => g.PricePerHour <= MaxPrice.Value);

            Grounds = await query.ToListAsync();
        }
    }
}
