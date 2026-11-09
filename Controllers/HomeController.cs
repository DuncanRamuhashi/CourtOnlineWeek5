using CourtOnlineWeek5.Data;
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
            try
            {
                ViewBag.TotalOriginalCases = await Database.OriginalCases.CountAsync();
                ViewBag.TotalCourtCases = await Database.CourtCases.CountAsync();
                ViewBag.OpenCases = await Database.CourtCases.CountAsync(c => c.Status == "Open" || c.Status == "In Progress" || c.Status == "Pending");
                ViewBag.TotalDefendants = await Database.Defendants.CountAsync();
                ViewBag.TotalCharges = await Database.Charges.CountAsync();
                ViewBag.UpcomingHearings = await Database.CourtCases
                    .Where(c => c.NextHearingDate != null && c.NextHearingDate >= DateTime.UtcNow)
                    .OrderBy(c => c.NextHearingDate)
                    .Take(5)
                    .Include(c => c.OriginalCase)
                    .Include(c => c.Court)
                    .ToListAsync();

                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }

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
