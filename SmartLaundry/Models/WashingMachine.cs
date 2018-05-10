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

        [Required(ErrorMessage = "{0} is required")]
        [Range(1, 10, ErrorMessage = "{0} must be a number between {1} and {2}")]
        [Display(Name = "Position")]
        public int Position { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        public int LaundryId { get; set; }

        public virtual Laundry Laundry { get; set; }

        public virtual ICollection<Reservation> Reservations { get; set; }
    }
}