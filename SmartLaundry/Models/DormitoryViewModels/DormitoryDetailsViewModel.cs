using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartLaundry.Models.DormitoryViewModels
{
    public class DormitoryDetailsViewModel {
        public Dormitory Dormitory;
        public ApplicationUser Manager;
        public List<ApplicationUser> Porters;
        public List<Laundry> Laundries;
    }
}
