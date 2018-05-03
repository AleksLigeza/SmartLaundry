using Microsoft.EntityFrameworkCore;
using SmartLaundry.Controllers.Helpers;
using SmartLaundry.Data.Interfaces;
using SmartLaundry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartLaundry.Data.Repositories
{
    public class WashingMachineRepository : IWashingMachineRepository
    {
        private readonly ApplicationDbContext _context;
        public WashingMachineRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<WashingMachine> WashingMachines => _context.WashingMachines;

        public WashingMachine GetWashingMachineById(int id) => _context.WashingMachines.FirstOrDefault(x => x.Id == id);

        public WashingMachine AddWashingMachine(int laundryId, int position)
        {
            if (_context.WashingMachines
                .Any(x => x.LaundryId == laundryId
                    && x.Position == position))
            {
                return null;
            }

            var machine = new WashingMachine()
            {
                LaundryId = laundryId,
                Position = position
            };
            _context.WashingMachines.Add(machine);
            _context.SaveChanges();
            return machine;
        }

        public void RemoveWashingMachine(WashingMachine washingMachine)
        {
            washingMachine = _context.WashingMachines
                .Where(x => x.Id == washingMachine.Id)
                .Include(x => x.Reservations)
                .Single();
            _context.WashingMachines.Remove(washingMachine);
            _context.SaveChanges();
        }

        public WashingMachine DisableWashingMachine(int id)
        {
            var startTime = DateTime.Now;
            startTime = LaundryTimeHelper.GetClosestStartTime(startTime);
            
            var machine = _context.WashingMachines.Where(x => x.Id == id)
                .Include(x => x.Reservations)
                .SingleOrDefault();

            var toRenew = machine.Reservations.Where(y => y.StartTime >= startTime && y.Fault == false).ToList();
            if (machine == null)
            {
                return machine;
            }

            foreach (var reservation in toRenew)
            {
                reservation.ToRenew = true;
                reservation.Confirmed = false;
            }

            var faultReservation = new Reservation
            {
                Fault = true,
                StartTime = startTime,
                WashingMachineId = id,
                ToRenew = false,
                Confirmed = true
            };
            _context.Reservations.Add(faultReservation);

            _context.SaveChanges();
            return machine;
        }

        public WashingMachine EnableWashingMachine(int id)
        {
            var machine = _context.WashingMachines.Where(x => x.Id == id)
                .Include(x => x.Reservations)
                .SingleOrDefault();

            if (machine == null)
            {
                return null;
            }

            var maxStartTime = machine.Reservations
                .Where(y => y.Fault == true)
                .Max(y => y.StartTime);

            var lastReservation = machine.Reservations
                .SingleOrDefault(x => x.StartTime == maxStartTime && x.Fault == true && x.EndTime == null);

            lastReservation.EndTime = DateTime.Now;
            _context.SaveChanges();
            return machine;
        }
    }
}
