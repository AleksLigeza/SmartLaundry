using Microsoft.EntityFrameworkCore;
using SmartLaundry.Data.Interfaces;
using SmartLaundry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartLaundry.Data.Repositories
{
    public class DormitoryRepository : IDormitoryRepository
    {
        private readonly ApplicationDbContext _context;

        public DormitoryRepository(ApplicationDbContext appDbContext)
        {
            _context = appDbContext;
        }
        public IEnumerable<Dormitory> Dormitories => _context.Dormitories;

        public Dormitory AddSingle(Dormitory source)
        {
            var result = _context.Dormitories.Add(source);
            _context.SaveChanges();
            return result.Entity;
        }

        public void DeleteSingle(Dormitory source)
        {
            _context.Dormitories.Remove(source);
            _context.SaveChanges();
        }

        public List<Dormitory> GetAll()
        {
            return _context.Dormitories.Include(x => x.Rooms).OrderBy(x => x.Name).ToList();
        }

        public Dormitory GetSingleById(int id)
        {
            return _context.Dormitories.Where(x => x.DormitoryID == id).Include(x => x.Rooms).Single();
        }

        public Dormitory GetSingleWithIncludes(int id)
        {
            return _context.Dormitories
                .Include(x => x.Laundries).Include(x => x.Manager)
                .Include(x => x.Porters).Include(x => x.Rooms)
                .Where(x => x.DormitoryID == id).SingleOrDefault();
        }

        public Dormitory UpdateSingle(Dormitory source)
        {
            var result = _context.Dormitories.Update(source);
            _context.SaveChanges();
            return result.Entity;
        }

        public Dormitory AssignManager(ApplicationUser user, Dormitory dormitory)
        {
            user.DormitoryManagerId = dormitory.DormitoryID;
            _context.Dormitories.Update(dormitory);
            _context.Users.Update(user);
            _context.SaveChanges();
            return _context.Users.Find(user.Id).DormitoryManager;
        }

        public Dormitory GetDormitoryWithRooms(int id)
        {
            return _context.Dormitories.Where(x => x.DormitoryID == id).Include(x => x.Rooms).ThenInclude(x => x.Occupants).SingleOrDefault();
        }

        public bool DormitoryHasRoom(int roomNumber, int dormiotryId)
        {
            var room = _context.Rooms.Where(x => x.DormitoryId == dormiotryId && x.Number == roomNumber).SingleOrDefault();
            return room != null;
        }
    }
}
