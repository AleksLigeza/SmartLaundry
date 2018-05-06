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

        [Required]
        public bool Fault { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        [Required]
        public bool ToRenew { get; set; }

        public bool Confirmed
        {
            get { return !ToRenew; }
        }

        [Required]
        public int WashingMachineId { get; set; }

        public virtual WashingMachine WashingMachine { get; set; }

        public int? RoomId { get; set; }
        public virtual Room Room { get; set; }
    }
}