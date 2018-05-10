using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartLaundry.Models
{
    public class Announcement
    {
        [Key]
        public int Id { get; set; }

        [StringLength(255)]
        [Display(Name = "Message")]
        public string Message { get; set; }

        public DateTime PublishingDate { get; set; }

        public int DormitoryId { get; set; }

        public virtual Dormitory Dormitory { get; set; }
    }
}