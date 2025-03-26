using Microsoft.AspNetCore.Mvc;

namespace OPS_Practice_Project.Views.Chat
{
    public class ChatController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Chat()
        {
            return View();
        }
    }
}
