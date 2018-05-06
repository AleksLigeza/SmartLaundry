using SmartLaundry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartLaundry.Data.Interfaces
{
    public interface IWashingMachineRepository
    {
        IQueryable<WashingMachine> WashingMachines { get; }
        WashingMachine AddWashingMachine(int laundryId, int position);
        void RemoveWashingMachine(WashingMachine washingMachine);
        WashingMachine GetWashingMachineById(int id);
        WashingMachine EnableWashingMachine(int id);
        WashingMachine DisableWashingMachine(int id);
    }
}