using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CourtOnlineWeek5.Data;
using CourtOnlineWeek5.Models;

namespace CourtOnlineWeek5.Controllers
{
    public class CourtCasesController : Controller
    {
        private CourtCaseDatabaseContext Database =>
            HttpContext.RequestServices.GetRequiredService<CourtCaseDatabaseContext>();

        // GET: CourtCases (list all)
        public async Task<IActionResult> Index()
        {
            try {
                ViewBag.CourtCases = await Database.CourtCases
    .Include(c => c.OriginalCase)
    .Include(c => c.Court)
    .Include(c => c.Judge)
    .Include(c => c.Prosecutor)
    .AsNoTracking()
    .OrderByDescending(c => c.CreatedAt)
    .ToListAsync();

                return View();
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");

            }

        }

        // GET: CourtCases/Manage?originalCaseId=5
        public async Task<IActionResult> Manage(int originalCaseId)
        {

            try {
                ViewBag.OriginalCase = await Database.OriginalCases
               .AsNoTracking()
               .FirstOrDefaultAsync(c => c.Id == originalCaseId);

                if (ViewBag.OriginalCase == null)
                    return NotFound();

                ViewBag.CourtCase = await Database.CourtCases
                    .Include(c => c.Court)
                    .Include(c => c.Judge)
                    .Include(c => c.Prosecutor)
                    .Include(c => c.Charges)
                    .Include(c => c.CourtCaseDefendants)
                        .ThenInclude(cd => cd.Defendant)
                    .FirstOrDefaultAsync(c => c.OriginalCaseId == originalCaseId);

                ViewBag.Courts = await Database.Courts.Where(c => c.IsActive).ToListAsync();
                ViewBag.Judges = await Database.Judges.Where(j => j.IsActive).ToListAsync();
                ViewBag.Prosecutors = await Database.Prosecutors.Where(p => p.IsActive).ToListAsync();

                if (ViewBag.CourtCase != null)
                {
                    ViewBag.Charges = ViewBag.CourtCase.Charges;
                    ViewBag.Defendants = ViewBag.CourtCase.CourtCaseDefendants;
                }
                else
                {
                    ViewBag.Charges = new List<Charge>();
                    ViewBag.Defendants = new List<CourtCaseDefendant>();
                }

                return View();
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");

            }
           
        }

        // GET: CourtCases/Create?originalCaseId=5
        public async Task<IActionResult> Create(int originalCaseId)
        {
            try
            {
                var originalCase = await Database.OriginalCases.FindAsync(originalCaseId);
                if (originalCase == null)
                    return NotFound();

                if (await Database.CourtCases.AnyAsync(c => c.OriginalCaseId == originalCaseId))
                {
                    return RedirectToAction(nameof(Manage), new { originalCaseId });
                }

                ViewBag.OriginalCase = originalCase;
                ViewBag.Courts = await Database.Courts.Where(c => c.IsActive).ToListAsync();
                ViewBag.Judges = await Database.Judges.Where(j => j.IsActive).ToListAsync();
                ViewBag.Prosecutors = await Database.Prosecutors.Where(p => p.IsActive).ToListAsync();

                return View();
            }
            catch(Exception ex) {

                Console.WriteLine($"Error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");

            }
          
        }

        // POST: CourtCases/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourtCase courtCase)
        {

            try
            {
                var originalCase = await Database.OriginalCases.FindAsync(courtCase.OriginalCaseId);
                if (originalCase == null)
                    return NotFound();

                if (await Database.CourtCases.AnyAsync(c => c.OriginalCaseId == courtCase.OriginalCaseId))
                {
                    ModelState.AddModelError("", "A court case already exists for this original case.");
                }

                if (ModelState.IsValid)
                {
                    courtCase.CreatedAt = DateTime.UtcNow;
                    Database.CourtCases.Add(courtCase);
                    await Database.SaveChangesAsync();
                    TempData["Success"] = "Court case created successfully.";
                    return RedirectToAction(nameof(Manage), new { originalCaseId = courtCase.OriginalCaseId });
                }

                ViewBag.OriginalCase = originalCase;
                ViewBag.Courts = await Database.Courts.Where(c => c.IsActive).ToListAsync();
                ViewBag.Judges = await Database.Judges.Where(j => j.IsActive).ToListAsync();
                ViewBag.Prosecutors = await Database.Prosecutors.Where(p => p.IsActive).ToListAsync();
                return View(courtCase);
            }
            catch(Exception ex) {
                Console.WriteLine($"Error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");

            }

        }

        // GET: CourtCases/Edit/5
        public async Task<IActionResult> Edit(int id)
        {

            try
            {
                var courtCase = await Database.CourtCases.FindAsync(id);
                if (courtCase == null)
                    return NotFound();

                ViewBag.Courts = await Database.Courts.Where(c => c.IsActive).ToListAsync();
                ViewBag.Judges = await Database.Judges.Where(j => j.IsActive).ToListAsync();
                ViewBag.Prosecutors = await Database.Prosecutors.Where(p => p.IsActive).ToListAsync();
                ViewBag.OriginalCaseId = courtCase.OriginalCaseId;

                return View(courtCase);
            }
            catch (Exception ex){
                Console.WriteLine($"Error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");

            }

        }

        // POST: CourtCases/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
      
        public async Task<IActionResult> Edit(int id, CourtCase courtCase)
        {

            try
            {
                if (id != courtCase.Id)
                    return BadRequest();

                // Remove navigation properties from validation.
                ModelState.Remove(nameof(CourtCase.Court));
                ModelState.Remove(nameof(CourtCase.Judge));
                ModelState.Remove(nameof(CourtCase.Prosecutor));
                ModelState.Remove(nameof(CourtCase.OriginalCase));

                if (!ModelState.IsValid)
                {
                    await LoadEditDropdowns();

                    ViewBag.OriginalCaseId = courtCase.OriginalCaseId;

                    return View(courtCase);
                }

                var existing = await Database.CourtCases.FindAsync(id);

                if (existing == null)
                    return NotFound();

                // Check that the selected court exists.
                var courtExists = await Database.Courts
                    .AnyAsync(c => c.Id == courtCase.CourtId && c.IsActive);

                if (!courtExists)
                {
                    ModelState.AddModelError(
                        nameof(courtCase.CourtId),
                        "Please select a valid active court.");

                    await LoadEditDropdowns();
                    ViewBag.OriginalCaseId = courtCase.OriginalCaseId;

                    return View(courtCase);
                }

                existing.Status = courtCase.Status;
                existing.CourtId = courtCase.CourtId;
                existing.JudgeId = courtCase.JudgeId;
                existing.ProsecutorId = courtCase.ProsecutorId;
                existing.CaseNumber = courtCase.CaseNumber;
                existing.FilingDate = courtCase.FilingDate;
                existing.NextHearingDate = courtCase.NextHearingDate;
                existing.ClosedDate = courtCase.ClosedDate;
                existing.Outcome = courtCase.Outcome;
                existing.Notes = courtCase.Notes;
                existing.IsConfidential = courtCase.IsConfidential;
                existing.UpdatedAt = DateTime.UtcNow;

                await Database.SaveChangesAsync();

                TempData["Success"] = "Court case updated successfully.";

                return RedirectToAction(
                    nameof(Manage),
                    new { originalCaseId = existing.OriginalCaseId });
            }
            catch (Exception ex) {
                Console.WriteLine($"Error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");

            }
          
        }
        // GET: CourtCases/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                ViewBag.CourtCase = await Database.CourtCases
    .Include(c => c.OriginalCase)
    .Include(c => c.Court)
    .Include(c => c.Charges)
    .Include(c => c.CourtCaseDefendants)
    .FirstOrDefaultAsync(c => c.Id == id);

                if (ViewBag.CourtCase == null)
                    return NotFound();

                return View();
            }
            catch (Exception ex) {
                Console.WriteLine($"Error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");

            }

        }

        // POST: CourtCases/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            try
            {
                var courtCase = await Database.CourtCases.FindAsync(id);
                if (courtCase == null)
                    return NotFound();

                var originalCaseId = courtCase.OriginalCaseId;
                Database.CourtCases.Remove(courtCase);
                await Database.SaveChangesAsync();
                TempData["Success"] = "Court case deleted successfully.";
                return RedirectToAction(nameof(Manage), new { originalCaseId });
            }
            catch (Exception ex) {
                Console.WriteLine($"Error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");

            }

        }

        // GET: CourtCases/AddDefendant?courtCaseId=5
        public async Task<IActionResult> AddDefendant(int courtCaseId)
        {

            try
            {
                var courtCase = await Database.CourtCases
    .Include(c => c.CourtCaseDefendants)
    .FirstOrDefaultAsync(c => c.Id == courtCaseId);

                if (courtCase == null)
                    return NotFound();

                var existingIds = courtCase.CourtCaseDefendants.Select(cd => cd.DefendantId).ToList();

                ViewBag.AvailableDefendants = await Database.Defendants
                    .Where(d => !existingIds.Contains(d.Id))
                    .OrderBy(d => d.Name)
                    .ToListAsync();

                ViewBag.CourtCaseId = courtCaseId;
                ViewBag.OriginalCaseId = courtCase.OriginalCaseId;

                return View();
            }
            catch (Exception ex) {
                Console.WriteLine($"Error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // POST: CourtCases/AddDefendant
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDefendant(int courtCaseId, string defendantId, string role, bool isPrimary)
        {

            try
            {
                var courtCase = await Database.CourtCases.FindAsync(courtCaseId);
                if (courtCase == null)
                    return NotFound();

                var defendant = await Database.Defendants.FindAsync(defendantId);
                if (defendant == null)
                    return NotFound();

                if (await Database.CourtCaseDefendants.AnyAsync(cd => cd.CourtCaseId == courtCaseId && cd.DefendantId == defendantId))
                {
                    TempData["Error"] = "This defendant is already assigned to this case.";
                    return RedirectToAction(nameof(AddDefendant), new { courtCaseId });
                }

                var relationship = new CourtCaseDefendant
                {
                    CourtCaseId = courtCaseId,
                    DefendantId = defendantId,
                    Role = string.IsNullOrWhiteSpace(role) ? "Accused" : role,
                    IsPrimary = isPrimary
                };

                Database.CourtCaseDefendants.Add(relationship);
                await Database.SaveChangesAsync();
                TempData["Success"] = "Defendant added to court case successfully.";
                return RedirectToAction(nameof(Manage), new { originalCaseId = courtCase.OriginalCaseId });
            }
            catch (Exception ex) {
                Console.WriteLine($"Error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");

            }

        }

        // POST: CourtCases/RemoveDefendant
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveDefendant(int courtCaseId, string defendantId)
        {

            try
            {
                var relationship = await Database.CourtCaseDefendants
    .FirstOrDefaultAsync(cd => cd.CourtCaseId == courtCaseId && cd.DefendantId == defendantId);

                if (relationship == null)
                    return NotFound();

                var courtCase = await Database.CourtCases.FindAsync(courtCaseId);
                Database.CourtCaseDefendants.Remove(relationship);
                await Database.SaveChangesAsync();

                TempData["Success"] = "Defendant removed from court case (defendant record preserved).";
                return RedirectToAction(nameof(Manage), new { originalCaseId = courtCase!.OriginalCaseId });
            }
            catch (Exception ex) {
                Console.WriteLine($"Error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }

        }
        private async Task LoadEditDropdowns()
        {
            ViewBag.Courts = await Database.Courts
                .Where(c => c.IsActive)
                .ToListAsync();

            ViewBag.Judges = await Database.Judges
                .Where(j => j.IsActive)
                .ToListAsync();

            ViewBag.Prosecutors = await Database.Prosecutors
                .Where(p => p.IsActive)
                .ToListAsync();
        }
    }
}
