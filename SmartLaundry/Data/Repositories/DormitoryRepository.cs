using Microsoft.EntityFrameworkCore;
using SmartLaundry.Data.Interfaces;
using SmartLaundry.Models;
using System.Collections.Generic;
using System.Linq;

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
            return _context.Dormitories.Where(x => x.DormitoryId == id).Include(x => x.Rooms).SingleOrDefault();
        }

        public Dormitory GetSingleWithIncludes(int id)
        {
            return _context.Dormitories.Include(x => x.Laundries)
                .Include(x => x.Manager).Include(x => x.Porters)
                .Include(x => x.Rooms).SingleOrDefault(x => x.DormitoryId == id);
        }

        public Dormitory UpdateSingle(Dormitory source)
        {
            var result = _context.Dormitories.Update(source);
            _context.SaveChanges();
            return result.Entity;
        }

        public Dormitory AssignManager(ApplicationUser user, Dormitory dormitory)
        {
            user.DormitoryManagerId = dormitory.DormitoryId;
            _context.Dormitories.Update(dormitory);
            _context.Users.Update(user);
            _context.SaveChanges();
            return _context.Users.Find(user.Id).DormitoryManager;
        }

        public Dormitory GetDormitoryWithRooms(int id)
        {
            return _context.Dormitories.Where(x => x.DormitoryId == id).Include(x => x.Rooms)
                .ThenInclude(x => x.Occupants).SingleOrDefault();
        }

        public bool DormitoryHasRoom(int roomNumber, int dormiotryId) =>
            _context.Rooms.Any(x => x.DormitoryId == dormiotryId && x.Number == roomNumber);
    }
}