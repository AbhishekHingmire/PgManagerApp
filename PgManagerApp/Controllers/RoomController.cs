using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using PgManagerApp.Models;
using PgManagerApp.Models.Room;
using PgManagerApp.Models.Transaction;

namespace PgManagerApp.Controllers
{
    public class RoomController : Controller
    {
        private readonly ApplicationDbContext _context;
        public RoomController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var roomData = new RoomViewModel();

            if (TempData["RoomData"] != null)
            {
                roomData = JsonConvert.DeserializeObject<RoomViewModel>(TempData["RoomData"].ToString()) ?? new RoomViewModel();
            }
            roomData.Rooms = _context.Rooms.ToList();


            return View(roomData);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrEdit(RoomViewModel model)
        {
            var room = new RoomViewModel();

            if (model.Id == 0)
            {
                if (ModelState.IsValid)
                {
                    _context.Rooms.Add(model);
                    _context.SaveChanges();
                    TempData["Message"] = $"Room {model.RoomNumber} added succesfully";
                }
            }
            else
            {
                //Update if valid
                if (ModelState.IsValid)
                {
                    _context.Rooms.Update(model);
                    _context.SaveChanges();
                    TempData["Message"] = $"Room {model.RoomNumber} updated succesfuly.";
                }
                //Populate popup for update if its IsEditable true
                else
                {
                    room = _context.Rooms.Where(x => x.Id == model.Id).FirstOrDefault();
                    room.IsEditable = model.IsEditable;
                    TempData["RoomData"] = JsonConvert.SerializeObject(room);
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteRoom(int Id)
        {
            var room = new RoomViewModel();
            room = _context.Rooms.Where(x => x.Id == Id).FirstOrDefault();
            if (room != null)
            {
                _context.Rooms.Remove(room);
                _context.SaveChanges();
            }
            TempData["Message"] = $"Room {room.RoomNumber} succesfully deleted.";
            return RedirectToAction("Index");
        }
    }
}
