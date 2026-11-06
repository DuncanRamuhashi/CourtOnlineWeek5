using Microsoft.AspNetCore.Mvc;

namespace CourtOnlineWeek5.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
