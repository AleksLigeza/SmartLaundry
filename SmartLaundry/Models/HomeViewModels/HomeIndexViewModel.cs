using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartLaundry.Models.AccountViewModels;
using SmartLaundry.Models.LaundryViewModels;

namespace SmartLaundry.Models.HomeViewModels
{
    public class HomeIndexViewModel
    {
        public LoginViewModel LoginViewModel { get; set; }
        public RegisterViewModel RegisterViewModel { get; set; }
        public DayViewModel DayViewModel { get; internal set; }
        public List<Announcement> Announcements { get; internal set; }
        public Dormitory Dormitory { get; internal set; }

    }
}
