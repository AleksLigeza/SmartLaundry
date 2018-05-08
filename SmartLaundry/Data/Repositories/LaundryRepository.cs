using Microsoft.EntityFrameworkCore;
using SmartLaundry.Data.Interfaces;
using SmartLaundry.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartLaundry.Data.Repositories
{
    public class LaundryRepository : ILaundryRepository
    {
        private readonly ApplicationDbContext _context;

        public LaundryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<Laundry> Laundries => _context.Laundries;

        public Laundry AddLaundry(int dormitoryId, int position)
        {
            if (_context.Laundries.Any(x => x.DormitoryId == dormitoryId && x.Position == position))
            {
                return null;
            }

            var laundry = new Laundry()
            {
                DormitoryId = dormitoryId,
                Position = position,
                startTime = new TimeSpan(15, 0, 0),
                shiftTime = new TimeSpan(2, 0, 0),
                shiftCount = 4
            };

            _context.Laundries.Add(laundry);
            _context.SaveChanges();
            return laundry;
        }

        public void UpdateLaundry(Laundry laundry)
        {
            _context.Laundries.Update(laundry);
            _context.SaveChanges();
        }

        public void RemoveLaundry(Laundry laundry)
        {
            if (laundry == null) return;
            laundry = _context.Laundries
                .Single(x => x.Id == laundry.Id);
            _context.Laundries.Remove(laundry);
            _context.SaveChanges();
        }

        public List<Laundry> GetDormitoryLaundriesWithEntitiesAtDay(int dormitoryId, DateTime date)
        {
            return _context.Laundries
                .Select(n => new Laundry
                {
                    DormitoryId = n.DormitoryId,
                    Id = n.Id,
                    Position = n.Position,
                    shiftCount = n.shiftCount,
                    shiftTime = n.shiftTime,
                    startTime = n.startTime,
                    Dormitory = n.Dormitory,
                    WashingMachines = n.WashingMachines.Select(w => new WashingMachine
                    {
                        Id = w.Id,
                        LaundryId = w.LaundryId,
                        Position = w.Position,
                        Laundry = w.Laundry,
                        Reservations = w.Reservations.Select(r => new Reservation
                        {
                            Id = r.Id,
                            Room = r.Room,
                            RoomId = r.RoomId,
                            ToRenew = r.ToRenew,
                            EndTime = r.EndTime,
                            Fault = r.Fault,
                            StartTime = r.StartTime,
                            WashingMachine = r.WashingMachine,
                            WashingMachineId = r.WashingMachineId
                        }).Where(r => r.StartTime.Date == date.Date).ToList()
                    }).ToList()
                }).Where(x => x.DormitoryId == dormitoryId).OrderBy(x => x.Position).ToList();
        }

        public Laundry GetLaundryById(int id) => _context.Laundries.FirstOrDefault(x => x.Id == id);
    }
}