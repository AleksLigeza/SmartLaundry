﻿using System;
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

        public bool Fault { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public int? WashingMachineId { get; set; }
        public virtual WashingMachine WashingMachine { get; set; }

        public int? RoomId { get; set; }
        public virtual Room Room { get; set; }
    }
}