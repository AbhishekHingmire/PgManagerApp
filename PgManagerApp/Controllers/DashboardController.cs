using Microsoft.AspNetCore.Mvc;
using PgManagerApp.Models;
using System.Diagnostics;

namespace PgManagerApp.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly ApplicationDbContext _context;

        public DashboardController(ILogger<DashboardController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var model = new DashboardViewModel();

            var users = _context.Users.ToList();

            model.TotalUsers = users.Count().ToString();

            var transactions = _context.Transactions.ToList();

            // Count of users who paid
            var paidUserCount = transactions
                .Select(t => t.UserId)
                .Distinct()
                .Count();

            model.CountPaid = paidUserCount.ToString();

            // Count of users who did not pay
            var userIdsWhoPaid = transactions
                .Select(t => t.UserId)
                .Distinct()
                .ToList();

            var notPaidUserCount = users
                .Count(u => !userIdsWhoPaid.Contains(u.Id));

            model.CountUnpaid = notPaidUserCount.ToString();

            // Calculate total paid amount after converting string to decimal
            var totalPaidAmount = transactions
                .Select(t =>
                {
                    // Try to parse the AmountPaid, defaulting to 0 if parsing fails
                    decimal.TryParse(t.AmountPaid, out var amountPaid);
                    return amountPaid;
                })
                .Sum();

            var totalRoomCharge = transactions
                .Select(t =>
                {
                    // Try to parse the AmountPaid, defaulting to 0 if parsing fails
                    decimal.TryParse(t.ChargeAmount, out var amountPaid);
                    return amountPaid;
                })
                .Sum();

            var totalNotPaidAmount = totalRoomCharge - totalPaidAmount;

            model.TotalPendingAmount = totalNotPaidAmount.ToString();

            model.TotalAmount = totalPaidAmount.ToString();

          
            TempData["UserCount"] = users.Count();


            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
