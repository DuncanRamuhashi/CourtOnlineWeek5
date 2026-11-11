using CourtOnlineWeek5.Data;
using CourtOnlineWeek5.Generic;
using CourtOnlineWeek5.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace CourtOnlineWeek5.Controllers
{
    public class HomeController : Controller
    {
        private CourtCaseDatabaseContext Database =>
       HttpContext.RequestServices.GetRequiredService<CourtCaseDatabaseContext>();

        public async Task<IActionResult> Index()
        {
            return await ErrorHandler.ExecuteAsync(async () =>
            {
                ViewBag.TotalOriginalCases = await Database.OriginalCases.CountAsync();
                ViewBag.TotalCourtCases = await Database.CourtCases.CountAsync();
                ViewBag.OpenCases = await Database.CourtCases.CountAsync(c => c.Status == "Open" || c.Status == "In Progress" || c.Status == "Pending");
                ViewBag.TotalDefendants = await Database.Defendants.CountAsync();
                ViewBag.TotalCharges = await Database.Charges.CountAsync();
                ViewBag.TotalJudges = await Database.Judges.CountAsync();
                ViewBag.TotalProcecutors = await Database.Prosecutors.CountAsync();
                ViewBag.UpcomingHearings = await Database.CourtCases
                    .Where(c => c.NextHearingDate != null && c.NextHearingDate >= DateTime.UtcNow)
                    .OrderBy(c => c.NextHearingDate)
                    .Take(5)
                    .Include(c => c.OriginalCase)
                    .Include(c => c.Court)
                    .ToListAsync();

                return View();
            });

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
