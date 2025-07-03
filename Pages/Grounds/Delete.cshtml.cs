using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.RegularExpressions;
using turfbooking.Data;
using turfbooking.Models;
using Microsoft.EntityFrameworkCore;

namespace turfbooking.Pages.Grounds
{
    [Authorize(Roles = "Admin")]
    public class DeleteModel : PageModel
    {
        private readonly AppDbContext _context;

        public DeleteModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Ground Ground { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            HttpContext.Session.SetString("PreviousPage", Url.Page("/Grounds/Index"));

            var ground = await _context.Grounds.FindAsync(id);
            if (ground == null)
            {
                return NotFound();
            }
            Ground = ground;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var groundToDelete = await _context.Grounds.FindAsync(Ground.Id);
            if (groundToDelete == null)
            {
                TempData["ErrorMessage"] = "Ground Not Exists";
                return RedirectToPage("/Grounds/Index");
            }
            var HasBookings =await _context.Bookings
                            .Where(s=>s.GroundId == groundToDelete.Id)
                            .Where(s => s.BookingDate >= DateTime.Today)
                            .Where(s => s.Status == BookingStatus.Confirmed)                           
                            .ToListAsync();

         
                var courts = await _context.Courts
                    .Where(s => s.GroundId == groundToDelete.Id)
                    .ToListAsync();
                _context.Courts.RemoveRange(courts);

                var slots = await _context.Slots
                    .Where(s => s.GroundId == groundToDelete.Id)
                    .ToListAsync();
                _context.Slots.RemoveRange(slots);

                var bookings = await _context.Bookings
                    .Where(b => b.GroundId == groundToDelete.Id)
                    .ToListAsync();
                _context.Bookings.RemoveRange(bookings);


                var reviews = await _context.Reviews
                    .Where(b => b.GroundId == groundToDelete.Id)
                    .ToListAsync();
                _context.Reviews.RemoveRange(reviews);
                await _context.SaveChangesAsync();

                _context.Grounds.Remove(groundToDelete);
                await _context.SaveChangesAsync();
                return RedirectToPage("/Grounds/Index");
        }
    }
}

