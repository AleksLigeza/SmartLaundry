using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartLaundry.Models
{
    public class Laundry
    {
        [Key]
        public int Id { get; set; }

        [Range(1, 10)]
        public int Position { get; set; }

        public int DormitoryId { get; set; }
        public virtual Dormitory Dormitory { get; set; }

        public virtual ICollection<WashingMachine> WashingMachines { get; set; }
    }
}
