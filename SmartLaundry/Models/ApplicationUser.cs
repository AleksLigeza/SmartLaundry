using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;


namespace SmartLaundry.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [Required, MaxLength(20), RegularExpression("^[a-zA-ZąćęłńóśźżĄĘŁŃÓŚŹŻ]*$")]
        public string Firstname { get; set; }
        [Required, MaxLength(20), RegularExpression("^[a-zA-Z ąćęłńóśźżĄĘŁŃÓŚŹŻ]*$")]
        public string Lastname { get; set; }

        public int? DormitoryManagerId { get; set; }
        public virtual Dormitory DormitoryManager { get; set; }
        public int? DormitoryPorterId { get; set; }
        public virtual Dormitory DormitoryPorter { get; set; }

        public int? RoomId { get; set; }
        public virtual Room Room { get; set; }
    }
}
