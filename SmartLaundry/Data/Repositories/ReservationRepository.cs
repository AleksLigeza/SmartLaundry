using SmartLaundry.Controllers.Helpers;
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

        public void UpdateReservation(Reservation reservation)
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
                    && (!x.ToRenew || x.ToRenew && (x.StartTime - DateTime.Now).TotalMinutes > 15));
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
                    && x.StartTime <= time && (x.EndTime == null || x.EndTime >= time))
                .Any();
        }

        public bool IsCurrentlyFault(int machineId)
        {
            return _context.Reservations
                .Where(x => x.Fault == true && x.WashingMachineId == machineId && x.EndTime == null)
                .Any();
        }

        public bool IsFaultAtTimeToday(int machineId, int hour)
        {
            var dateTime = DateTime.Today;
            dateTime = dateTime.AddHours(hour);
            dateTime = LaundryTimeHelper.GetClosestStartTime(dateTime);

            return IsFaultAtTime(machineId, dateTime);
        }

        public Reservation GetRoomToRenewReservation(int roomId)
        {
            return _context.Reservations
                .Where(x => x.RoomId == roomId
                    && x.ToRenew
                    && x.StartTime > DateTime.UtcNow.Subtract(new TimeSpan(48, 0, 0)))
                .SingleOrDefault();
        }
    }
}
