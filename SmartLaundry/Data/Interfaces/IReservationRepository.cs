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
        void AddReservation(Reservation reservation);
        void RemoveReservation(Reservation reservation);
        void UpdateRescervation(Reservation reservation);
    }
}
