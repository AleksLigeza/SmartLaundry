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
    }
}
