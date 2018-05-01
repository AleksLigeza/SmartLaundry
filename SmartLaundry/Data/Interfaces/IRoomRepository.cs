using SmartLaundry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartLaundry.Data.Interfaces
{
    public interface IRoomRepository
    {
        IEnumerable<Room> Rooms { get; }

        Room AddRoomToDormitory(int roomNumber, int dormitoryId);
        int? DeleteRoom(int id);
        void AssignOccupant(Room room, ApplicationUser user);
        void RemoveOccupant(Room room, ApplicationUser user);
        Room GetRoomById(int id);
        Room GetRoomWithOccupants(int id);
    }
}
