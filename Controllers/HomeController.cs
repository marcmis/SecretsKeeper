using Microsoft.AspNetCore.Mvc;

namespace SecretsKeeper.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
