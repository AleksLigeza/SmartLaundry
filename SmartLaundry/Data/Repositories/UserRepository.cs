using SmartLaundry.Data.Interfaces;
using SmartLaundry.Models;
using System.Collections.Generic;
using System.Linq;

namespace SmartLaundry.Data.Repositories {
    public class UserRepository : IUserRepository {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext appDbContext) {
            _context = appDbContext;
        }

        public IEnumerable<ApplicationUser> Users => _context.Users;
        List<ApplicationUser> IUserRepository.GetAll() => Users.ToList();
        
        public ApplicationUser GetUserByEmail(string email) => Users.SingleOrDefault(x => x.Email == email);

        public List<ApplicationUser> GetUsersWithEmailLike(string email) => Users.Where(x => x.Email.Contains(email)).OrderBy(x => x.Email).ToList();

        public Dormitory AssignDormitoryAsPorter(ApplicationUser user, Dormitory dormitory) {
            user.DormitoryPorterId = dormitory.DormitoryID;
            _context.Users.Update(user);
            _context.SaveChanges();
            return _context.Users.Find(user.Id).DormitoryPorter;
        }

        public void RemoveDormitoryPorter(ApplicationUser user, Dormitory dormitory) {
            user.DormitoryPorterId = null;
            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public List<ApplicationUser> FindDormitoryPorters(int dormitoryId) =>
            _context.Users.Where(x => x.DormitoryPorterId == dormitoryId).OrderBy(x => x.Email).ToList();

        public ApplicationUser GetUserById(string id) => _context.Users.FirstOrDefault(x => x.Id == id);
    }
}
