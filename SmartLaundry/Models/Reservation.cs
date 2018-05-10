using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartLaundry.Models
{
    public class Reservation
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        public bool Fault { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        public bool ToRenew { get; set; }

        public bool Confirmed
        {
            get { return !ToRenew; }
        }

        [Required(ErrorMessage = "{0} is required")]
        public int WashingMachineId { get; set; }

        public virtual WashingMachine WashingMachine { get; set; }

        public int? RoomId { get; set; }
        public virtual Room Room { get; set; }
    }
}