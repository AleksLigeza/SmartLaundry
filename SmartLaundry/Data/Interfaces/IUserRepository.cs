using SmartLaundry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartLaundry.Data.Interfaces
{
    public interface IUserRepository
    {
        IEnumerable<ApplicationUser> Users { get; }
        List<ApplicationUser> GetAll();
        ApplicationUser GetUserByEmail(string email);
        List<ApplicationUser> GetUsersWithEmailLike(string email);
        Dormitory AssignDormitoryAsManager(ApplicationUser user, Dormitory dormitory);
        ApplicationUser FindDormitoryManager(int dormitoryId);
        List<ApplicationUser> FindDormitoryPorters(int dormitoryId);
    }
}
