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
            var users = new UserRegistration();
            if (TempData["UserData"] != null)
            {
                users = JsonConvert.DeserializeObject<UserRegistration>(TempData["UserData"].ToString());
                int RoomId = _context.Transactions.Where(x => x.UserId == users.Id).Select(x => x.RoomId).FirstOrDefault();
                string RoomNo = _context.Rooms.Where(x => x.Id == RoomId).Select(x => x.RoomNumber).FirstOrDefault();
                users.RoomNumber = RoomNo;
            }
            users.Users = _context.Users.ToList();

            foreach(var data in users.Users)
            {
                string paid = _context.Transactions.Where(x => x.UserId == data.Id).Select(x => x.AmountPaid).FirstOrDefault();
                string charge = _context.Transactions.Where(x => x.UserId == data.Id).Select(x => x.ChargeAmount).FirstOrDefault();
                

                int pending = Convert.ToInt32(charge) - Convert.ToInt32(paid);
                data.PendingAmount = Convert.ToString(pending);
            }
            
            
            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteUser(int Id)
        {
            var userDetails = new UserRegistration();
            userDetails = _context.Users.Where(x => x.Id == Id).FirstOrDefault();
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
                if(ModelState.IsValid)
                {
                    _context.Users.Update(newUser);
                    _context.SaveChanges();
                    TempData["Message"] = "User succesfully updated.";
                    return RedirectToAction("Index");
                }
                usr = _context.Users.Where(x => x.Id == newUser.Id).FirstOrDefault();
                usr.ViewOnly = newUser.ViewOnly;
                TempData["UserData"] = JsonConvert.SerializeObject(usr);
            }
            return RedirectToAction("Index");
        }
    }
}
