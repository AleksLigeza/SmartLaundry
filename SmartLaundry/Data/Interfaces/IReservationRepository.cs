using SmartLaundry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartLaundry.Data.Interfaces
{
    public interface IReservationRepository
    {
        IQueryable<Reservation> Reservations { get; }

        Reservation GetReservationById(int id);
        List<Reservation> GetRoomReservations(int roomId);
        Reservation GetRoomTodaysReservation(int roomId);
        Reservation GetRoomDailyReservation(int roomId, DateTime date);
        void AddReservation(Reservation reservation);
        void RemoveReservation(Reservation reservation);
        void UpdateReservation(Reservation reservation);
        Reservation GetHourReservation(int machineId, DateTime startTime);
        bool IsFaultAtTime(int machineId, DateTime time);
        bool IsFaultAtTimeToday(int machineId, TimeSpan startTime);
        bool IsFaultAtTimeAtDay(int machineId, DateTime dateTime);
        bool IsCurrentlyFault(int machineId);
        Reservation GetRoomToRenewReservation(int roomId);
        bool HasReservationToRenew(int roomId);
        Dictionary<int, Reservation> GetDormitoryWashingMachineStates(int id);
    }
}