using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CourtOnlineWeek5.Data;
using CourtOnlineWeek5.Models;

namespace CourtOnlineWeek5.Controllers
{
    public class JudgesController : Controller
    {
        private CourtCaseDatabaseContext Database =>
            HttpContext.RequestServices.GetRequiredService<CourtCaseDatabaseContext>();

        public async Task<IActionResult> Index()
        {
            ViewBag.Judges = await Database.Judges
                .Include(j => j.Court)
                .AsNoTracking()
                .OrderBy(j => j.Name)
                .ToListAsync();
            return View();
        }

        public async Task<IActionResult> Details(string id)
        {
            ViewBag.Judge = await Database.Judges
                .Include(j => j.Court)
                .Include(j => j.CourtCases)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (ViewBag.Judge == null)
                return NotFound();

            return View();
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Courts = await Database.Courts.Where(c => c.IsActive).ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Judge judge)
        {
            if (await Database.Judges.AnyAsync(j => j.Id == judge.Id))
            {
                ModelState.AddModelError("Id", "This Judge ID already exists.");
            }

            if (ModelState.IsValid)
            {
                Database.Judges.Add(judge);
                await Database.SaveChangesAsync();
                TempData["Success"] = "Judge registered successfully.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Courts = await Database.Courts.Where(c => c.IsActive).ToListAsync();
            return View(judge);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var judge = await Database.Judges.FindAsync(id);
            if (judge == null)
                return NotFound();

            ViewBag.Courts = await Database.Courts.Where(c => c.IsActive).ToListAsync();
            return View(judge);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Judge judge)
        {
            if (id != judge.Id)
                return BadRequest();

            if (ModelState.IsValid)
            {
                var existing = await Database.Judges.FindAsync(id);
                if (existing == null)
                    return NotFound();

                existing.Name = judge.Name;
                existing.Title = judge.Title;
                existing.CourtId = judge.CourtId;
                existing.Email = judge.Email;
                existing.Phone = judge.Phone;
                existing.AppointmentDate = judge.AppointmentDate;
                existing.IsActive = judge.IsActive;

                await Database.SaveChangesAsync();
                TempData["Success"] = "Judge updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Courts = await Database.Courts.Where(c => c.IsActive).ToListAsync();
            return View(judge);
        }

        public async Task<IActionResult> Delete(string id)
        {
            ViewBag.Judge = await Database.Judges
                .Include(j => j.Court)
                .Include(j => j.CourtCases)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (ViewBag.Judge == null)
                return NotFound();

            return View();
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var judge = await Database.Judges
                .Include(j => j.CourtCases)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (judge == null)
                return NotFound();

            if (judge.CourtCases.Any())
            {
                TempData["Error"] = "Cannot delete a judge assigned to court cases. Deactivate instead.";
                return RedirectToAction(nameof(Index));
            }

            Database.Judges.Remove(judge);
            await Database.SaveChangesAsync();
            TempData["Success"] = "Judge deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
