using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartLaundry.Data;
using SmartLaundry.Data.Interfaces;
using SmartLaundry.Models;
using SmartLaundry.Models.DormitoryViewModels;

namespace SmartLaundry.Controllers {
    public class DormitoryController : Controller {
        private readonly IDormitoryRepository _dormitoryRepo;
        private readonly IUserRepository _userRepo;

        public DormitoryController(IDormitoryRepository dormitoryRepository, IUserRepository userRepository) {
            _dormitoryRepo = dormitoryRepository;
            _userRepo = userRepository;
        }

        [HttpGet]
        public IActionResult Index() {
            return View(_dormitoryRepo.GetAll());
        }

        [HttpGet]
        public IActionResult ManageDormitoryUsers(
            int id,
            string currentFilter,
            string searchString,
            int? page) {

            if(searchString != null) {
                page = 1;
            } else {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            List<ApplicationUser> users;
            if(!String.IsNullOrEmpty(searchString)) {
                users = _userRepo.GetUsersWithEmailLike(searchString);
            } else {
                users = _userRepo.Users.ToList();
            }

            var dormitory = _dormitoryRepo.GetSingleById(id);

            if (dormitory == null) {
                return NotFound();
            }

            int pageSize = 10;
            return View(new ManageDormitoryUsersViewModel(new PaginatedList<ApplicationUser>(users, users.Count, page ?? 1, pageSize), dormitory));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AssignManager(int dormitoryId, string managerEmail) {
            var dormitory = _dormitoryRepo.GetSingleById(dormitoryId);
            var user = _userRepo.GetUserByEmail(managerEmail);

            _dormitoryRepo.AssignManager(user, dormitory);

            return RedirectToAction("Details", new { id = dormitoryId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AssignPorter(int dormitoryId, string porterEmail) {
            var dormitory = _dormitoryRepo.GetSingleById(dormitoryId);
            var user = _userRepo.GetUserByEmail(porterEmail);
            user.DormitoryPorterId = dormitoryId;

            _userRepo.AssignDormitoryAsPorter(user, dormitory);

            return RedirectToAction("Details", new { id = dormitoryId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemovePorter(int dormitoryId, string porterEmail) {
            var dormitory = _dormitoryRepo.GetSingleById(dormitoryId);
            var user = _userRepo.GetUserByEmail(porterEmail);
            user.DormitoryPorterId = dormitoryId;

            _userRepo.RemoveDormitoryPorter(user, dormitory);

            return RedirectToAction("Details", new { id = dormitoryId });
        }

        [HttpGet]
        public IActionResult Details(int? id) {
            if(id == null) {
                return NotFound();
            }

            var dormitory = _dormitoryRepo.GetSingleById(id.Value);
            if(dormitory == null) {
                return NotFound();
            }

            var model = new DormitoryDetailsViewModel();
            model.Dormitory = dormitory;
            model.Manager = _userRepo.GetUserById(dormitory.ManagerId);
            model.Porters = _userRepo.FindDormitoryPorters(dormitory.DormitoryID);

            return View(model);
        }

        [HttpGet]
        public IActionResult Create() {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("DormitoryID,Name,Address,ZipCode,City")] Dormitory dormitory) {
            if(ModelState.IsValid) {
                _dormitoryRepo.AddSingle(dormitory);
                return RedirectToAction(nameof(Index));
            }
            return View(dormitory);
        }

        [HttpGet]
        public IActionResult Edit(int? id) {
            if(id == null) {
                return NotFound();
            }

            var dormitory = _dormitoryRepo.GetSingleById(id.Value);
            if(dormitory == null) {
                return NotFound();
            }
            return View(dormitory);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("DormitoryID,Name,Address,ZipCode,City")] Dormitory dormitory) {
            if(id != dormitory.DormitoryID) {
                return NotFound();
            }

            if(ModelState.IsValid) {
                try {
                    _dormitoryRepo.UpdateSingle(dormitory);
                } catch(DbUpdateConcurrencyException) {
                    if(!DormitoryExists(dormitory.DormitoryID)) {
                        return NotFound();
                    } else {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(dormitory);
        }

        [HttpGet]
        public IActionResult Delete(int? id) {
            if(id == null) {
                return NotFound();
            }

            var dormitory = _dormitoryRepo.GetSingleById(id.Value);
            if(dormitory == null) {
                return NotFound();
            }

            return View(dormitory);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id) {
            var dormitory = _dormitoryRepo.GetSingleById(id);
            _dormitoryRepo.DeleteSingle(dormitory);
            return RedirectToAction(nameof(Index));
        }

        private bool DormitoryExists(int id) {
            return _dormitoryRepo.Dormitories.Any(e => e.DormitoryID == id);
        }
    }
}
