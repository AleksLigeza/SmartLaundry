using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartLaundry.Models.DormitoryViewModels
{
    public class ManageRoomUsersViewModel
    {
        public PaginatedList<ApplicationUser> Users;
        public Room Room;
    }
}
