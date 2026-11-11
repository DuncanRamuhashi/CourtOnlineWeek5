
using CourtOnlineWeek5.Data;
using CourtOnlineWeek5.Generic;
using CourtOnlineWeek5.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace CourtOnlineWeek5.Controllers
{
    public class OriginalCasesController : Controller
    {
        private CourtCaseDatabaseContext Database =>
            HttpContext.RequestServices
                .GetRequiredService<CourtCaseDatabaseContext>();

        // GET: OriginalCases
        public async Task<IActionResult> Index()
        {
            return await ErrorHandler.ExecuteAsync(async () =>
            {
                ViewBag.OriginalCases = await Database.OriginalCases
    .OrderByDescending(c => c.Date)
    .ToListAsync();

                return View();
            });

        }

        // GET: OriginalCases/Details/5
        public async Task<IActionResult> Details(int id)
        {
            return await ErrorHandler.ExecuteAsync(async () =>
            {

                ViewBag.OriginalCase = await Database.OriginalCases
    .Include(c => c.CourtCases)
        .ThenInclude(cc => cc.Court)
    .Include(c => c.CourtCases)
        .ThenInclude(cc => cc.Judge)
    .FirstOrDefaultAsync(c => c.Id == id);

                if (ViewBag.OriginalCase == null)
                    return NotFound();

                return View();
            });

        }

        // GET: OriginalCases/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: OriginalCases/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OriginalCase originalCase)
        {
            return await ErrorHandler.ExecuteAsync(async () =>
            {
                if (!ModelState.IsValid)
                    return View(originalCase);

                if (await Database.OriginalCases.AnyAsync(
                    c => c.CaseNumber == originalCase.CaseNumber))
                {
                    ModelState.AddModelError(
                        "CaseNumber",
                        "This case number already exists.");

                    return View(originalCase);
                }

                Database.OriginalCases.Add(originalCase);
                await Database.SaveChangesAsync();

                TempData["Success"] = "Original case registered successfully.";

                return RedirectToAction(nameof(Index));
            });

        }

        // GET: OriginalCases/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            return await ErrorHandler.ExecuteAsync(async () =>
            {
                var originalCase = await Database.OriginalCases.FindAsync(id);

                if (originalCase == null)
                    return NotFound();

                return View(originalCase);
            });

        }

        // POST: OriginalCases/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OriginalCase originalCase)
        {

            return await ErrorHandler.ExecuteAsync(async () =>
            {

                if (id != originalCase.Id)
                    return BadRequest();

                if (!ModelState.IsValid)
                    return View(originalCase);

                var existing = await Database.OriginalCases.FindAsync(id);

                if (existing == null)
                    return NotFound();

                if (await Database.OriginalCases.AnyAsync(
                    c => c.CaseNumber == originalCase.CaseNumber && c.Id != id))
                {
                    ModelState.AddModelError(
                        "CaseNumber",
                        "This case number already exists.");

                    return View(originalCase);
                }

                existing.CaseNumber = originalCase.CaseNumber;
                existing.Name = originalCase.Name;
                existing.Date = originalCase.Date;
                existing.Description = originalCase.Description;
                existing.CaseType = originalCase.CaseType;
                existing.Priority = originalCase.Priority;
                existing.ReportedBy = originalCase.ReportedBy;

                await Database.SaveChangesAsync();

                TempData["Success"] = "Original case updated successfully.";

                return RedirectToAction(nameof(Index));
            });

        }

        // GET: OriginalCases/Delete/5
        public async Task<IActionResult> Delete(int id)
        {

            return await ErrorHandler.ExecuteAsync(async () =>
            {
                ViewBag.OriginalCase = await Database.OriginalCases
    .Include(c => c.CourtCases)
    .FirstOrDefaultAsync(c => c.Id == id);

                if (ViewBag.OriginalCase == null)
                    return NotFound();

                return View();
            });

        }

        // POST: OriginalCases/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            return await ErrorHandler.ExecuteAsync(async () =>
            {
                var originalCase = await Database.OriginalCases
    .Include(c => c.CourtCases)
    .FirstOrDefaultAsync(c => c.Id == id);

                if (originalCase == null)
                    return NotFound();

                if (originalCase.CourtCases.Any())
                {
                    TempData["Error"] =
                        "Cannot delete an original case that has associated court cases.";

                    return RedirectToAction(nameof(Index));
                }

                Database.OriginalCases.Remove(originalCase);

                await Database.SaveChangesAsync();

                TempData["Success"] = "Original case deleted successfully.";

                return RedirectToAction(nameof(Index));
            });

        }
    }
}
