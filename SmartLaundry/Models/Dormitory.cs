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
        public int DormitoryID { get; set; }

        [Required, MaxLength(40), RegularExpression("^[a-zA-Z ąćęłńóśźżĄĘŁŃÓŚŹŻ]*$")]
        public string Name { get; set; }

        [Required, MaxLength(40), RegularExpression("^[a-zA-Z ąćęłńóśźżĄĘŁŃÓŚŹŻ0-9]*$")]
        public string Address { get; set; }

        [Required, RegularExpression("\\d{2}-\\d{3}")]
        public string ZipCode { get; set; }

        [Required, MaxLength(40), RegularExpression("^[a-zA-Z ąćęłńóśźżĄĘŁŃÓŚŹŻ]*$")]
        public string City { get; set; }

        public virtual ApplicationUser Manager { get; set; }
        public virtual ICollection<ApplicationUser> Porters { get; set; }
        public virtual ICollection<Laundry> Laundries { get; set; }
        public virtual ICollection<Room> Rooms { get; set; }
        public virtual ICollection<Announcement> Announcements { get; set; }
    }
}