using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CourtOnlineWeek5.Data;
using CourtOnlineWeek5.Models;

namespace CourtOnlineWeek5.Controllers
{
    public class ProsecutorsController : Controller
    {
        private CourtCaseDatabaseContext Database =>
            HttpContext.RequestServices.GetRequiredService<CourtCaseDatabaseContext>();

        public async Task<IActionResult> Index()
        {
            try {
                ViewBag.Prosecutors = await Database.Prosecutors
    .AsNoTracking()
    .OrderBy(p => p.Name)
    .ToListAsync();
                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }

        }

        public async Task<IActionResult> Details(string id)
        {
            try {
                ViewBag.Prosecutor = await Database.Prosecutors
    .Include(p => p.CourtCases)
    .FirstOrDefaultAsync(p => p.Id == id);

                if (ViewBag.Prosecutor == null)
                    return NotFound();

                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }

        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Prosecutor prosecutor)
        {
            try {
                if (await Database.Prosecutors.AnyAsync(p => p.Id == prosecutor.Id))
                {
                    ModelState.AddModelError("Id", "This Prosecutor ID already exists.");
                }

                if (ModelState.IsValid)
                {
                    Database.Prosecutors.Add(prosecutor);
                    await Database.SaveChangesAsync();
                    TempData["Success"] = "Prosecutor registered successfully.";
                    return RedirectToAction(nameof(Index));
                }
                return View(prosecutor);
            }
            
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }


        public async Task<IActionResult> Edit(string id)
        {
            try {
                var prosecutor = await Database.Prosecutors.FindAsync(id);
                if (prosecutor == null)
                    return NotFound();
                return View(prosecutor);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Prosecutor prosecutor)
        {
            try
            {
                if (id != prosecutor.Id)
                    return BadRequest();

                if (ModelState.IsValid)
                {
                    var existing = await Database.Prosecutors.FindAsync(id);
                    if (existing == null)
                        return NotFound();

                    existing.Name = prosecutor.Name;
                    existing.Rank = prosecutor.Rank;
                    existing.Office = prosecutor.Office;
                    existing.Email = prosecutor.Email;
                    existing.Phone = prosecutor.Phone;
                    existing.IsActive = prosecutor.IsActive;

                    await Database.SaveChangesAsync();
                    TempData["Success"] = "Prosecutor updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                return View(prosecutor);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }


        }

        public async Task<IActionResult> Delete(string id)
        {
            try
            {

                ViewBag.Prosecutor = await Database.Prosecutors
    .Include(p => p.CourtCases)
    .FirstOrDefaultAsync(p => p.Id == id);

                if (ViewBag.Prosecutor == null)
                    return NotFound();

                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }


        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {

            try {
                var prosecutor = await Database.Prosecutors
                  .Include(p => p.CourtCases)
                  .FirstOrDefaultAsync(p => p.Id == id);

                if (prosecutor == null)
                    return NotFound();

                if (prosecutor.CourtCases.Any())
                {
                    TempData["Error"] = "Cannot delete a prosecutor assigned to court cases. Deactivate instead.";
                    return RedirectToAction(nameof(Index));
                }

                Database.Prosecutors.Remove(prosecutor);
                await Database.SaveChangesAsync();
                TempData["Success"] = "Prosecutor deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }

        }
    }
}
