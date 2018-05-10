using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartLaundry.Models.DormitoryViewModels
{
    public class DormitoryDetailsViewModel
    {
        public Dormitory Dormitory;
        [Display(Name = "Manager")]
        public ApplicationUser Manager;
        public List<ApplicationUser> Porters;
        public List<Laundry> Laundries;

        public List<Announcement> Announcements { get; internal set; }
    }
}