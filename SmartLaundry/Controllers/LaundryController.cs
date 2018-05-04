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
            return redirectToDay(id, DateTime.Now.Date);
        }

        [HttpGet("/[controller]/[action]/{id}/{year}/{month}/{day}")]
        public IActionResult Day(int id, int year, int month, int day)
        {
            var date = new DateTime(year, month, day);
            var laundries = _laundryRepo.GetDormitoryLaundriesWithEntitiesAtDay(id, date);

            if (laundries == null)
            {
                laundries = new List<Laundry>();
            }

            var userId = _userManager.GetUserId(User);
            var roomId = _userRepo.GetUserById(userId).RoomId;

            var model = new DayViewModel()
            {
                Laundries = laundries,
                DormitoryId = id,
                currentRoomReservation = _resetvationRepo.GetRoomDailyReservation(roomId.Value, date),
                hasReservationToRenew = _resetvationRepo.HasReservationToRenew(roomId.Value),
                washingMachineState = _resetvationRepo.GetDormitoryWashingMachineStates(id),
                date = date
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult AddLaundry(int laundryPosition, int dormitoryId, TimeSpan startTime, TimeSpan shiftTime, int shiftCount)
        {
            TimeSpan wholeWorkingTime = startTime + shiftTime * shiftCount;
            if (wholeWorkingTime.TotalHours > 24)
            {
                return BadRequest();
            }

            var laundry = _laundryRepo.AddLaundry(dormitoryId, laundryPosition);

            if (laundry == null)
            {
                return BadRequest();
            }

            laundry.startTime = startTime;
            laundry.shiftCount = shiftCount;
            laundry.shiftTime = shiftTime;

            _laundryRepo.UpdateLaundry(laundry);

            return redirectToDayByLaundryId(laundry.Id, DateTime.Now.Date);

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

            return redirectToDayByLaundryId(machine.LaundryId, DateTime.Now.Date);
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
            return redirectToDayByLaundryId(machine.LaundryId, DateTime.Now.Date);

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

            return redirectToDayByLaundryId(laundryId, reservation.StartTime);

        }

        [HttpPost]
        public IActionResult Reserve(DateTime startTime, int machineId)
        {
            var userId = _userManager.GetUserId(User);
            var roomId = _userRepo.GetUserById(userId).RoomId;

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
            var roomReservation = _resetvationRepo.GetRoomDailyReservation(roomId.Value, startTime);
            if (toRenew != null)
            {
                _resetvationRepo.RemoveReservation(toRenew);
            }
            else if (roomReservation != null)
            {
                return BadRequest();
            }

            _resetvationRepo.AddReservation(reservation);

            return redirectToDayByLaundryId(machine.LaundryId, startTime);
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

            return redirectToDayByLaundryId(machine.LaundryId, DateTime.Now.Date);
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

            return redirectToDayByLaundryId(machine.LaundryId, DateTime.Now.Date);
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
            return redirectToDayByLaundryId(laundryId, reservation.StartTime.Date);
        }


        private IActionResult redirectToDayByLaundryId(int laundryId, DateTime day)
        {
            var dormitoryId = _laundryRepo.GetLaundryById(laundryId).DormitoryId;
            return redirectToDay(dormitoryId, day);
        }

        private IActionResult redirectToDay(int dormitoryId, DateTime day)
        {
            return RedirectToAction(
                nameof(Day),
                new
                {
                    id = dormitoryId,
                    year = day.Year,
                    month = day.Month,
                    day = day.Day
                });
        }
    }
}