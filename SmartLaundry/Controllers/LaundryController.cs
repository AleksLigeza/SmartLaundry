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
        private readonly IWashingMachineRepository _washingMachineRepo;
        private readonly UserManager<ApplicationUser> _userManager;

        public LaundryController(
            ILaundryRepository laundryRepository,
            IReservationRepository reservationRepository,
            IUserRepository userRepository,
            IWashingMachineRepository washingMachineRepo,
            UserManager<ApplicationUser> userManager)
        {
            _laundryRepo = laundryRepository;
            _resetvationRepo = reservationRepository;
            _userRepo = userRepository;
            _userManager = userManager;
            _washingMachineRepo = washingMachineRepo;
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
                currentRoomReservation = _resetvationRepo.GetRoomTodaysReservation(roomId.Value),
                isFaultAtTimeToday = _resetvationRepo.IsFaultAtTimeToday,
                isCurrentlyFault = _resetvationRepo.IsCurrentlyFault
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

            return redirectToIndex(laundry.Id);

        }

        [HttpPost]
        public IActionResult ChangeShifts(int laundryId, TimeSpan startTime, TimeSpan shiftTime, int shiftCount)
        {
            TimeSpan wholeWorkingTime = startTime + shiftTime * shiftCount;
            if (wholeWorkingTime.TotalHours > 24)
            {
                return BadRequest();
            }

            var laundry = _laundryRepo.GetLaundryById(laundryId);

            laundry.startTime = startTime;
            laundry.shiftCount = shiftCount;
            laundry.shiftTime = shiftTime;

            _laundryRepo.UpdateLaundry(laundry);

            return redirectToIndex(laundryId);
        }

        [HttpPost]
        public IActionResult DeleteLaundry(int id)
        {
            var laundry = _laundryRepo.GetLaundryById(id);
            if (laundry == null)
            {
                return null;
            }
            var dormitoryId = _laundryRepo.GetLaundryById(laundry.Id).DormitoryId;

            _laundryRepo.RemoveLaundry(laundry);

            return RedirectToAction(nameof(Index), new { id = dormitoryId });

        }

        [HttpPost]
        public IActionResult AddWashingMachine(int id, int machinePosition)
        {
            var machine = _washingMachineRepo.AddWashingMachine(id, machinePosition);
            if (machine == null)
            {
                return BadRequest();
            }

            return redirectToIndex(machine.LaundryId);
        }

        [HttpPost]
        public IActionResult RemoveWashingMachine(int machineId)
        {
            var machine = _washingMachineRepo.GetWashingMachineById(machineId);

            if (machine == null)
            {
                return BadRequest();
            }

            _washingMachineRepo.RemoveWashingMachine(machine);
            return redirectToIndex(machine.LaundryId);

        }

        [HttpPost]
        public IActionResult CancelReservation(int reservationId)
        {
            var reservation = _resetvationRepo.GetReservationById(reservationId);

            if (reservation == null
                || _resetvationRepo.GetHourReservation(reservation.WashingMachineId, reservation.StartTime) == null
                || _resetvationRepo.IsFaultAtTime(reservation.WashingMachineId, reservation.StartTime)
                || reservation.StartTime < DateTime.Now)
            {
                return BadRequest();
            }

            var laundryId = _washingMachineRepo.GetWashingMachineById(reservation.WashingMachineId).LaundryId;

            _resetvationRepo.RemoveReservation(reservation);

            return redirectToIndex(laundryId);

        }

        [HttpPost]
        public IActionResult Reserve(TimeSpan hour, int machineId)
        {
            var userId = _userManager.GetUserId(User);
            var roomId = _userRepo.GetUserById(userId).RoomId;

            var startTime = DateTime.Today;
            startTime = startTime.Add(hour);

            var reservation = new Reservation()
            {
                StartTime = startTime,
                RoomId = roomId,
                WashingMachineId = machineId,
                Fault = false
            };

            var reservationAtHour = _resetvationRepo.GetHourReservation(machineId, startTime);
            var faultAtTime = _resetvationRepo.IsFaultAtTime(machineId, startTime);

            if (reservation == null
                || reservationAtHour != null
                || faultAtTime
                || reservation.StartTime < DateTime.Now)
            {
                return BadRequest();
            }

            var machine = _washingMachineRepo.GetWashingMachineById(machineId);

            if (_resetvationRepo.IsCurrentlyFault(machineId))
            {
                return BadRequest();
            }

            var toRenew = _resetvationRepo.GetRoomToRenewReservation(roomId.Value);
            var roomReservation = _resetvationRepo.GetRoomTodaysReservation(roomId.Value);
            if (toRenew != null)
            {
                _resetvationRepo.RemoveReservation(toRenew);
            }
            else if (roomReservation != null)
            {
                return BadRequest();
            }

            _resetvationRepo.AddReservation(reservation);

            return redirectToIndex(machine.LaundryId);
        }

        [HttpPost]
        public IActionResult EnableWashingMachine(int machineId)
        {
            if (!_resetvationRepo.IsCurrentlyFault(machineId))
            {
                return BadRequest();
            }

            var machine = _washingMachineRepo.EnableWashingMachine(machineId);
            if (machine == null)
            {
                return BadRequest();
            }

            return redirectToIndex(machine.LaundryId);
        }

        [HttpPost]
        public IActionResult DisableWashingMachine(int machineId)
        {
            if (_resetvationRepo.IsCurrentlyFault(machineId))
            {
                return BadRequest();
            }

            var machine = _washingMachineRepo.DisableWashingMachine(machineId);
            if (machine == null)
            {
                return BadRequest();
            }

            return redirectToIndex(machine.LaundryId);
        }

        [HttpPost]
        public IActionResult ConfirmReservation(int reservationId)
        {
            var reservation = _resetvationRepo.GetReservationById(reservationId);

            if (reservation == null
                || _resetvationRepo.IsFaultAtTime(reservation.WashingMachineId, reservation.StartTime)
                || reservation.Confirmed == true
                || reservation.StartTime < DateTime.Now)
            {
                return BadRequest();
            }

            reservation.ToRenew = false;
            _resetvationRepo.UpdateReservation(reservation);

            var laundryId = _washingMachineRepo.GetWashingMachineById(reservation.WashingMachineId).LaundryId;
            return redirectToIndex(laundryId);
        }


        private IActionResult redirectToIndex(int laundryId)
        {
            var dormitoryId = _laundryRepo.GetLaundryById(laundryId).DormitoryId;
            return RedirectToAction(nameof(Index), new { id = dormitoryId });
        }
    }
}