using Microsoft.AspNetCore.Mvc;

namespace SportsClub.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View(); //error.cshtml file is located under Shared folder which will be accessed if file doesn't exist under home folder of the views.
        }
    }
}
