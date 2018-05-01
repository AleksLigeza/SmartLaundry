using Microsoft.EntityFrameworkCore;
using SmartLaundry.Data.Interfaces;
using SmartLaundry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            var laundry = new Laundry()
            {
                DormitoryId = dormitoryId,
                Position = position
            };
            _context.Laundries.Add(laundry);
            _context.SaveChanges();
            return laundry;
        }

        public WashingMachine AddWashingMachine(int laundryId, int position)
        {
            var machine = new WashingMachine()
            {
                LaundryId = laundryId,
                Position = position
            };
            _context.WashingMachines.Add(machine);
            _context.SaveChanges();
            return machine;
        }

        public Laundry GetLaundryWithIncludedEntities(int laundryId)
        {
            return _context.Laundries
                .Where(x => x.Id == laundryId)
                .Include(x => x.WashingMachines)
                .ThenInclude(x => x.Reservations)
                .SingleOrDefault();
        }

        public void RemoveLaundry(Laundry laundry)
        {
            _context.Laundries.Remove(laundry);
            _context.SaveChanges();
        }

        public void RemoveWashingMachine(WashingMachine washingMachine)
        {
            _context.WashingMachines.Remove(washingMachine);
            _context.SaveChanges();
        }

        public List<Laundry> GetDormitoryLaundriesWithEntities(int dormitoryId)
        {
            return _context.Laundries
                .Where(x => x.DormitoryId == dormitoryId)
                .Include(x => x.WashingMachines)
                .ThenInclude(x => x.Reservations)
                .ThenInclude(x => x.Room)
                .ToList();
        }
    }
}
