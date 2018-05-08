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
            return _context.Dormitories.OrderBy(x => x.Name).ToList();
        }

        public Dormitory GetSingleById(int id) => _context.Dormitories.SingleOrDefault(x => x.DormitoryId == id);
        

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

        public bool IsRoomInDormitory(int roomNumber, int dormiotryId) =>
            _context.Rooms.Any(x => x.DormitoryId == dormiotryId && x.Number == roomNumber);
    }
}