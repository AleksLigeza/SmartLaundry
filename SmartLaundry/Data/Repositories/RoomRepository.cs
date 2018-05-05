using SmartLaundry.Data.Interfaces;
using SmartLaundry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace SmartLaundry.Data.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly ApplicationDbContext _context;
        public RoomRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Room> Rooms => _context.Rooms;

        public Room AddRoomToDormitory(int roomNumber, int dormitoryId)
        {
            var room = new Room()
            {
                DormitoryId = dormitoryId,
                Number = roomNumber
            };
            _context.Rooms.Add(room);
            _context.SaveChanges();
            return room;
        }

        public int? DeleteRoom(int id)
        {
            var room = _context.Rooms.SingleOrDefault(x => x.Id == id);

            if (room == null)
            {
                return null;
            }
            var dormitoryId = room.DormitoryId;

            _context.Rooms.Remove(room);
            _context.SaveChanges();

            return dormitoryId;
        }

        public void AssignOccupant(Room room, ApplicationUser user)
        {
            user.RoomId = room.Id;
            _context.SaveChanges();
        }

        public void RemoveOccupant(Room room, ApplicationUser user)
        {
            user.RoomId = null;
            _context.SaveChanges();
        }

        public Room GetRoomById(int id) => _context.Rooms.Where(x => x.Id == id).Include(x=>x.Dormitory).SingleOrDefault();
        public Room GetRoomWithOccupants(int id) => _context.Rooms.Where(x => x.Id == id).Include(x => x.Occupants).Include(x=>x.Dormitory).SingleOrDefault();
        
    }
}
