using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TurfBookingApp.Data;
using TurfBookingApp.Models;

//[Authorize(Roles = "User")]
public class UserController : Controller
{
    private readonly AppDbContext _context;

    public UserController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Profile()
    {
        var username = User.Identity?.Name;
        var user = _context.Users.FirstOrDefault(u => u.Username == username);
        if (user == null) return RedirectToPage("/Login");
        return View(user);
    }

    [HttpPost]
    public IActionResult Update(User updatedUser)
    {
        var user = _context.Users.Find(updatedUser.Id);
        if (user == null) return NotFound();

        user.PhoneNumber = updatedUser.PhoneNumber;
        _context.SaveChanges();

        return RedirectToAction("Profile");
    }
}
