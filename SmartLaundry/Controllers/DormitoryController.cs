using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartLaundry.Authorization;
using SmartLaundry.Controllers.Helpers;
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
            _announcementRepo = announcementRepo;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(_dormitoryRepo.GetAll());
        }

        [Authorize(Policy = "MinimumManager")]
        [HttpGet]
        public async Task<IActionResult> ManageDormitoryUsers(
            int id,
            string currentFilter,
            string searchString,
            int? page)
        {
            if(!await AuthHelpers.CheckDormitoryMembership(User, id))
            {
                return ControllerHelpers.ShowAccessDeniedErrorPage(this);
            }

            if(searchString != null)
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
                .Where(x => x.DormitoryPorterId == dormitory.DormitoryId
                            || x.DormitoryManagerId == dormitory.DormitoryId
                            && dormitory.Rooms.Any(z => z.Id == x.RoomId))
                .ToList();
            users = usersWithoutDormitory.Concat(thisDormitoryUsers).ToList();

            if(dormitory == null)
            {
                return ControllerHelpers.Show404ErrorPage(this);
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
            if(room == null)
            {
                return ControllerHelpers.Show404ErrorPage(this);
            }

            if(!await AuthHelpers.CheckDormitoryMembership(User, room.Dormitory))
            {
                return ControllerHelpers.ShowAccessDeniedErrorPage(this);
            }

            if(searchString != null)
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
                            && !x.NormalizedEmail.Equals(RolesData.AdminEmail))
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

            if(dormitory.Manager != null)
            {
                await _userManager.RemoveFromRoleAsync(dormitory.Manager, "Manager");
            }

            var user = _userRepo.GetUserByEmail(managerEmail);
            user.DormitoryPorterId = null;
            if(await _userManager.IsInRoleAsync(user, "Porter"))
            {
                await _userManager.RemoveFromRoleAsync(user, "Porter");
            }

            _dormitoryRepo.AssignManager(user, dormitory);
            if(await _userManager.IsInRoleAsync(user, "Manager") == false)
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
            if(dormitory == null)
            {
                return ControllerHelpers.Show404ErrorPage(this);
            }
            if(!await AuthHelpers.CheckDormitoryMembership(User, dormitory))
            {
                return ControllerHelpers.ShowAccessDeniedErrorPage(this);
            }

            var user = _userRepo.GetUserByEmail(porterEmail);
            user.DormitoryPorterId = dormitoryId;

            _userRepo.AssignDormitoryAsPorter(user, dormitory);
            if(await _userManager.IsInRoleAsync(user, "Porter") == false)
            {
                await _userManager.AddToRoleAsync(user, "Porter");
            }

            return RedirectToAction("Details", new { id = dormitoryId });
        }

        [HttpPost]
        [Authorize(Policy = "MinimumManager")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemovePorter(int dormitoryId, string porterEmail)
        {
            var dormitory = _dormitoryRepo.GetSingleById(dormitoryId);
            if(dormitory == null)
            {
                return ControllerHelpers.Show404ErrorPage(this);
            }
            if(!await AuthHelpers.CheckDormitoryMembership(User, dormitory))
            {
                return ControllerHelpers.ShowAccessDeniedErrorPage(this);
            }

            var user = _userRepo.GetUserByEmail(porterEmail);
            user.DormitoryPorterId = dormitoryId;

            _userRepo.RemoveDormitoryPorter(user, dormitory);
            await _userManager.RemoveFromRoleAsync(user, "Porter");

            return RedirectToAction("Details", new { id = dormitoryId });
        }

        [HttpGet]
        public IActionResult Details(int? id)
        {
            if(id == null)
            {
                return ControllerHelpers.Show404ErrorPage(this);
            }

            var dormitory = _dormitoryRepo.GetSingleWithIncludes(id.Value);
            if(dormitory == null)
            {
                return ControllerHelpers.Show404ErrorPage(this);
            }

            var model = new DormitoryDetailsViewModel
            {
                Dormitory = dormitory,
                Manager = dormitory.Manager,
                Porters = dormitory.Porters.ToList(),
                Laundries = dormitory.Laundries.ToList(),
                Announcements = _announcementRepo.GetDormitoryAnnouncements(dormitory.DormitoryId)
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
            if(!ModelState.IsValid)
                return View(dormitory);
            _dormitoryRepo.AddSingle(dormitory);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Policy = "MinimumManager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return ControllerHelpers.Show404ErrorPage(this);
            }

            var dormitory = _dormitoryRepo.GetSingleById(id.Value);
            if(dormitory == null)
            {
                return ControllerHelpers.Show404ErrorPage(this);
            }
            if(!await AuthHelpers.CheckDormitoryMembership(User, dormitory))
            {
                return ControllerHelpers.ShowAccessDeniedErrorPage(this);
            }

            return View(dormitory);
        }

        [HttpPost]
        [Authorize(Policy = "MinimumManager")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DormitoryID,Name,Address,ZipCode,City")]
            Dormitory dormitory)
        {
            if(!await AuthHelpers.CheckDormitoryMembership(User, id))
            {
                return ControllerHelpers.ShowAccessDeniedErrorPage(this);
            }

            if(id != dormitory.DormitoryId)
            {
                return ControllerHelpers.Show404ErrorPage(this);
            }

            if(ModelState.IsValid)
            {
                try
                {
                    _dormitoryRepo.UpdateSingle(dormitory);
                }
                catch(DbUpdateConcurrencyException)
                {
                    if(!DormitoryExists(dormitory.DormitoryId))
                    {
                        return ControllerHelpers.Show404ErrorPage(this);
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
            if(id == null)
            {
                return ControllerHelpers.Show404ErrorPage(this);
            }

            var dormitory = _dormitoryRepo.GetSingleById(id.Value);

            return dormitory == null ? ControllerHelpers.Show404ErrorPage(this) : View(dormitory);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dormitory = _dormitoryRepo.GetSingleWithIncludes(id);

            await _userManager.RemoveFromRoleAsync(dormitory.Manager, "Manager");

            foreach(var porter in dormitory.Porters)
            {
                await _userManager.RemoveFromRoleAsync(porter, "Porter");
            }

            foreach(var room in dormitory.Rooms)
            {
                foreach(var user in room.Occupants)
                {
                    await _userManager.RemoveFromRoleAsync(user, "Occupant");
                }
            }

            _dormitoryRepo.DeleteSingle(dormitory);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("[controller]/[action]/{id}")]
        [Authorize(Policy = "MinimumPorter")]
        public async Task<IActionResult> Rooms(int id)
        {
            var dormitory = _dormitoryRepo.GetDormitoryWithRooms(id);
            if(!await AuthHelpers.CheckDormitoryMembership(User, dormitory))
            {
                return ControllerHelpers.ShowAccessDeniedErrorPage(this);
            }

            var model = CreateRoomsViewModel(id, dormitory);

            return View(model);
        }

        [HttpGet]
        [Authorize(Policy = "MinimumPorter")]
        public IActionResult Rooms(RoomsViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return ControllerHelpers.Show404ErrorPage(this);
            }

            return View(model);
        }

        [ValidateAntiForgeryToken]
        [Authorize(Policy = "MinimumManager")]
        [HttpPost]
        public async Task<IActionResult> AddRoom(RoomsViewModel parentModel)
        {
            var roomNumber = parentModel.RoomToAdd.Number;
            var dormitoryId = parentModel.RoomToAdd.DormitoryId;

            if(!await AuthHelpers.CheckDormitoryMembership(User, dormitoryId))
            {
                return ControllerHelpers.ShowAccessDeniedErrorPage(this);
            }

            if(_dormitoryRepo.DormitoryHasRoom(roomNumber, dormitoryId))
            {
                var dormitory = _dormitoryRepo.GetDormitoryWithRooms(dormitoryId);
                var model = CreateRoomsViewModel(dormitoryId, dormitory);
                model.ErrorMessage = "There is a room with the same number.";
                model.RoomToAdd.Number = roomNumber;
                return View(nameof(Rooms), model);
            }

            _roomRepo.AddRoomToDormitory(roomNumber, dormitoryId);
            return RedirectToAction(nameof(Rooms), new { id = dormitoryId });
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize(Policy = "MinimumManager")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var room = _roomRepo.GetRoomWithOccupants(id);

            if(room == null)
            {
                return ControllerHelpers.Show404ErrorPage(this);
            }

            if(!await AuthHelpers.CheckDormitoryMembership(User, room.Dormitory))
            {
                return ControllerHelpers.ShowAccessDeniedErrorPage(this);
            }

            foreach(var user in room.Occupants)
            {
                await _userManager.RemoveFromRoleAsync(user, "Occupant");
            }

            var dormitoryId = _roomRepo.DeleteRoom(id);

            if(dormitoryId == null)
            {
                return ControllerHelpers.Show404ErrorPage(this);
            }

            return RedirectToAction(nameof(Rooms), new { id = dormitoryId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "MinimumManager")]
        public async Task<IActionResult> AssignOccupant(string userId, int roomId)
        {
            var room = _roomRepo.GetRoomById(roomId);

            if(!await AuthHelpers.CheckDormitoryMembership(User, room.Dormitory))
            {
                return ControllerHelpers.ShowAccessDeniedErrorPage(this);
            }

            var user = _userRepo.GetUserById(userId);

            _roomRepo.AssignOccupant(room, user);
            if(await _userManager.IsInRoleAsync(user, "Occupant") == false)
            {
                await _userManager.AddToRoleAsync(user, "Occupant");
            }

            return RedirectToAction("Rooms", new { id = room.DormitoryId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "MinimumManager")]
        public async Task<IActionResult> RemoveOccupant(string userId, int roomId)
        {
            var room = _roomRepo.GetRoomById(roomId);

            if(!await AuthHelpers.CheckDormitoryMembership(User, room.Dormitory))
            {
                return ControllerHelpers.ShowAccessDeniedErrorPage(this);
            }

            var user = _userRepo.GetUserById(userId);

            _roomRepo.RemoveOccupant(room, user);
            await _userManager.RemoveFromRoleAsync(user, "Occupant");

            return RedirectToAction("Rooms", new { id = room.DormitoryId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "MinimumManager")]
        public async Task<IActionResult> AddAnnouncement(string message, int dormitoryId)
        {
            if(!await AuthHelpers.CheckDormitoryMembership(User, dormitoryId))
            {
                return ControllerHelpers.ShowAccessDeniedErrorPage(this);
            }

            var announcement = new Announcement()
            {
                Message = message,
                DormitoryId = dormitoryId,
                PublishingDate = DateTime.Now
            };
            _announcementRepo.CreateAnnouncement(announcement);

            return RedirectToAction(nameof(Details), new { id = dormitoryId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "MinimumManager")]
        public async Task<IActionResult> RemoveAnnouncement(int announcementId)
        {
            var announcement = _announcementRepo.GetAnnouncementById(announcementId);
            if(!await AuthHelpers.CheckDormitoryMembership(User, announcement.DormitoryId))
            {
                return ControllerHelpers.ShowAccessDeniedErrorPage(this);
            }

            var dormitoryId = announcement.DormitoryId;

            _announcementRepo.DeleteAnnouncement(announcementId);

            return RedirectToAction(nameof(Details), new { id = dormitoryId });
        }

        private bool DormitoryExists(int id)
        {
            return _dormitoryRepo.Dormitories.Any(e => e.DormitoryId == id);
        }

        private static RoomsViewModel CreateRoomsViewModel(int id, Dormitory dormitory)
        {
            return new RoomsViewModel()
            {
                ErrorMessage = "",
                Dormitory = dormitory,
                RoomToAdd = new Room()
                {
                    DormitoryId = id,
                }
            };
        }
    }
}