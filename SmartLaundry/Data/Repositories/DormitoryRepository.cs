using SmartLaundry.Data.Interfaces;
using SmartLaundry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartLaundry.Data.Repositories {
    public class DormitoryRepository : IDormitoryRepository {
        private readonly ApplicationDbContext _context;

        public DormitoryRepository(ApplicationDbContext appDbContext) {
            _context = appDbContext;
        }
        public IEnumerable<Dormitory> Dormitories => _context.Dormitories;

        public Dormitory AddSingle(Dormitory source) {
            var result = _context.Dormitories.Add(source);
            _context.SaveChanges();
            return result.Entity;
        }

        public void DeleteSingle(Dormitory source) {
            _context.Dormitories.Remove(source);
            _context.SaveChanges();
        }

        public List<Dormitory> GetAll() {
            return Dormitories.OrderBy(x => x.Name).ToList();
        }

        public Dormitory GetSingleById(int id) {
            return _context.Dormitories.Find(id);
        }

        public Dormitory UpdateSingle(Dormitory source) {
            var result = _context.Dormitories.Update(source);
            _context.SaveChanges();
            return result.Entity;
        }
    }
}
