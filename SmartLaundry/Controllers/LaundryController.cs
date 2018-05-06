using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartLaundry.Authorization;
using SmartLaundry.Data.Interfaces;
using SmartLaundry.Models;
using SmartLaundry.Models.LaundryViewModels;

namespace SmartLaundry.Controllers
{
    [Authorize]
    public class LaundryController : Controller
    {
        private readonly ILaundryRepository _laundryRepo;
        private readonly IReservationRepository _reservationRepo;
        private readonly IUserRepository _userRepo;
        private readonly IWashingMachineRepository _washingMachineRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly IDormitoryRepository _dormitoryRepo;

        public LaundryController(
            ILaundryRepository laundryRepository, IReservationRepository reservationRepository,
            IUserRepository userRepository, IWashingMachineRepository washingMachineRepo,
            UserManager<ApplicationUser> userManager, IAuthorizationService authorizationService,
            IDormitoryRepository dormitoryRepository)
        {
            _laundryRepo = laundryRepository;
            _reservationRepo = reservationRepository;
            _userRepo = userRepository;
            _userManager = userManager;
            _washingMachineRepo = washingMachineRepo;
            _authorizationService = authorizationService;
            _dormitoryRepo = dormitoryRepository;
        }

        [HttpGet]
        [Authorize(Policy = "MinimumOccupant")]
        public IActionResult Index(int id)
        {
            return RedirectToDay(id, DateTime.Now.Date);
        }

        [HttpGet("/[controller]/[action]/{id}/{year}/{month}/{day}")]
        [Authorize(Policy = "MinimumOccupant")]
        public async Task<IActionResult> Day(int id, int year, int month, int day)
        {
            if (!await CheckDormitoryMembership(id))
            {
                return BadRequest();
            }

            var date = new DateTime(year, month, day);
            var laundries = _laundryRepo.GetDormitoryLaundriesWithEntitiesAtDay(id, date) ?? new List<Laundry>();

            var userId = _userManager.GetUserId(User);
            var roomId = _userRepo.GetUserById(userId).RoomId;

            var model = new DayViewModel()
            {
                Laundries = laundries,
                DormitoryId = id,
                washingMachineState = _reservationRepo.GetDormitoryWashingMachineStates(id),
                date = date
            };

            if (roomId != null)
            {
                model.currentRoomReservation = _reservationRepo.GetRoomDailyReservation(roomId.Value, date);
                model.hasReservationToRenew = _reservationRepo.HasReservationToRenew(roomId.Value);
            }
            else
            {
                model.currentRoomReservation = null;
                model.hasReservationToRenew = false;
            }

            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize(Policy = "MinimumManager")]
        public async Task<IActionResult> AddLaundry(int laundryPosition, int dormitoryId, TimeSpan startTime,
            TimeSpan shiftTime, int shiftCount)
        {
            if (!await CheckDormitoryMembership(dormitoryId))
            {
                return BadRequest();
            }

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

            return RedirectToDayByLaundryId(laundry.Id, DateTime.Now.Date);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize(Policy = "MinimumManager")]
        public async Task<IActionResult> DeleteLaundry(int id)
        {
            var laundry = _laundryRepo.GetLaundryById(id);
            if (laundry == null)
            {
                return null;
            }

            var dormitoryId = _laundryRepo.GetLaundryById(laundry.Id).DormitoryId;

            if (!await CheckDormitoryMembership(dormitoryId))
            {
                return BadRequest();
            }

            _laundryRepo.RemoveLaundry(laundry);

            return RedirectToAction(nameof(Index), new {id = dormitoryId});
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize(Policy = "MinimumManager")]
        public async Task<IActionResult> AddWashingMachine(int id, int machinePosition)
        {
            var laundry = _laundryRepo.GetLaundryById(id);
            if (!await CheckDormitoryMembership(laundry.DormitoryId))
            {
                return BadRequest();
            }

            var machine = _washingMachineRepo.AddWashingMachine(id, machinePosition);
            if (machine == null)
            {
                return BadRequest();
            }

            return RedirectToDayByLaundryId(machine.LaundryId, DateTime.Now.Date);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize(Policy = "MinimumManager")]
        public async Task<IActionResult> RemoveWashingMachine(int machineId)
        {
            var machine = _washingMachineRepo.GetWashingMachineById(machineId);

            if (machine == null)
            {
                return BadRequest();
            }

            if (!await CheckDormitoryMembership(machine))
            {
                return BadRequest();
            }

            _washingMachineRepo.RemoveWashingMachine(machine);
            return RedirectToDayByLaundryId(machine.LaundryId, DateTime.Now.Date);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize(Policy = "MinimumOccupant")]
        public async Task<IActionResult> CancelReservation(int reservationId)
        {
            var reservation = _reservationRepo.GetReservationById(reservationId);
            var machine = _washingMachineRepo.GetWashingMachineById(reservation.WashingMachineId);

            if (!await CheckDormitoryMembership(reservation))
            {
                return BadRequest();
            }

            var userId = _userManager.GetUserId(User);
            var roomId = _userRepo.GetUserById(userId).RoomId;

            if (_reservationRepo.GetHourReservation(reservation.WashingMachineId, reservation.StartTime) == null
                || _reservationRepo.IsFaultAtTime(reservation.WashingMachineId, reservation.StartTime)
                || reservation.StartTime < DateTime.Now
                || roomId != reservation.RoomId && roomId != null)
            {
                return BadRequest();
            }

            // ReSharper disable once PossibleNullReferenceException
            _reservationRepo.RemoveReservation(reservation);

            return RedirectToDayByLaundryId(machine.LaundryId, reservation.StartTime);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize(Policy = "MinimumOccupant")]
        public async Task<IActionResult> Reserve(DateTime startTime, int machineId)
        {
            var machine = _washingMachineRepo.GetWashingMachineById(machineId);
            if (!await CheckDormitoryMembership(machine))
            {
                return BadRequest();
            }

            var userId = _userManager.GetUserId(User);
            var roomId = _userRepo.GetUserById(userId).RoomId;

            var reservation = new Reservation()
            {
                StartTime = startTime,
                RoomId = roomId,
                WashingMachineId = machineId,
                Fault = false
            };

            var reservationAtHour = _reservationRepo.GetHourReservation(machineId, startTime);
            var faultAtTime = _reservationRepo.IsFaultAtTime(machineId, startTime);

            if (reservationAtHour != null
                || faultAtTime
                || reservation.StartTime < DateTime.Now)
            {
                return BadRequest();
            }

            if (_reservationRepo.IsCurrentlyFault(machineId))
            {
                return BadRequest();
            }

            if (roomId != null)
            {
                var toRenew = _reservationRepo.GetRoomToRenewReservation(roomId.Value);
                var roomReservation = _reservationRepo.GetRoomDailyReservation(roomId.Value, startTime);
                if (toRenew != null)
                {
                    _reservationRepo.RemoveReservation(toRenew);
                }
                else if (roomReservation != null)
                {
                    return BadRequest();
                }
            }

            _reservationRepo.AddReservation(reservation);

            return RedirectToDayByLaundryId(machine.LaundryId, startTime);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize(Policy = "MinimumPorter")]
        public async Task<IActionResult> EnableWashingMachine(int machineId)
        {
            var machine = _washingMachineRepo.GetWashingMachineById(machineId);
            if (machine == null)
            {
                return BadRequest();
            }

            if (!await CheckDormitoryMembership(machine))
            {
                return BadRequest();
            }

            if (!_reservationRepo.IsCurrentlyFault(machineId))
            {
                return BadRequest();
            }

            _washingMachineRepo.EnableWashingMachine(machineId);

            return RedirectToDayByLaundryId(machine.LaundryId, DateTime.Now.Date);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize(Policy = "MinimumPorter")]
        public async Task<IActionResult> DisableWashingMachine(int machineId)
        {
            var machine = _washingMachineRepo.GetWashingMachineById(machineId);
            if (machine == null)
            {
                return BadRequest();
            }

            if (!await CheckDormitoryMembership(machine))
            {
                return BadRequest();
            }

            if (_reservationRepo.IsCurrentlyFault(machineId))
            {
                return BadRequest();
            }

            _washingMachineRepo.DisableWashingMachine(machineId);

            return RedirectToDayByLaundryId(machine.LaundryId, DateTime.Now.Date);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize(Policy = "MinimumOccupant")]
        public async Task<IActionResult> ConfirmReservation(int reservationId)
        {
            var reservation = _reservationRepo.GetReservationById(reservationId);

            var machine = _washingMachineRepo.GetWashingMachineById(reservation.WashingMachineId);
            if (!await CheckDormitoryMembership(machine))
            {
                return BadRequest();
            }

            if (_reservationRepo.IsFaultAtTime(reservation.WashingMachineId, reservation.StartTime)
                || reservation.Confirmed
                || reservation.StartTime < DateTime.Now)
            {
                return BadRequest();
            }

            reservation.ToRenew = false;
            _reservationRepo.UpdateReservation(reservation);

            return RedirectToDayByLaundryId(machine.LaundryId, reservation.StartTime.Date);
        }


        private IActionResult RedirectToDayByLaundryId(int laundryId, DateTime day)
        {
            var dormitoryId = _laundryRepo.GetLaundryById(laundryId).DormitoryId;
            return RedirectToDay(dormitoryId, day);
        }

        private IActionResult RedirectToDay(int dormitoryId, DateTime day)
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

        private async Task<bool> CheckDormitoryMembership(WashingMachine machine)
        {
            var dormitoryId = _laundryRepo.GetLaundryById(machine.LaundryId).DormitoryId;
            return await CheckDormitoryMembership(dormitoryId);
        }

        private async Task<bool> CheckDormitoryMembership(Reservation reservation)
        {
            var machine = _washingMachineRepo.GetWashingMachineById(reservation.WashingMachineId);
            return await CheckDormitoryMembership(machine);
        }
    }
}