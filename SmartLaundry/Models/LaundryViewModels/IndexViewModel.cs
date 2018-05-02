using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartLaundry.Models.LaundryViewModels
{
    public class IndexViewModel
    {
        public delegate bool IsFaultAtTimeToday(int machineId, int hour);
        public delegate bool IsCurrentlyFault(int machineId);

        public List<Laundry> Laundries;
        public int DormitoryId;
        public Reservation currentRoomReservation;
        public IsFaultAtTimeToday isFaultAtTimeToday;
        public IsCurrentlyFault isCurrentlyFault;
    }
}
