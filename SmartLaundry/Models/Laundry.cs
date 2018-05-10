using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartLaundry.Models
{
    public class Laundry
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [Range(1, 10, ErrorMessage = "{0} must be a number between {1} and {2}")]
        [DefaultValue(1)]
        [Display(Name = "Position")]
        public int Position { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [DataType(DataType.Time)]
        [Display(Name = "Start time")]
        public TimeSpan startTime { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [DataType(DataType.Time)]
        [Display(Name = "Shift length")]
        public TimeSpan shiftTime { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [Range(1, 24, ErrorMessage = "{0} must be a number between {1} and {2}")]
        [DefaultValue(4)]
        [Display(Name = "Shift count")]
        public int shiftCount { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        public int DormitoryId { get; set; }

        public virtual Dormitory Dormitory { get; set; }

        public virtual ICollection<WashingMachine> WashingMachines { get; set; }
    }
}