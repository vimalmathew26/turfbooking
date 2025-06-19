using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using turfbooking.Data;
using turfbooking.Models;

namespace turfbooking.Controllers
{
    //[Authorize]
    public class GroundsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public GroundsController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Grounds
        public async Task<IActionResult> Index()
        {
            var grounds = await _context.Grounds.ToListAsync();
            return View(grounds);
        }

        // GET: Grounds/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Grounds/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Ground ground, IFormFile PhotoFile)
        {
            if (ModelState.IsValid)
            {
                if (PhotoFile != null && PhotoFile.Length > 0)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(PhotoFile.FileName);
                    string extension = Path.GetExtension(PhotoFile.FileName);
                    string fullPath = fileName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;

                    string path = Path.Combine(wwwRootPath + "/uploads/", fullPath);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await PhotoFile.CopyToAsync(stream);
                    }

                    ground.PhotoPath = "/uploads/" + fullPath;
                }

                _context.Add(ground);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(ground);
        }

        // GET: Grounds/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var ground = await _context.Grounds.FindAsync(id);
            if (ground == null) return NotFound();

            return View(ground);
        }

        // POST: Grounds/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Ground ground, IFormFile PhotoFile)
        {
            if (id != ground.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (PhotoFile != null && PhotoFile.Length > 0)
                    {
                        string wwwRootPath = _webHostEnvironment.WebRootPath;
                        string fileName = Path.GetFileNameWithoutExtension(PhotoFile.FileName);
                        string extension = Path.GetExtension(PhotoFile.FileName);
                        string fullPath = fileName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;

                        string path = Path.Combine(wwwRootPath + "/uploads/", fullPath);

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await PhotoFile.CopyToAsync(stream);
                        }

                        ground.PhotoPath = "/uploads/" + fullPath;
                    }

                    _context.Update(ground);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GroundExists(ground.Id)) return NotFound();
                    else throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(ground);
        }

        // GET: Grounds/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var ground = await _context.Grounds
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ground == null) return NotFound();

            return View(ground);
        }

        // POST: Grounds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ground = await _context.Grounds
                .Include(g => g.Slots)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (ground == null) return NotFound();

            // Restrict deletion if active bookings (slots) exist
            //if (ground.Slots.Any(s => s.IsBooked))
            //{
            //    ModelState.AddModelError("", "Cannot delete ground with active bookings.");
            //    return View(ground);
            //}

            _context.Grounds.Remove(ground);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GroundExists(int id)
        {
            return _context.Grounds.Any(e => e.Id == id);
        }
    }
}