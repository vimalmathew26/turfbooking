using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.RazorPages;
using turfbooking.Data;
using turfbooking.Models;

namespace turfbooking.Pages.Admin
{
    public class GroundBookingManagementModel : PageModel
    {     
            private readonly AppDbContext _context;

            public GroundBookingManagementModel(AppDbContext context)

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



