using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartLaundry.Data.Interfaces;
using SmartLaundry.Models;

namespace SmartLaundry.Data.Mock
{
    public class MockRoomRepo : IRoomRepository
    {
        private List<Room> _rooms;

        public IEnumerable<Room> Rooms
        {
            get
            {
                if(_rooms != null) return _rooms;
                return _rooms = new List<Room>()
                {
                    new Room()
                    {
                        Id = 1,
                        Number = 1,
                        DormitoryId = 1,
                        Occupants = new List<ApplicationUser>(),
                        Dormitory = new Dormitory()
                        {
                            Rooms = new List<Room>()
                            {
                                new Room()
                                {
                                    Number = 1,
                                    Id = 1
                                }
                            }
                        }
                    },
                    new Room()
                    {
                        Id = 2,
                        Number = 2,
                        DormitoryId = 1,
                        Occupants = new List<ApplicationUser>()
                    },
                    new Room()
                    {
                        Id = 3,
                        Number = 1,
                        DormitoryId = 2,
                        Occupants = new List<ApplicationUser>()
                    }
                };
            }
        }

        public Room AddRoomToDormitory(int roomNumber, int dormitoryId)
        {
            var room = new Room()
            {
                Number = roomNumber,
                DormitoryId = dormitoryId
            };
            ((List<Room>)Rooms).Add(room);
            return room;
        }

        public int? DeleteRoom(int id)
        {
            var room = _rooms.Find(x=>x.Id == id);
            var roomDormId = room.DormitoryId;
            ((List<Room>)Rooms).Remove(room);

            return roomDormId;
        }

        public void AssignOccupant(Room room, ApplicationUser user)
        {
            room.Occupants.Add(user);
            user.RoomId = room.Id;
            user.Room = room;
        }

        public void RemoveOccupant(Room room, ApplicationUser user)
        {
            room.Occupants.Remove(user);
            user.RoomId = null;
            user.Room = null;
        }

        public Room GetRoomById(int id)
        {
            return Rooms.SingleOrDefault(x => x.Id == id);
        }
    }
}
