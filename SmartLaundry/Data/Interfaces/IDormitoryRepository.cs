using SmartLaundry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartLaundry.Data.Interfaces
{
    public interface IDormitoryRepository
    {
        IEnumerable<Dormitory> Dormitories { get; }

        List<Dormitory> GetAll();
        Dormitory GetSingleById(int id);
        Dormitory AddSingle(Dormitory source);
        Dormitory UpdateSingle(Dormitory source);
        void DeleteSingle(Dormitory source);
        Dormitory AssignManager(ApplicationUser user, Dormitory dormitory);
        Dormitory GetSingleWithIncludes(int id);
        Dormitory GetDormitoryWithRooms(int id);
        bool DormitoryHasRoom(int roomNumber, int dormitoryId);
    }
}