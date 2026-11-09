
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CourtOnlineWeek5.Data;
using CourtOnlineWeek5.Models;

namespace CourtOnlineWeek5.Controllers
{
    public class ChargesController : Controller
    {
        private CourtCaseDatabaseContext Database =>
            HttpContext.RequestServices
                .GetRequiredService<CourtCaseDatabaseContext>();

        // GET: Charges
        public async Task<IActionResult> Index(int? courtCaseId)
        {

            try {
                var query = Database.Charges
        .AsNoTracking()
        .Include(c => c.CourtCase)
            .ThenInclude(cc => cc.OriginalCase)
        .AsQueryable();

                if (courtCaseId.HasValue)
                {
                    query = query.Where(c =>
                        c.CourtCaseId == courtCaseId.Value);

                    ViewBag.CourtCaseId = courtCaseId.Value;
                }

                var charges = await query
                    .OrderBy(c => c.Name)
                    .ToListAsync();

                return View(charges);
            }
            catch(Exception ex)
            {
       
                Console.WriteLine($"Error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }

        }

        // GET: Charges/Details/5
        public async Task<IActionResult> Details(int id)
        {

            try {
                var charge = await Database.Charges
        .AsNoTracking()
        .Include(c => c.CourtCase)
            .ThenInclude(cc => cc.OriginalCase)
        .FirstOrDefaultAsync(c => c.Id == id);

                if (charge == null)
                    return NotFound();

                return View(charge);
            }
            catch (Exception ex)
            {
             
                Console.WriteLine($"Error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }

        }

        // GET: Charges/Create?courtCaseId=5
        public async Task<IActionResult> Create(int courtCaseId)
        {

            try {
                if (courtCaseId <= 0)
                    return BadRequest("Invalid court case ID.");

                var courtCase = await Database.CourtCases
                    .Include(c => c.OriginalCase)
                    .FirstOrDefaultAsync(c => c.Id == courtCaseId);

                if (courtCase == null)
                    return NotFound("Court case not found.");

                ViewBag.CourtCase = courtCase;

                return View(new Charge
                {
                    CourtCaseId = courtCase.Id
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }

        }

        // POST: Charges/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Charge charge)
        {

            try
            {
                ModelState.Remove(nameof(charge.CourtCase));

                if (string.IsNullOrWhiteSpace(charge.Name))
                {
                    ModelState.AddModelError(
                        nameof(charge.Name),
                        "Charge name is required.");
                }

                if (charge.Name?.Length > 200)
                    ModelState.AddModelError(
                        nameof(charge.Name),
                        "Charge name cannot exceed 200 characters.");

                var courtCase = await Database.CourtCases
                    .Include(c => c.OriginalCase)
                    .FirstOrDefaultAsync(c => c.Id == charge.CourtCaseId);

                if (courtCase == null)
                    return NotFound("Court case not found.");

                if (ModelState.IsValid)
                {
                    charge.CourtCase = courtCase;

                    Database.Charges.Add(charge);
                    await Database.SaveChangesAsync();

                    TempData["Success"] = "Charge added successfully.";

                    return RedirectToAction(
                        "Manage",
                        "CourtCases",
                        new { originalCaseId = courtCase.OriginalCaseId });
                }

                ViewBag.CourtCase = courtCase;

                return View(charge);
            }
            catch (Exception ex) {
                Console.WriteLine($"Error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
            
        }

        // GET: Charges/Edit/5
        public async Task<IActionResult> Edit(int id)
        {

            try
            {
                var charge = await Database.Charges
              .AsNoTracking()
              .Include(c => c.CourtCase)
              .FirstOrDefaultAsync(c => c.Id == id);

                if (charge == null)
                    return NotFound();

                return View(charge);
            }
            catch (Exception ex) {
                Console.WriteLine($"Error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
          
        }

        // POST: Charges/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Charge charge)
        {


            try
            {
                if (id != charge.Id)
                    return BadRequest();

                ModelState.Remove(nameof(charge.CourtCase));

                if (string.IsNullOrWhiteSpace(charge.Name))
                {
                    ModelState.AddModelError(
                        nameof(charge.Name),
                        "Charge name is required.");
                }

                if (charge.Name?.Length > 200)
                    ModelState.AddModelError(
                        nameof(charge.Name),
                        "Charge name cannot exceed 200 characters.");

                var existing = await Database.Charges
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (existing == null)
                    return NotFound();

                var courtCase = await Database.CourtCases
                    .FirstOrDefaultAsync(c => c.Id == existing.CourtCaseId);

                if (courtCase == null)
                    return NotFound("Associated court case not found.");

                if (ModelState.IsValid)
                {
                    existing.Name = charge.Name;
                    existing.ChargeCode = charge.ChargeCode;
                    existing.Statute = charge.Statute;
                    existing.Description = charge.Description;
                    existing.Plea = charge.Plea;
                    existing.Severity = charge.Severity;

                    await Database.SaveChangesAsync();

                    TempData["Success"] = "Charge updated successfully.";

                    return RedirectToAction(
                        "Manage",
                        "CourtCases",
                        new { originalCaseId = courtCase.OriginalCaseId });
                }

                charge.CourtCaseId = existing.CourtCaseId;
                charge.CourtCase = courtCase;

                return View(charge);
            }
            catch(Exception ex) {
                Console.WriteLine($"Error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");

            }
           
        }

        // GET: Charges/Delete/5
        public async Task<IActionResult> Delete(int id)
        {

            try {
                var charge = await Database.Charges
                   .AsNoTracking()
                   .Include(c => c.CourtCase)
                       .ThenInclude(cc => cc.OriginalCase)
                   .FirstOrDefaultAsync(c => c.Id == id);

                if (charge == null)
                    return NotFound();

                return View(charge);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");


            }
           
        }

        // POST: Charges/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            try
            {
                var charge = await Database.Charges
    .FirstOrDefaultAsync(c => c.Id == id);

                if (charge == null)
                    return NotFound();

                var courtCase = await Database.CourtCases
                    .FirstOrDefaultAsync(c => c.Id == charge.CourtCaseId);

                if (courtCase == null)
                    return NotFound("Associated court case not found.");

                var originalCaseId = courtCase.OriginalCaseId;

                Database.Charges.Remove(charge);
                await Database.SaveChangesAsync();

                TempData["Success"] = "Charge deleted successfully.";

                return RedirectToAction(
                    "Manage",
                    "CourtCases",
                    new { originalCaseId });
            }
            catch (Exception ex) {

                Console.WriteLine($"Error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");

            }


        }
    }
}
