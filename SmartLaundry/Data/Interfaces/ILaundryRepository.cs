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
        Laundry GetLaundryById(int id);
        Laundry GetLaundryWithIncludedEntities(int laundryId);
        List<Laundry> GetDormitoryLaundriesWithEntitiesAtDay(int dormitoryId, DateTime date);
        void UpdateLaundry(Laundry laundry);
    }
}