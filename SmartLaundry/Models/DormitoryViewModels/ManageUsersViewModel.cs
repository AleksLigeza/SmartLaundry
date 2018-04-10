using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartLaundry.Models.DormitoryViewModels
{
    public class ManageUsersViewModel {
        public List<ApplicationUser> Users;
        public int DormitoryId;
        public string Search;
    }
}
