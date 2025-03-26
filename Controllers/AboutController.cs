using Microsoft.AspNetCore.Mvc;

namespace OPS_Practice_Project.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult OurStory()
        {
            return View();
        }

        public IActionResult Blog()
        {
            return View();
        }
        public IActionResult NewsUpdates()
        {
            return View();
        }
    }
}
