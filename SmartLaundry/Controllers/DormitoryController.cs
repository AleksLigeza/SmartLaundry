using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartLaundry.Authorization;
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
        private readonly IAuthorizationService _authorizationService;
        private readonly IAnnouncementRepository _announcementRepo;

        public DormitoryController(
            IDormitoryRepository dormitoryRepository, IUserRepository userRepository,
            IRoomRepository roomRepository, UserManager<ApplicationUser> userManager,
            IAuthorizationService authorizationService, IAnnouncementRepository announcementRepo)
        {
            _dormitoryRepo = dormitoryRepository;
            _userRepo = userRepository;
            _roomRepo = roomRepository;
            _userManager = userManager;
            _authorizationService = authorizationService;
            _announcementRepo = announcementRepo;
        }

        [HttpGet]
        public IActionResult Index()
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

            var users = !string.IsNullOrEmpty(searchString)
                ? _userRepo.GetUsersWithEmailLike(searchString)
                : _userRepo.Users.ToList();

            var dormitory = _dormitoryRepo.GetSingleById(id);

            var usersWithoutDormitory = users
                .Where(x => x.DormitoryManagerId == null
                            && x.DormitoryPorterId == null
                            && x.RoomId == null
                            && x.Email != "admin@admin.admin")
                .ToList();
            var thisDormitoryUsers = users
                .Where(x => x.DormitoryPorterId == dormitory.DormitoryID
                            || x.DormitoryManagerId == dormitory.DormitoryID
                            && dormitory.Rooms.Any(z => z.Id == x.RoomId))
                .ToList();
            users = usersWithoutDormitory.Concat(thisDormitoryUsers).ToList();

            if (dormitory == null)
            {
                return NotFound();
            }

            int pageSize = 10;
            return View(new ManageDormitoryUsersViewModel(
                new PaginatedList<ApplicationUser>(users, users.Count, page ?? 1, pageSize), dormitory));
        }

        [Authorize(Policy = "MinimumManager")]
        [HttpGet]
        public async Task<IActionResult> ManageRoomUsers(
            int id,
            string currentFilter,
            string searchString,
            int? page)
        {
            var room = _roomRepo.GetRoomWithOccupants(id);
            if (room == null)
            {
                return NotFound();
            }

            if (!await CheckDormitoryMembership(room.Dormitory))
            {
                return BadRequest();
            }

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var users = !String.IsNullOrEmpty(searchString)
                ? _userRepo.GetUsersWithEmailLike(searchString)
                : _userRepo.Users.ToList();

            var dormitory = room.Dormitory;

            var usersWithoutDormitory = users
                .Where(x => x.DormitoryManagerId == null
                            && x.DormitoryPorterId == null
                            && x.RoomId == null
                            && x.Email != "admin@admin.admin")
                .ToList();
            var thisDormitoryUsers = users
                .Where(x => x.DormitoryPorterId == null
                            && x.DormitoryManagerId == null
                            && dormitory.Rooms.Any(z => z.Id == x.RoomId))
                .ToList();
            users = usersWithoutDormitory.Concat(thisDormitoryUsers).ToList();

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

            return RedirectToAction("Details", new {id = dormitoryId});
        }

        [HttpPost]
        [Authorize(Policy = "MinimumManager")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignPorter(int dormitoryId, string porterEmail)
        {
            var dormitory = _dormitoryRepo.GetSingleById(dormitoryId);
            if (!await CheckDormitoryMembership(dormitory))
            {
                return BadRequest();
            }

            var user = _userRepo.GetUserByEmail(porterEmail);
            user.DormitoryPorterId = dormitoryId;

            _userRepo.AssignDormitoryAsPorter(user, dormitory);
            if (await _userManager.IsInRoleAsync(user, "Porter") == false)
            {
                await _userManager.AddToRoleAsync(user, "Porter");
            }

            return RedirectToAction("Details", new {id = dormitoryId});
        }

        [HttpPost]
        [Authorize(Policy = "MinimumManager")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemovePorter(int dormitoryId, string porterEmail)
        {
            var dormitory = _dormitoryRepo.GetSingleById(dormitoryId);
            if (!await CheckDormitoryMembership(dormitory))
            {
                return BadRequest();
            }

            var user = _userRepo.GetUserByEmail(porterEmail);
            user.DormitoryPorterId = dormitoryId;

            _userRepo.RemoveDormitoryPorter(user, dormitory);
            await _userManager.RemoveFromRoleAsync(user, "Porter");

            return RedirectToAction("Details", new {id = dormitoryId});
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

            var model = new DormitoryDetailsViewModel
            {
                Dormitory = dormitory,
                Manager = dormitory.Manager,
                Porters = dormitory.Porters.ToList(),
                Laundries = dormitory.Laundries.ToList(),
                Announcements = _announcementRepo.GetDormitoryAnnouncements(dormitory.DormitoryID)
            };

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
        public IActionResult Create([Bind("DormitoryID,Name,Address,ZipCode,City")]
            Dormitory dormitory)
        {
            if (!ModelState.IsValid)
                return View(dormitory);
            _dormitoryRepo.AddSingle(dormitory);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Policy = "MinimumManager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var dormitory = _dormitoryRepo.GetSingleById(id.Value);
            if (!await CheckDormitoryMembership(dormitory))
            {
                return BadRequest();
            }

            return View(dormitory);
        }

        [HttpPost]
        [Authorize(Policy = "MinimumManager")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DormitoryID,Name,Address,ZipCode,City")]
            Dormitory dormitory)
        {
            if (!await CheckDormitoryMembership(id))
            {
                return BadRequest();
            }

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

            return View(dormitory);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dormitory = _dormitoryRepo.GetSingleWithIncludes(id);

            await _userManager.RemoveFromRoleAsync(dormitory.Manager, "Manager");

            foreach (var porter in dormitory.Porters)
            {
                await _userManager.RemoveFromRoleAsync(porter, "Porter");
            }

            foreach (var room in dormitory.Rooms)
            {
                foreach (var user in room.Occupants)
                {
                    await _userManager.RemoveFromRoleAsync(user, "Occupant");
                }
            }

            _dormitoryRepo.DeleteSingle(dormitory);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Policy = "MinimumPorter")]
        public async Task<IActionResult> Rooms(int id)
        {
            var dormitory = _dormitoryRepo.GetDormitoryWithRooms(id);
            if (!await CheckDormitoryMembership(dormitory))
            {
                return BadRequest();
            }

            return View(dormitory);
        }

        [ValidateAntiForgeryToken]
        [Authorize(Policy = "MinimumManager")]
        [HttpPost]
        public async Task<IActionResult> AddRoom(int roomNumber, int dormitoryId)
        {
            if (!await CheckDormitoryMembership(dormitoryId))
            {
                return BadRequest();
            }

            if (!_dormitoryRepo.DormitoryHasRoom(dormitoryId, roomNumber))
            {
                return BadRequest();
            }

            _roomRepo.AddRoomToDormitory(roomNumber, dormitoryId);
            return RedirectToAction(nameof(Rooms), new {id = dormitoryId});
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize(Policy = "MinimumManager")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var room = _roomRepo.GetRoomWithOccupants(id);

            if (room == null)
            {
                return NotFound();
            }

            if (!await CheckDormitoryMembership(room.Dormitory))
            {
                return BadRequest();
            }

            foreach (var user in room.Occupants)
            {
                await _userManager.RemoveFromRoleAsync(user, "Occupant");
            }

            var dormitoryId = _roomRepo.DeleteRoom(id);

            if (dormitoryId == null)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Rooms), new {id = dormitoryId});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "MinimumManager")]
        public async Task<IActionResult> AssignOccupant(string userId, int roomId)
        {
            var room = _roomRepo.GetRoomById(roomId);

            if (!await CheckDormitoryMembership(room.Dormitory))
            {
                return BadRequest();
            }

            var user = _userRepo.GetUserById(userId);

            _roomRepo.AssignOccupant(room, user);
            if (await _userManager.IsInRoleAsync(user, "Occupant") == false)
            {
                await _userManager.AddToRoleAsync(user, "Occupant");
            }

            return RedirectToAction("Rooms", new {id = room.DormitoryId});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "MinimumManager")]
        public async Task<IActionResult> RemoveOccupant(string userId, int roomId)
        {
            var room = _roomRepo.GetRoomById(roomId);

            if (!await CheckDormitoryMembership(room.Dormitory))
            {
                return BadRequest();
            }

            var user = _userRepo.GetUserById(userId);

            _roomRepo.RemoveOccupant(room, user);
            await _userManager.RemoveFromRoleAsync(user, "Occupant");

            return RedirectToAction("Rooms", new {id = room.DormitoryId});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "MinimumManager")]
        public async Task<IActionResult> AddAnnouncement(string message, int dormitoryId)
        {
            if (!await CheckDormitoryMembership(dormitoryId))
            {
                return BadRequest();
            }

            var announcement = new Announcement()
            {
                Message = message,
                DormitoryId = dormitoryId,
                PublishingDate = DateTime.Now
            };
            _announcementRepo.CreateAnnouncement(announcement);

            return RedirectToAction(nameof(Details), new {id = dormitoryId});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "MinimumManager")]
        public async Task<IActionResult> RemoveAnnouncement(int announcementId)
        {
            var announcement = _announcementRepo.GetAnnouncementById(announcementId);
            if (!await CheckDormitoryMembership(announcement.DormitoryId))
            {
                return BadRequest();
            }

            var dormitoryId = announcement.DormitoryId;

            _announcementRepo.DeleteAnnouncement(announcementId);

            return RedirectToAction(nameof(Details), new {id = dormitoryId});
        }

        private bool DormitoryExists(int id)
        {
            return _dormitoryRepo.Dormitories.Any(e => e.DormitoryID == id);
        }

        private async Task<bool> CheckDormitoryMembership(Dormitory dormitory)
        {
            var authorizationResult =
                await _authorizationService.AuthorizeAsync(User, dormitory, AuthPolicies.DormitoryMembership);
            return authorizationResult.Succeeded;
        }

        private async Task<bool> CheckDormitoryMembership(int dormitoryId)
        {
            var dormitory = _dormitoryRepo.GetSingleById(dormitoryId);
            return await CheckDormitoryMembership(dormitory);
        }
    }
}