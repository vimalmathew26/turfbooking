using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using turfbooking.Data;
using turfbooking.Models;

namespace turfbooking.Pages.Admin

{
    [Authorize(Roles = "Admin")]
    public class GroundSlotModel : PageModel

    {
        private readonly AppDbContext _context;

        public GroundSlotModel(AppDbContext context)

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

            //Grounds = await _context.Grounds

            //    .Where(g => showInactive || g.IsActive)

            //    .ToListAsync();

            Grounds = await _context.Grounds.ToListAsync();
        }
        }

 }

