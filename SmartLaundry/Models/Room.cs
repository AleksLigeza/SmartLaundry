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

        [Required, Range(1, 2000)]
        public int Number { get; set; }

        [Required]
        public int DormitoryId { get; set; }

        public virtual Dormitory Dormitory { get; set; }

        public virtual ICollection<ApplicationUser> Occupants { get; set; }
        public virtual ICollection<Reservation> Reservations { get; set; }
    }
}