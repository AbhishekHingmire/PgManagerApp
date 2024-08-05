using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using PgManagerApp.Models;
using PgManagerApp.Models.Transaction;

namespace PgManagerApp.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ApplicationDbContext _context;
        public TransactionController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var model = new TransactionViewModel();

            if (TempData["TransData"] != null)
            {
                model = JsonConvert.DeserializeObject<TransactionViewModel>(TempData["TransData"].ToString()) ?? new TransactionViewModel();
            }

            //For user names
            model.Users = _context.Users.Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = u.Name
            }).ToList();

            //For transactions
            model.Transactions = _context.Transactions.Select(t => new TransactionViewModel
            {
                Id = t.Id,
                UserName = _context.Users.FirstOrDefault(u => u.Id == t.UserId).Name, // Map user name
                RoomNumber = _context.Rooms.FirstOrDefault(u => u.Id == t.RoomId).RoomNumber,
                ChargeAmount = t.ChargeAmount,
                AmountPaid = t.AmountPaid,
                StartDate = t.StartDate,
                EndDate = t.EndDate
            }).ToList();

            //Same for rooms
            model.Rooms = _context.Rooms.Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = u.RoomNumber
            }).ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrEdit(TransactionViewModel model) 
        {
            var tran = new TransactionViewModel();

            if (model.Id == 0)
            {
                if (ModelState.IsValid)
                {
                    _context.Transactions.Add(model);
                    _context.SaveChanges();
                    TempData["Message"] = "Transaction added.";
                }
                else
                {
                    TempData["Error"] = "Something went wrong!";
                }
            }
            else
            {
                //Update if valid
                if (ModelState.IsValid)
                {
                        _context.Transactions.Update(model);
                        _context.SaveChanges();
                        TempData["Message"] = "Transaction updated succesfuly.";
                }
               
                //Populate popup for update if its IsEditable true
                else
                {
                    tran = _context.Transactions.Where(x => x.Id == model.Id).FirstOrDefault();
                    tran.IsEditable = model.IsEditable;
                    TempData["TransData"] = JsonConvert.SerializeObject(tran);
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteTransaction(int Id)
        {
            var transaction = new TransactionViewModel();
            transaction = _context.Transactions.Where(x => x.Id == Id).FirstOrDefault();
            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
                _context.SaveChanges();
            }
            TempData["Message"] = "Transaction succesfully deleted.";
            return RedirectToAction("Index");
        }
    }
}
