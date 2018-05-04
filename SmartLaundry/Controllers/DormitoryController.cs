using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartLaundry.Data;
using SmartLaundry.Data.Interfaces;
using SmartLaundry.Models;
using SmartLaundry.Models.DormitoryViewModels;

namespace SmartLaundry.Controllers
{
    [Authorize]
    public class DormitoryController : Controller
    {
        private readonly IDormitoryRepository _dormitoryRepo;
        private readonly IUserRepository _userRepo;
        private readonly IRoomRepository _roomRepo;
        private readonly UserManager<ApplicationUser> _userManager;

        public DormitoryController(
            IDormitoryRepository dormitoryRepository,
            IUserRepository userRepository,
            IRoomRepository roomRepository,
            UserManager<ApplicationUser> userManager)
        {
            _dormitoryRepo = dormitoryRepository;
            _userRepo = userRepository;
            _roomRepo = roomRepository;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(_dormitoryRepo.GetAll());
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public IActionResult ManageDormitoryUsers(
                int id,
                string currentFilter,
                string searchString,
                int? page)
        {

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            List<ApplicationUser> users;
            if (!String.IsNullOrEmpty(searchString))
            {
                users = _userRepo.GetUsersWithEmailLike(searchString);
            }
            else
            {
                users = _userRepo.Users.ToList();
            }

            var dormitory = _dormitoryRepo.GetSingleById(id);

            if (dormitory == null)
            {
                return NotFound();
            }

            int pageSize = 10;
            return View(new ManageDormitoryUsersViewModel(new PaginatedList<ApplicationUser>(users, users.Count, page ?? 1, pageSize), dormitory));
        }

        [Authorize(Policy = "MinimumManager")]
        [HttpGet]
        public IActionResult ManageRoomUsers(
                int id,
                string currentFilter,
                string searchString,
                int? page)
        {

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            List<ApplicationUser> users;
            if (!String.IsNullOrEmpty(searchString))
            {
                users = _userRepo.GetUsersWithEmailLike(searchString);
            }
            else
            {
                users = _userRepo.Users.ToList();
            }

            var room = _roomRepo.GetRoomWithOccupants(id);
            if (room == null)
            {
                return NotFound();
            }

            int pageSize = 10;
            var model = new ManageRoomUsersViewModel()
            {
                Room = room,
                Users = new PaginatedList<ApplicationUser>(users, users.Count, page ?? 1, pageSize)
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignManager(int dormitoryId, string managerEmail)
        {
            var dormitory = _dormitoryRepo.GetSingleWithIncludes(dormitoryId);

            if (dormitory.Manager != null)
            {
                await _userManager.RemoveFromRoleAsync(dormitory.Manager, "Manager");
            }

            var user = _userRepo.GetUserByEmail(managerEmail);
            user.DormitoryPorterId = null;
            if (await _userManager.IsInRoleAsync(user, "Porter"))
            {
                await _userManager.RemoveFromRoleAsync(user, "Porter");
            }

            _dormitoryRepo.AssignManager(user, dormitory);
            if (await _userManager.IsInRoleAsync(user, "Manager") == false)
            {
                await _userManager.AddToRoleAsync(user, "Manager");
            }
            return RedirectToAction("Details", new { id = dormitoryId });
        }

        [HttpPost]
        [Authorize(Policy = "MinimumManager")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignPorter(int dormitoryId, string porterEmail)
        {
            var dormitory = _dormitoryRepo.GetSingleById(dormitoryId);
            var user = _userRepo.GetUserByEmail(porterEmail);
            user.DormitoryPorterId = dormitoryId;

            _userRepo.AssignDormitoryAsPorter(user, dormitory);
            if (await _userManager.IsInRoleAsync(user, "Porter") == false)
            {
                await _userManager.AddToRoleAsync(user, "Porter");
            }

            return RedirectToAction("Details", new { id = dormitoryId });
        }

        [HttpPost]
        [Authorize(Policy = "MinimumManager")]
        [ValidateAntiForgeryToken]
        public IActionResult RemovePorter(int dormitoryId, string porterEmail)
        {
            var dormitory = _dormitoryRepo.GetSingleById(dormitoryId);
            var user = _userRepo.GetUserByEmail(porterEmail);
            user.DormitoryPorterId = dormitoryId;

            _userRepo.RemoveDormitoryPorter(user, dormitory);
            _userManager.RemoveFromRoleAsync(user, "Porter");

            return RedirectToAction("Details", new { id = dormitoryId });
        }

        [HttpGet]
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dormitory = _dormitoryRepo.GetSingleWithIncludes(id.Value);
            if (dormitory == null)
            {
                return NotFound();
            }

            var model = new DormitoryDetailsViewModel();
            model.Dormitory = dormitory;
            model.Manager = dormitory.Manager;
            model.Porters = dormitory.Porters.ToList();
            model.Laundries = dormitory.Laundries.ToList();

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("DormitoryID,Name,Address,ZipCode,City")] Dormitory dormitory)
        {
            if (ModelState.IsValid)
            {
                _dormitoryRepo.AddSingle(dormitory);
                return RedirectToAction(nameof(Index));
            }
            return View(dormitory);
        }

        [HttpGet]
        [Authorize(Policy = "MinimumManager")]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dormitory = _dormitoryRepo.GetSingleById(id.Value);
            if (dormitory == null)
            {
                return NotFound();
            }
            return View(dormitory);
        }

        [HttpPost]
        [Authorize(Policy = "MinimumManager")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("DormitoryID,Name,Address,ZipCode,City")] Dormitory dormitory)
        {
            if (id != dormitory.DormitoryID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _dormitoryRepo.UpdateSingle(dormitory);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DormitoryExists(dormitory.DormitoryID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(dormitory);
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dormitory = _dormitoryRepo.GetSingleById(id.Value);
            if (dormitory == null)
            {
                return NotFound();
            }

            return View(dormitory);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var dormitory = _dormitoryRepo.GetSingleWithIncludes(id);

            _userManager.RemoveFromRoleAsync(dormitory.Manager, "Manager");

            foreach (var porter in dormitory.Porters)
            {
                _userManager.RemoveFromRoleAsync(porter, "Porter");
            }

            foreach (var room in dormitory.Rooms)
            {
                foreach (var user in room.Occupants)
                {
                    _userManager.RemoveFromRoleAsync(user, "Occupant");
                }
            }

            _dormitoryRepo.DeleteSingle(dormitory);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Policy = "MinimumPorter")]
        public IActionResult Rooms(int id)
        {
            var dormitory = _dormitoryRepo.GetDormitoryWithRooms(id);
            return View(dormitory);
        }

        [ValidateAntiForgeryToken]
        [Authorize(Policy = "MinimumManager")]
        [HttpPost]
        public IActionResult AddRoom(int roomNumber, int dormitoryId)
        {
            if (_dormitoryRepo.DormitoryHasRoom(dormitoryId, roomNumber))
            {
                return BadRequest();
            }

            _roomRepo.AddRoomToDormitory(roomNumber, dormitoryId);
            return RedirectToAction(nameof(Rooms), new { id = dormitoryId });
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize(Policy = "MinimumManager")]
        public IActionResult DeleteRoom(int id)
        {
            var room = _roomRepo.GetRoomWithOccupants(id);

            if (room == null)
            {
                return NotFound();
            }

            foreach (var user in room.Occupants)
            {
                _userManager.RemoveFromRoleAsync(user, "Occupant");
            }

            var dormitoryId = _roomRepo.DeleteRoom(id);

            if (dormitoryId == null)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Rooms), new { id = dormitoryId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "MinimumManager")]
        public async Task<IActionResult> AssignOccupant(string userId, int roomId)
        {
            var room = _roomRepo.GetRoomById(roomId);
            var user = _userRepo.GetUserById(userId);

            _roomRepo.AssignOccupant(room, user);
            if (await _userManager.IsInRoleAsync(user, "Occupant") == false)
            {
                await _userManager.AddToRoleAsync(user, "Occupant");
            }

            return RedirectToAction("Rooms", new { id = room.DormitoryId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "MinimumManager")]
        public IActionResult RemoveOccupant(string userId, int roomId)
        {
            var room = _roomRepo.GetRoomById(roomId);
            var user = _userRepo.GetUserById(userId);

            _roomRepo.RemoveOccupant(room, user);
            _userManager.RemoveFromRoleAsync(user, "Occupant");

            return RedirectToAction("Rooms", new { id = room.DormitoryId });
        }

        private bool DormitoryExists(int id)
        {
            return _dormitoryRepo.Dormitories.Any(e => e.DormitoryID == id);
        }

    }
}
