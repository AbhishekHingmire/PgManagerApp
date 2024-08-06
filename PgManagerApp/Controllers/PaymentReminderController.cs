using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PgManagerApp.Models;

namespace PgManagerApp.Controllers
{
    public class PaymentReminderController : Controller
    {
        private readonly ApplicationDbContext _context;
        public PaymentReminderController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var users = new UserRegistration();
            users.Users = _context.Users.ToList();

            foreach (var user in users.Users)
            {
                // Convert nvarchar to decimal and aggregate the totals for each user
                var totalCharge = _context.Transactions
                    .Where(x => x.UserId == user.Id)
                    .Select(x => Convert.ToInt32(x.ChargeAmount))
                .Sum();

                var totalPaid = _context.Transactions
                    .Where(x => x.UserId == user.Id)
                    .Select(x => Convert.ToInt32(x.AmountPaid))
                    .Sum();

                // Calculate the pending amount
                var pendingAmount = totalCharge - totalPaid;

                // Set the pending amount in the user object
                user.PendingAmount = pendingAmount.ToString();
            }
            return View(users);
        }
    }
}
