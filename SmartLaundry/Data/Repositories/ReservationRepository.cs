using SmartLaundry.Data.Interfaces;
using SmartLaundry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartLaundry.Data.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        public readonly ApplicationDbContext _context;
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

        public void UpdateRescervation(Reservation reservation)
        {
            _context.Reservations.Update(reservation);
            _context.SaveChanges();
        }

        public Reservation GetRoomTodaysReservation(int roomId)
        {
            return _context.Reservations
                .FirstOrDefault(
                    x => x.RoomId == roomId
                    && x.StartTime.Date == DateTime.Today.Date
                    && x.Fault == false
                    && x.ToRenew == false);
        }

        public Reservation GetHourReservation(int machineId, DateTime startTime)
        {
            return _context.Reservations
                .FirstOrDefault(
                    x => x.WashingMachineId == machineId
                    && x.StartTime == startTime
                    && x.Fault == false);
        }

        public bool IsFaultAtTime(int machineId, DateTime time)
        {
            return _context.Reservations
                .Where(x => x.Fault == true && x.WashingMachineId == machineId
                    && x.StartTime <= time && (x.EndTime == null || x.EndTime > time))
                .Any();
        }

        public bool IsCurrentlyFault(int machineId)
        {
            return IsFaultAtTime(machineId, DateTime.Now);
        }

        public bool IsFaultAtTimeToday(int machineId, int hour)
        {
            var dateTime = DateTime.Today;
            dateTime = dateTime.AddHours(hour);
            dateTime = GetLastStartTime(dateTime);

            return IsFaultAtTime(machineId, dateTime);
        }

        private DateTime GetLastEndTime(DateTime time)
        {
            if (time.Hour < 15)
            {
                return new DateTime(time.Year, time.Month, time.Day, 14, 59, 59, 999);
            }
            else if (time.Hour < 17)
            {
                return new DateTime(time.Year, time.Month, time.Day, 16, 59, 59, 999);
            }
            else if (time.Hour < 19)
            {
                return new DateTime(time.Year, time.Month, time.Day, 18, 59, 59, 999);
            }
            else if (time.Hour < 21)
            {
                return new DateTime(time.Year, time.Month, time.Day, 20, 59, 59, 999);
            }
            else
            {
                return new DateTime(time.Year, time.Month, time.Day, 22, 59, 59, 999);
            }
        }

        private DateTime GetLastStartTime(DateTime time)
        {
            if (time.Hour < 23)
            {
                return new DateTime(time.Year, time.Month, time.Day, 21, 0, 0, 0);
            }
            else if (time.Hour < 21)
            {
                return new DateTime(time.Year, time.Month, time.Day, 19, 0, 0, 0);
            }
            else if (time.Hour < 19)
            {
                return new DateTime(time.Year, time.Month, time.Day, 17, 0, 0, 0);
            }
            else if (time.Hour < 17)
            {
                return new DateTime(time.Year, time.Month, time.Day, 15, 0, 0, 0);
            }
            else
            {
                return new DateTime(time.Year, time.Month, time.Day, 21, 0, 0, 0);
            }
        }
    }
}
