using Microsoft.AspNetCore.Mvc;
using PgManagerApp.Models;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;

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
            if (HttpContext.Session.GetInt32("MasterUserId") != null && HttpContext.Session.GetInt32("Username") != null)
            {
                var model = new DashboardViewModel();
               

                var users = _context.Users.Where(x => x.MasterId == HttpContext.Session.GetInt32("MasterUserId")).ToList();
                

                model.TotalUsers = users.Count().ToString();

                var transactions = _context.Transactions.Where(x => x.MasterId == HttpContext.Session.GetInt32("MasterUserId")).ToList();

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


                //CHartData
                var daysOfWeek = new List<DayOfWeek>
    {
        DayOfWeek.Sunday,
        DayOfWeek.Monday,
        DayOfWeek.Tuesday,
        DayOfWeek.Wednesday,
        DayOfWeek.Thursday,
        DayOfWeek.Friday,
        DayOfWeek.Saturday
    };

                var weeklyData = transactions
                    .GroupBy(t => t.StartDate.DayOfWeek)
                    .Select(g => new
                    {
                        Day = g.Key,
                        Count = g.Count()
                    })
                    .ToDictionary(x => x.Day, x => x.Count); // Convert to dictionary for easy lookup

                var dataPoints = daysOfWeek.Select(day => new
                {
                    label = day.ToString(),
                    y = weeklyData.ContainsKey(day) ? weeklyData[day] : 0
                }).ToList();

                ViewBag.DataPoints = JsonSerializer.Serialize(dataPoints);
                ViewBag.UserCounts = weeklyData;

                return View(model);
            }
            else
            {
                return RedirectToAction("Login","Auth");
            }
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
