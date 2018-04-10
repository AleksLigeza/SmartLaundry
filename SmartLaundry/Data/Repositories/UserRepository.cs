using SmartLaundry.Data.Interfaces;
using SmartLaundry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartLaundry.Data.Repositories {
    public class UserRepository : IUserRepository {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext appDbContext) {
            _context = appDbContext;
        }

        public IEnumerable<ApplicationUser> Users => _context.Users;
        List<ApplicationUser> IUserRepository.GetAll() => Users.ToList();
        
        public ApplicationUser GetUserByEmail(string email) => Users.SingleOrDefault(x => x.Email == email);

        public List<ApplicationUser> GetUsersWithEmailLike(string email) => Users.Where(x => x.Email.Contains(email)).ToList();

        public Dormitory AssignDormitoryAsManager(ApplicationUser user, Dormitory dormitory) {
            user.DormitoryManagerID = dormitory.DormitoryID;
            _context.Users.Update(user);
            _context.SaveChanges();
            return _context.Users.Find(user.Id).DormitoryManager;
        }

        public ApplicationUser FindDormitoryManager(int dormitoryId) => _context.Users.SingleOrDefault(x => x.DormitoryManagerID == dormitoryId);


        public List<ApplicationUser> FindDormitoryPorters(int dormitoryId) =>
            _context.Users.Where(x => x.DormitoryPorterID == dormitoryId).ToList();
    }
}
