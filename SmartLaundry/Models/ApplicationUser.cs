using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace SmartLaundry.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public virtual Dormitory DormitoryManager { get; set; }
        public int? DormitoryPorterId { get; set; }
        public virtual Dormitory DormitoryPorter { get; set; }
    }
}
