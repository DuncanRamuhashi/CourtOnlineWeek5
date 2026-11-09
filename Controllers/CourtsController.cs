using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CourtOnlineWeek5.Data;
using CourtOnlineWeek5.Models;

namespace CourtOnlineWeek5.Controllers
{
    public class CourtsController : Controller
    {
        private CourtCaseDatabaseContext Database =>
            HttpContext.RequestServices.GetRequiredService<CourtCaseDatabaseContext>();

        public async Task<IActionResult> Index()
        {
            ViewBag.Courts = await Database.Courts
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ToListAsync();
            return View();
        }

        public async Task<IActionResult> Details(int id)
        {
            ViewBag.Court = await Database.Courts
                .Include(c => c.Judges)
                .Include(c => c.CourtCases)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (ViewBag.Court == null)
                return NotFound();

            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Court court)
        {
            if (ModelState.IsValid)
            {
                court.CreatedAt = DateTime.UtcNow;
                Database.Courts.Add(court);
                await Database.SaveChangesAsync();
                TempData["Success"] = "Court created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(court);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var court = await Database.Courts.FindAsync(id);
            if (court == null)
                return NotFound();
            return View(court);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Court court)
        {
            if (id != court.Id)
                return BadRequest();

            if (ModelState.IsValid)
            {
                var existing = await Database.Courts.FindAsync(id);
                if (existing == null)
                    return NotFound();

                existing.Name = court.Name;
                existing.Location = court.Location;
                existing.CourtCode = court.CourtCode;
                existing.CourtLevel = court.CourtLevel;
                existing.Province = court.Province;
                existing.Phone = court.Phone;
                existing.Email = court.Email;
                existing.Address = court.Address;
                existing.IsActive = court.IsActive;

                await Database.SaveChangesAsync();
                TempData["Success"] = "Court updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(court);
        }

        public async Task<IActionResult> Delete(int id)
        {
            ViewBag.Court = await Database.Courts
                .Include(c => c.Judges)
                .Include(c => c.CourtCases)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (ViewBag.Court == null)
                return NotFound();

            return View();
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var court = await Database.Courts
                .Include(c => c.Judges)
                .Include(c => c.CourtCases)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (court == null)
                return NotFound();

            if (court.Judges.Any() || court.CourtCases.Any())
            {
                TempData["Error"] = "Cannot delete a court that has assigned judges or court cases. Deactivate it instead.";
                return RedirectToAction(nameof(Index));
            }

            Database.Courts.Remove(court);
            await Database.SaveChangesAsync();
            TempData["Success"] = "Court deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
