using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using turfbooking.Models;
using turfbooking.Data; 

public class AddSlotModel : PageModel
{
    private readonly AppDbContext _context;

    public AddSlotModel(AppDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Slot Slot { get; set; }
    public Ground Ground;


    public async Task<IActionResult> OnGetAsync()
    {
        int GroundId = 1;
        
        Ground = await _context.Grounds.FindAsync(GroundId);
        if (Ground == null)
        {
            return NotFound();
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {      
        
        Slot.IsBooked = false;

        _context.Slots.Add(Slot);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Slot added successfully!";
        return RedirectToPage("./AddSlot");
    }

}