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
        public IEnumerable<Dormitory> Dormitories
        {
            get
            {
                return new List<Dormitory>
                {
                    new Dormitory
                    {
                        Name = "Promień",
                        DormitoryID = 1,
                        Address = "Akademicka 1",
                        City = "Rzeszów",
                        ZipCode = "35-084"
                    },
                    new Dormitory
                    {
                        Name = "Pingwin",
                        DormitoryID = 2,
                        Address = "Akademicka 5",
                        City = "Rzeszów",
                        ZipCode = "35-084"
                    },
                    new Dormitory
                    {
                        Name = "Nestor",
                        DormitoryID = 3,
                        Address = "Akademicka 3",
                        City = "Rzeszów",
                        ZipCode = "35-084"
                    },
                };
            }
        }

        public Dormitory AddSingle(Dormitory source)
        {
            throw new NotImplementedException();
        }

        public Dormitory AssignManager(ApplicationUser user, Dormitory dormitory)
        {
            throw new NotImplementedException();
        }

        public void DeleteSingle(Dormitory source)
        {
            throw new NotImplementedException();
        }

        public bool DormitoryHasRoom(int roomNumber, int dormitoryId)
        {
            throw new NotImplementedException();
        }

        public List<Dormitory> GetAll()
        {
            throw new NotImplementedException();
        }

        public Dormitory GetDormitoryWithRooms(int id)
        {
            throw new NotImplementedException();
        }

        public Dormitory GetSingleById(int id)
        {
            throw new NotImplementedException();
        }

        public Dormitory GetSingleWithIncludes(int id)
        {
            throw new NotImplementedException();
        }

        public Dormitory UpdateSingle(Dormitory source)
        {
            throw new NotImplementedException();
        }
    }
}