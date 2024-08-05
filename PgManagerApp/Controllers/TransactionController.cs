using Microsoft.AspNetCore.Mvc;

namespace PgManagerApp.Controllers
{
    public class TransactionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
