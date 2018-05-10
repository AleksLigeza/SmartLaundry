using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartLaundry.Models
{
    public class Room
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "{0} is required"), Range(1, 2000)]
        [Display(Name = "Number")]
        public int Number { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        public int DormitoryId { get; set; }

        public virtual Dormitory Dormitory { get; set; }

        public virtual ICollection<ApplicationUser> Occupants { get; set; }
        public virtual ICollection<Reservation> Reservations { get; set; }
    }
}