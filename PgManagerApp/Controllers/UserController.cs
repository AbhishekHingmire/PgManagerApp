using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PgManagerApp.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PgManagerApp.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("MasterUserId") != null && HttpContext.Session.GetString("Username") != null)
            {
                var users = new UserRegistration();
                users.MasterId = HttpContext.Session.GetInt32("MasterUserId");
                if (TempData["UserData"] != null)
                {
                    users = JsonConvert.DeserializeObject<UserRegistration>(TempData["UserData"].ToString());
                    int RoomId = _context.Transactions.Where(x => x.UserId == users.Id).Select(x => x.RoomId).FirstOrDefault();
                    string RoomNo = _context.Rooms.Where(x => x.Id == RoomId).Select(x => x.RoomNumber).FirstOrDefault();
                    users.RoomNumber = RoomNo;
                }
                users.Users = _context.Users.Where(x => x.MasterId == HttpContext.Session.GetInt32("MasterUserId")).ToList();

                foreach (var user in users.Users)
                {
                    // Convert nvarchar to decimal and aggregate the totals for each user
                    var totalCharge = _context.Transactions
                        .Where(x => x.UserId == user.Id && x.MasterId == HttpContext.Session.GetInt32("MasterUserId"))
                        .Select(x => Convert.ToDecimal(x.ChargeAmount))
                        .Sum();

                    var totalPaid = _context.Transactions
                        .Where(x => x.UserId == user.Id && x.MasterId == HttpContext.Session.GetInt32("MasterUserId"))
                        .Select(x => Convert.ToDecimal(x.AmountPaid))
                        .Sum();

                    // Calculate the pending amount
                    var pendingAmount = totalCharge - totalPaid;

                    // Set the pending amount in the user object
                    user.PendingAmount = pendingAmount.ToString();
                }



                return View(users);
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteUser(int Id)
        {
            var userDetails = new UserRegistration();
            userDetails = _context.Users.Where(x => x.Id == Id && x.MasterId == HttpContext.Session.GetInt32("MasterUserId")).FirstOrDefault();
            if (userDetails != null)
            {
                _context.Users.Remove(userDetails);
                _context.SaveChanges();
            }
            TempData["Message"] = "User succesfully deleted.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrEdit(UserRegistration newUser)
        {
            if (HttpContext.Session.GetInt32("MasterUserId") != null && HttpContext.Session.GetString("Username") != null)
            {
                var usr = new UserRegistration();

                if (newUser.Id == 0)
                {
                    if (ModelState.IsValid)
                    {
                        _context.Users.Add(newUser);
                        _context.SaveChanges();
                        TempData["Message"] = "User succesfully added.";
                    }
                    else
                    {
                        TempData["Error"] = "Something went wrong!";
                    }
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        _context.Users.Update(newUser);
                        _context.SaveChanges();
                        TempData["Message"] = "User succesfully updated.";
                        return RedirectToAction("Index");
                    }
                    usr = _context.Users.Where(x => x.Id == newUser.Id && x.MasterId == HttpContext.Session.GetInt32("MasterUserId")).FirstOrDefault();
                    usr.ViewOnly = newUser.ViewOnly;
                    TempData["UserData"] = JsonConvert.SerializeObject(usr);
                }
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }
        }
    }
}
