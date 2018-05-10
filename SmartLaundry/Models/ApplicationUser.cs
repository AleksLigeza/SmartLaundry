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
        [Required(ErrorMessage = "{0} is required"), MaxLength(20, ErrorMessage = "The field {0} must be at max {1} characters long."), RegularExpression("^[a-zA-ZąćęłńóśźżĄĘŁŃÓŚŹŻ]*$", ErrorMessage = "The field {0} is not valid.")]
        [Display(Name = "Firstname")]
        public string Firstname { get; set; }

        [Required(ErrorMessage = "{0} is required"), MaxLength(20, ErrorMessage = "The field {0} must be at max {1} characters long."), RegularExpression("^[a-zA-Z ąćęłńóśźżĄĘŁŃÓŚŹŻ]*$", ErrorMessage = "The field {0} is not valid.")]
        [Display(Name = "Lastname")]
        public string Lastname { get; set; }

        public int? DormitoryManagerId { get; set; }
        public virtual Dormitory DormitoryManager { get; set; }
        public int? DormitoryPorterId { get; set; }
        public virtual Dormitory DormitoryPorter { get; set; }

        public int? RoomId { get; set; }
        public virtual Room Room { get; set; }
    }
}