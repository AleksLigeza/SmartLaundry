using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartLaundry.Models.LaundryViewModels
{
    public class DayViewModel
    {
        public List<Laundry> Laundries;
        public int DormitoryId;
        public Reservation currentRoomReservation;
        public DateTime date;
        public bool hasReservationToRenew;
        public Dictionary<int, Reservation> washingMachineState;

        public bool IsFaultAtTime(WashingMachine machine, DateTime time)
        {
            return isCurrentlyFault(machine.Id) && time >= washingMachineState[machine.Id].StartTime
                   || machine.Reservations.Where(x => x.StartTime <= time
                                                      && (x.EndTime == null || x.EndTime > time) && x.Fault == true)
                       .Any();
        }

        public bool isCurrentlyFault(int machineId)
        {
            return washingMachineState[machineId] != null && washingMachineState[machineId].EndTime == null;
        }
    }
}