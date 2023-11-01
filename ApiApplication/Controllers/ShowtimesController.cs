using Microsoft.AspNetCore.Mvc;

namespace ApiApplication.Controllers
{
    public class ShowtimesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
