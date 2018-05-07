using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartLaundry.Models.DormitoryViewModels
{
    public class RoomsViewModel
    {
        [Required]
        public Dormitory Dormitory { get; set; }

        public Room RoomToAdd { get; set; }
        public string ErrorMessage { get; set; }
    }
}
