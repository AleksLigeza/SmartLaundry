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
        public string Name { get; set; }
        public string Address { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }

        public virtual ApplicationUser Manager { get; set; }
        public virtual ICollection<ApplicationUser> Porters { get; set; }
    }
}
