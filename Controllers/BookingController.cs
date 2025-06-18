using Microsoft.AspNetCore.Mvc;

namespace TurfBookingApp.Controllers
{
    public class BookingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
