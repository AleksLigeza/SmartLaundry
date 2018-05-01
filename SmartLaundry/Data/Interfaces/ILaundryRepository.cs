using SmartLaundry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartLaundry.Data.Interfaces
{
    public interface ILaundryRepository
    {
        IQueryable<Laundry> Laundries { get; }

        Laundry AddLaundry(int dormitoryId, int position);
        void RemoveLaundry(Laundry laundry);
        WashingMachine AddWashingMachine(int laundryId, int position);
        void RemoveWashingMachine(WashingMachine washingMachine);
        Laundry GetLaundryWithIncludedEntities(int laundryId);
        List<Laundry> GetDormitoryLaundriesWithEntities(int dormitoryId);
    }
}
