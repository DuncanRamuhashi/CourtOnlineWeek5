using CourtOnlineWeek5.Data;
using CourtOnlineWeek5.Generic;
using CourtOnlineWeek5.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CourtOnlineWeek5.Controllers
{
    public class DefendantsController : Controller
    {
        private CourtCaseDatabaseContext Database =>
            HttpContext.RequestServices.GetRequiredService<CourtCaseDatabaseContext>();

        public async Task<IActionResult> Index()
        {

            return await ErrorHandler.ExecuteAsync(async () =>
            {
                ViewBag.Defendants = await Database.Defendants
    .AsNoTracking()
    .OrderBy(d => d.Name)
    .ToListAsync();
                return View();
            });

        }

        public async Task<IActionResult> Details(string id)
        {
            return await ErrorHandler.ExecuteAsync(async () =>
            {
                ViewBag.Defendant = await Database.Defendants
    .Include(d => d.CourtCaseDefendants)
        .ThenInclude(cd => cd.CourtCase)
            .ThenInclude(cc => cc.OriginalCase)
    .FirstOrDefaultAsync(d => d.Id == id);

                if (ViewBag.Defendant == null)
                    return NotFound();

                return View();
            });


        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Defendant defendant)
        {

            return await ErrorHandler.ExecuteAsync(async () =>
            {
                if (await Database.Defendants.AnyAsync(d => d.Id == defendant.Id))
                {
                    ModelState.AddModelError("Id", "This Defendant ID already exists.");
                }

                if (ModelState.IsValid)
                {
                    Database.Defendants.Add(defendant);
                    await Database.SaveChangesAsync();
                    TempData["Success"] = "Defendant registered successfully.";
                    return RedirectToAction(nameof(Index));
                }
                return View(defendant);
            });

        }

        public async Task<IActionResult> Edit(string id)
        {
            return await ErrorHandler.ExecuteAsync(async () =>
            {
                var defendant = await Database.Defendants.FindAsync(id);
                if (defendant == null)
                    return NotFound();
                return View(defendant);
            });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Defendant defendant)
        {
            return await ErrorHandler.ExecuteAsync(async () =>
            {
                if (id != defendant.Id)
                    return BadRequest();

                if (ModelState.IsValid)
                {
                    var existing = await Database.Defendants.FindAsync(id);
                    if (existing == null)
                        return NotFound();

                    existing.Name = defendant.Name;
                    existing.Idnumber = defendant.Idnumber;
                    existing.DateOfBirth = defendant.DateOfBirth;
                    existing.Gender = defendant.Gender;
                    existing.Address = defendant.Address;
                    existing.Phone = defendant.Phone;
                    existing.Nationality = defendant.Nationality;
                    existing.IsInCustody = defendant.IsInCustody;

                    await Database.SaveChangesAsync();
                    TempData["Success"] = "Defendant updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                return View(defendant);
            });

        }

        public async Task<IActionResult> Delete(string id)
        {
            return await ErrorHandler.ExecuteAsync(async () =>
            {
                ViewBag.Defendant = await Database.Defendants
    .Include(d => d.CourtCaseDefendants)
    .FirstOrDefaultAsync(d => d.Id == id);

                if (ViewBag.Defendant == null)
                    return NotFound();

                return View();
            });

        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {

            return await ErrorHandler.ExecuteAsync(async () =>
            {

                var defendant = await Database.Defendants
    .Include(d => d.CourtCaseDefendants)
    .FirstOrDefaultAsync(d => d.Id == id);

                if (defendant == null)
                    return NotFound();

                if (defendant.CourtCaseDefendants.Any())
                {
                    TempData["Error"] = "Cannot delete a defendant who is assigned to court cases. Remove assignments first.";
                    return RedirectToAction(nameof(Index));
                }

                Database.Defendants.Remove(defendant);
                await Database.SaveChangesAsync();
                TempData["Success"] = "Defendant deleted successfully.";
                return RedirectToAction(nameof(Index));
            });

        }
    }
}
