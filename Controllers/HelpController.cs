using Microsoft.AspNetCore.Mvc;

namespace OPS_Practice_Project.Controllers
{
    public class HelpController : Controller
    {
        public IActionResult FAQPage()
        {
            return View();
        }

        public IActionResult CustomerSupport()
        {
            return View();
        }
    }
}
