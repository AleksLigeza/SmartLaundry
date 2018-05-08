using Microsoft.EntityFrameworkCore;
using SmartLaundry.Controllers.Helpers;
using SmartLaundry.Data.Interfaces;
using SmartLaundry.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartLaundry.Data.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly ApplicationDbContext _context;

        public ReservationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<Reservation> Reservations => _context.Reservations;

        public void AddReservation(Reservation reservation)
        {
            _context.Reservations.Add(reservation);
            _context.SaveChanges();
        }

        public Reservation GetReservationById(int id)
        {
            return _context.Reservations.FirstOrDefault(x => x.Id == id);
        }

        public List<Reservation> GetRoomReservations(int roomId)
        {
            return _context.Reservations.Where(x => x.RoomId == roomId).ToList();
        }

        public void RemoveReservation(Reservation reservation)
        {
            _context.Reservations.Remove(reservation);
            _context.SaveChanges();
        }

        public void UpdateReservation(Reservation reservation)
        {
            _context.Reservations.Update(reservation);
            _context.SaveChanges();
        }

        public Reservation GetRoomTodaysReservation(int roomId)
        {
            return GetRoomDailyReservation(roomId, DateTime.Now.Date);
        }

        public Reservation GetRoomDailyReservation(int roomId, DateTime date)
        {
            return _context.Reservations
                .FirstOrDefault(
                    x => x.RoomId == roomId
                         && x.StartTime.Date == date.Date
                         && x.Fault == false
                         && (!x.ToRenew || x.ToRenew
                             && (x.StartTime - DateTime.Now).TotalMinutes > 15)
                );
        }
        
        public Reservation GetHourReservation(int machineId, DateTime startTime)
        {
            return _context.Reservations
                .FirstOrDefault(
                    x => x.WashingMachineId == machineId
                         && x.StartTime == startTime
                         && x.Fault == false
                         && x.Confirmed);
        }

        public bool IsFaultAtTime(int machineId, DateTime time)
        {
            return _context.Reservations
                .Any(x => x.Fault && x.WashingMachineId == machineId
                                  && x.StartTime <= time && (x.EndTime == null || x.EndTime >= time));
        }

        public bool IsCurrentlyFault(int machineId)
        {
            return _context.Reservations
                .Any(x => x.Fault && x.WashingMachineId == machineId && x.EndTime == null);
        }

        public bool IsFaultAtTimeToday(int machineId, TimeSpan startTime)
        {
            var dateTime = DateTime.Today;
            dateTime = dateTime.Add(startTime);
            return IsFaultAtTimeAtDay(machineId, dateTime);
        }

        public bool IsFaultAtTimeAtDay(int machineId, DateTime dateTime)
        {
            var machine = _context.WashingMachines.Single(x => x.Id == machineId);
            dateTime = LaundryTimeHelper.GetClosestStartTime(dateTime, machine.Laundry.startTime,
                machine.Laundry.shiftTime, machine.Laundry.shiftCount);

            return IsFaultAtTime(machineId, dateTime);
        }

        public Reservation GetRoomToRenewReservation(int roomId)
        {
            return _context.Reservations
                .SingleOrDefault(x => x.RoomId == roomId
                                      && x.ToRenew
                                      && x.StartTime > DateTime.UtcNow.Subtract(new TimeSpan(48, 0, 0)));
        }

        public bool HasReservationToRenew(int roomId)
        {
            return _context.Reservations
                .Any(x => x.RoomId == roomId
                          && x.ToRenew
                          && x.StartTime > DateTime.Now.AddHours(-48));
        }

        public Dictionary<int, Reservation> GetDormitoryWashingMachineStates(int id)
        {
            var dictionary = new Dictionary<int, Reservation>();
            var laundires = _context.Laundries
                .Where(x => x.DormitoryId == id)
                .ToList();
            foreach (var laundry in laundires)
            {
                foreach (var machine in laundry.WashingMachines)
                {
                    if (machine.Reservations.Any(x => x.Fault))
                    {
                        var lastFaultTime = machine.Reservations.Where(x => x.Fault).Max(x => x.StartTime);
                        var lastFaults = machine.Reservations.Where(x => x.StartTime == lastFaultTime && x.Fault)
                            .ToList();
                        if (lastFaults.Count > 1)
                        {
                            dictionary.Add(machine.Id,
                                lastFaults.Any(x => x.EndTime == null)
                                    ? lastFaults.Single(x => x.EndTime == null)
                                    : lastFaults.OrderByDescending(x => x.EndTime).ToList()[0]);
                        }
                        else
                        {
                            dictionary.Add(machine.Id, lastFaults[0]);
                        }
                    }
                    else
                    {
                        dictionary.Add(machine.Id, null);
                    }
                }
            }

            return dictionary;
        }
    }
}