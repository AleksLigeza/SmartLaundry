using SmartLaundry.Data.Interfaces;
using SmartLaundry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartLaundry.Data.Mock
{
    public class MockDormitoryRepo : IDormitoryRepository
    {
        private List<Dormitory> _dormitories;
        public IEnumerable<Dormitory> Dormitories
        {
            get
            {
                if (_dormitories != null) return _dormitories;
                return _dormitories = new List<Dormitory>
                {
                    new Dormitory
                    {
                        Name = "Promień",
                        DormitoryId = 1,
                        Address = "Akademicka 1",
                        City = "Rzeszów",
                        ZipCode = "35-084",
                        Porters = new List<ApplicationUser>(),
                        Rooms = new List<Room>()
                        {
                            new Room()
                            {
                                Id = 1,
                                Number = 1
                            }
                        }
                    },
                    new Dormitory
                    {
                        Name = "Pingwin",
                        DormitoryId = 2,
                        Address = "Akademicka 5",
                        City = "Rzeszów",
                        ZipCode = "35-084",
                        Porters = new List<ApplicationUser>()
                    },
                    new Dormitory
                    {
                        Name = "Nestor",
                        DormitoryId = 3,
                        Address = "Akademicka 3",
                        City = "Rzeszów",
                        ZipCode = "35-084",
                        Porters = new List<ApplicationUser>()
                    },
                };
            }
        }

        public Dormitory AddSingle(Dormitory source)
        {
            var maxId = Dormitories.Max(x => x.DormitoryId);
            source.DormitoryId = maxId + 1;

            _dormitories.Add(source);

            return source;
        }

        public Dormitory AssignManager(ApplicationUser user, Dormitory dormitory)
        {
            dormitory.Manager = user;
            user.DormitoryManager = dormitory;
            user.DormitoryManagerId = dormitory.DormitoryId;
            return dormitory;
        }

        public void DeleteSingle(Dormitory source)
        {
            _dormitories.Remove(source);
            //((IDisposable)source).Dispose();
        }

        public bool IsRoomInDormitory(int roomNumber, int dormitoryId)
        {
            var dormitory = Dormitories.SingleOrDefault(x => x.DormitoryId == dormitoryId);
            if (dormitory == null) return false;
            return dormitory.Rooms.Any(x => x.Number == roomNumber);
        }

        public List<Dormitory> GetAll()
        {
            return Dormitories.ToList();
        }

        public Dormitory GetSingleById(int id)
        {
            return Dormitories.SingleOrDefault(x => x.DormitoryId == id);
        }

        public Dormitory UpdateSingle(Dormitory source)
        {
            var dormitory = Dormitories.SingleOrDefault(x => x.DormitoryId == source.DormitoryId);
            if (dormitory == null) return null;
            _dormitories.Remove(dormitory);
            _dormitories.Add(source);
            return source;
        }
    }
}