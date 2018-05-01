using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartLaundry.Data.Interfaces;
using SmartLaundry.Models;
using SmartLaundry.Models.LaundryViewModels;

namespace SmartLaundry.Controllers
{
    public class LaundryController : Controller
    {
        private readonly ILaundryRepository _laundryRepo;
        private readonly IReservationRepository _resetvationRepo;
        private readonly IUserRepository _userRepo;
        private readonly UserManager<ApplicationUser> _userManager;


        public LaundryController(
            ILaundryRepository laundryRepository,
            IReservationRepository reservationRepository,
            IUserRepository userRepository,
            UserManager<ApplicationUser> userManager)
        {
            _laundryRepo = laundryRepository;
            _resetvationRepo = reservationRepository;
            _userRepo = userRepository;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index(int id)
        {
            var laundries = _laundryRepo.GetDormitoryLaundriesWithEntities(id);

            if (laundries == null)
            {
                laundries = new List<Laundry>();
            }

            var userId = _userManager.GetUserId(User);
            var roomId = _userRepo.GetUserById(userId).RoomId;

            var model = new IndexViewModel()
            {
                Laundries = laundries,
                DormitoryId = id,
                currentRoomReservation = _resetvationRepo.GetRoomTodaysReservation(roomId.Value)
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult AddLaundry(int laundryPosition, int dormitoryId)
        {
            var laundry = _laundryRepo.AddLaundry(dormitoryId, laundryPosition);

            if (laundry == null)
            {
                return BadRequest();
            }

            return RedirectToAction(nameof(Index), new { id = dormitoryId });
        }

        [HttpPost]
        public IActionResult DeleteLaundry(int id)
        {
            var laundry = _laundryRepo.GetLaundryById(id);
            var dormitoryId = laundry.DormitoryId;
            if (laundry == null)
            {
                return null;
            }

            _laundryRepo.RemoveLaundry(laundry);
            return RedirectToAction(nameof(Index), new { id = dormitoryId });
        }

        [HttpPost]
        public IActionResult AddWashingMachine(int id, int machinePosition)
        {
            var machine = _laundryRepo.AddWashingMachine(id, machinePosition);
            var dormitoryId = _laundryRepo.GetLaundryById(id).DormitoryId;
            if (machine == null)
            {
                return BadRequest();
            }

            return RedirectToAction(nameof(Index), new { id = dormitoryId });
        }

        [HttpPost]
        public IActionResult RemoveWashingMachine(int machineId)
        {
            var machine = _laundryRepo.GetWashingMachineById(machineId);

            if (machine == null)
            {
                return BadRequest();
            }
            var dormitoryId = _laundryRepo.GetLaundryById(machine.LaundryId.Value).DormitoryId;

            _laundryRepo.RemoveWashingMachine(machine);
            return RedirectToAction(nameof(Index), new { id = dormitoryId });
        }

        [HttpPost]
        public IActionResult CancelReservation(int reservationId)
        {
            var reservation = _resetvationRepo.GetReservationById(reservationId);
                
            if (reservation == null)
            {
                return BadRequest();
            }

            var laundryId = _laundryRepo.GetWashingMachineById(reservation.WashingMachineId.Value).LaundryId;
            var dormitoryId = _laundryRepo.GetLaundryById(laundryId.Value).DormitoryId;

            _resetvationRepo.RemoveReservation(reservation);

            return RedirectToAction(nameof(Index), new { id = dormitoryId });
        }

        [HttpPost]
        public IActionResult Reserve(int hour, int machineId)
        {
            var userId = _userManager.GetUserId(User);
            var roomId = _userRepo.GetUserById(userId).RoomId;

            var today = DateTime.Today;
            var reservation = new Reservation()
            {
                StartTime = new DateTime(today.Year, today.Month, today.Day, hour, 0, 0),
                RoomId = roomId,
                WashingMachineId = machineId
            };

            if (reservation == null)
            {
                return BadRequest();
            }

            _resetvationRepo.AddReservation(reservation);

            var laundryId = _laundryRepo.GetWashingMachineById(machineId).LaundryId.Value;
            var dormitoryId = _laundryRepo.GetLaundryById(laundryId).DormitoryId;

            return RedirectToAction(nameof(Index), new { id = dormitoryId });
        }
    }
}