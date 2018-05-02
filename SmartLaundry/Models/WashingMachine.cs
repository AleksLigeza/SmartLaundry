using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartLaundry.Models
{
    public class WashingMachine
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Range(1, 10)]
        public int Position { get; set; }

        [Required]
        public int LaundryId { get; set; }
        public virtual Laundry Laundry { get; set; }

        public virtual ICollection<Reservation> Reservations { get; set; }
    }
}
