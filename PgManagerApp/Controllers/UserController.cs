using Microsoft.AspNetCore.Mvc;

namespace PgManagerApp.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
