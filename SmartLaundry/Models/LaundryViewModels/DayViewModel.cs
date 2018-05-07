using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartLaundry.Models.LaundryViewModels
{
    public class DayViewModel
    {
        public List<Laundry> Laundries { get; set; }
        public int DormitoryId { get; set; }
        public Reservation currentRoomReservation { get; set; }
        public DateTime date { get; set; }
        public bool hasReservationToRenew { get; set; }
        public Dictionary<int, Reservation> washingMachineState { get; set; }

        public string AddLaundryError { get; set; }
        public string AddWashingMachineError { get; set; }

        public Laundry LaundryToAdd { get; set; }
        public WashingMachine WashingMachineToAdd { get; set; }

        public AddLaundryViewModel AddLaundryViewModel { get; set; }

        public bool IsFaultAtTime(WashingMachine machine, DateTime time)
        {
            return isCurrentlyFault(machine.Id) && time >= washingMachineState[machine.Id].StartTime
                   || machine.Reservations
                       .Any(x => x.StartTime <= time && (x.EndTime == null || x.EndTime > time) && x.Fault);
        }

        public bool isCurrentlyFault(int machineId)
        {
            return washingMachineState[machineId] != null && washingMachineState[machineId].EndTime == null;
        }
    }
}