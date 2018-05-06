using Microsoft.EntityFrameworkCore;
using SmartLaundry.Controllers.Helpers;
using SmartLaundry.Data.Interfaces;
using SmartLaundry.Models;
using System;
using System.Linq;

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

        public void RemoveWashingMachine(WashingMachine washingMachine) {
            if (washingMachine == null) return;
            var machine = washingMachine;
            washingMachine = _context.WashingMachines
                .Where(x => x.Id == machine.Id)
                .Include(x => x.Reservations)
                .Single();
            _context.WashingMachines.Remove(washingMachine);
            _context.SaveChanges();
        }

        public WashingMachine DisableWashingMachine(int id)
        {
            var machine = _context.WashingMachines.Where(x => x.Id == id)
                .Include(x => x.Reservations)
                .Include(x => x.Laundry)
                .Single();

            var startTime = DateTime.Now;
            startTime = LaundryTimeHelper.GetClosestStartTime(
                startTime, machine.Laundry.startTime,
                machine.Laundry.shiftTime,
                machine.Laundry.shiftCount
            );

            var toRenew = machine.Reservations.Where(y => y.StartTime >= startTime && y.Fault == false).ToList();


            foreach (var reservation in toRenew)
            {
                reservation.ToRenew = true;
            }

            var faultReservation = new Reservation
            {
                Fault = true,
                StartTime = startTime,
                WashingMachineId = id,
                ToRenew = false
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
                .Where(y => y.Fault)
                .Max(y => y.StartTime);

            var lastReservation = machine.Reservations
                .Single(x => x.StartTime == maxStartTime && x.Fault && x.EndTime == null);

            lastReservation.EndTime = DateTime.Now;
            _context.SaveChanges();
            return machine;
        }
    }
}
