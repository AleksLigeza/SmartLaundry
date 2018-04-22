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
        Dormitory AssignDormitoryAsPorter(ApplicationUser user, Dormitory dormitory);
        void RemoveDormitoryPorter(ApplicationUser user, Dormitory dormitory);
        List<ApplicationUser> FindDormitoryPorters(int dormitoryId);
        ApplicationUser GetUserById(string id);
    }
}
