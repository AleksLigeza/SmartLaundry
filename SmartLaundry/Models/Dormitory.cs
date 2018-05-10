using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartLaundry.Models
{
    public class Dormitory
    {
        [Key]
        public int DormitoryId { get; set; }

        [Required(ErrorMessage = "{0} is required"), MaxLength(40, ErrorMessage = "The field {0} must be at max {1} characters long."), RegularExpression("^[a-zA-Z ąćęłńóśźżĄĘŁŃÓŚŹŻ]*$", ErrorMessage = "The field {0} is not valid.")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "{0} is required"), MaxLength(40, ErrorMessage = "The field {0} must be at max {1} characters long."), RegularExpression("^[a-zA-Z ąćęłńóśźżĄĘŁŃÓŚŹŻ0-9]*$", ErrorMessage = "The field {0} is not valid.")]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required(ErrorMessage = "{0} is required"), RegularExpression("\\d{2}-\\d{3}", ErrorMessage = "The field {0} must be a valid zip code.")]
        [Display(Name = "ZipCode")]
        public string ZipCode { get; set; }

        [Required(ErrorMessage = "{0} is required"), MaxLength(40, ErrorMessage = "The field {0} must be at max {1} characters long."), RegularExpression("^[a-zA-Z ąćęłńóśźżĄĘŁŃÓŚŹŻ]*$", ErrorMessage = "The field {0} is not valid.")]
        [Display(Name = "City")]
        public string City { get; set; }

        [Display(Name = "Manager")]
        public virtual ApplicationUser Manager { get; set; }
        [Display(Name = "Porters")]
        public virtual ICollection<ApplicationUser> Porters { get; set; }
        [Display(Name = "Laundries")]
        public virtual ICollection<Laundry> Laundries { get; set; }
        public virtual ICollection<Room> Rooms { get; set; }
        public virtual ICollection<Announcement> Announcements { get; set; }
    }
}