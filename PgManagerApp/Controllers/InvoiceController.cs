using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using PgManagerApp.Models;
using PgManagerApp.Models.Room;

namespace PgManagerApp.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly ApplicationDbContext _context;
        public InvoiceController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var users = _context.Users
            .Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = u.Name
            })
            .ToList();

            ViewBag.Users = users;
            return View();
        }
        
        [HttpPost]
        public IActionResult GenerateInvoice(string Id)
        {
           
            var user = _context.Users
            .Where(u => u.Id == Convert.ToInt32(Id))
            .Select(u => new
            {
                u.Name,
                u.MobileNumber,
                u.Email
            })
            .FirstOrDefault();

            if (user == null)
            {
                return null; // Handle user not found
            }

            var transactions = _context.Transactions
                .Where(t => t.UserId == Convert.ToInt32(Id))
                .Select(t => new
                {
                    t.RoomId,
                    PaidAmount = t.AmountPaid.ToString(),  // Format as string
                    ChargeAmount = t.ChargeAmount.ToString(),  // Format as string
                    StartDate = t.StartDate.ToString("yyyy-MM-dd"),  // Format as string
                    EndDate = t.EndDate.ToString("yyyy-MM-dd"),  // Format as string
                    RoomNumber = _context.Rooms
                        .Where(r => r.Id == t.RoomId)
                        .Select(r => r.RoomNumber)
                        .FirstOrDefault()
                })
                .ToList();

            var totalChargeAmount = transactions.Sum(t => decimal.Parse(t.ChargeAmount));
            var totalPaidAmount = transactions.Sum(t => decimal.Parse(t.PaidAmount));

            var invoiceData = new InvoiceViewModel
            {
                UserName = user.Name,
                MobileNumber = user.MobileNumber,
                Email = user.Email,
                RoomDetails = transactions.Select(t => new RoomDetail
                {
                    RoomNumber = t.RoomNumber,
                    ChargeAmount = t.ChargeAmount,
                    PaidAmount = t.PaidAmount,
                    StartDate = t.StartDate,
                    EndDate = t.EndDate
                }).ToList(),
                TotalChargeAmount = totalChargeAmount.ToString(),
                TotalPaidAmount = totalPaidAmount.ToString()
            };

            if(invoiceData!=null)
            {
                TempData["InvoiceData"] = JsonConvert.SerializeObject(invoiceData);
            }
            else
            {
                TempData["Error"] = "No invoice data found!";
            }
            return RedirectToAction("Index");
        }
    }
}
