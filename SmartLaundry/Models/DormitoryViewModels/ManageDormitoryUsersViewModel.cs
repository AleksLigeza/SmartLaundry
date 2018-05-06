using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartLaundry.Models.DormitoryViewModels
{
    public class ManageDormitoryUsersViewModel
    {
        public PaginatedList<ApplicationUser> Users;
        public Dormitory Dormitory;

        public ManageDormitoryUsersViewModel(PaginatedList<ApplicationUser> users, Dormitory dormitory)
        {
            Users = users;
            Dormitory = dormitory;
        }
    }
}