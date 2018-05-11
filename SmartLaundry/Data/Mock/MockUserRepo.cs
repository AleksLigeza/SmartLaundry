using SmartLaundry.Data.Interfaces;
using SmartLaundry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartLaundry.Data.Mock
{
    public class MockUserRepo : IUserRepository
    {
        private List<ApplicationUser> _users;

        public IEnumerable<ApplicationUser> Users
        {
            get
            {
                if(_users != null)
                    return _users;
                return _users = new List<ApplicationUser>()
                {
                    new ApplicationUser()
                    {
                        Email = "abc123@abc.pl",
                        UserName = "abc123@abc.pl",
                        NormalizedEmail = "ABC123@ABC.PL",
                        Firstname = "Jan",
                        Lastname = "Kowalski",
                        Id = "a"
                    },
                    new ApplicationUser()
                    {
                        Email = "abc12345@abc.pl",
                        UserName = "abc12345@abc.pl",
                        NormalizedEmail = "ABC12345@ABC.PL",
                        Firstname = "Piotr",
                        Lastname = "Nowak",
                        Id = "b",
                        RoomId = 1
                    },
                    new ApplicationUser()
                    {
                        Email = "abcddddd123@abc.pl",
                        UserName = "abcddddd123@abc.pl",
                        NormalizedEmail = "ABCDDDDD@ABC.PL",
                        Firstname = "Janek",
                        Lastname = "Kowalski",
                        Id = "c",
                        DormitoryPorterId = 1
                    },
                    new ApplicationUser()
                    {
                        Email = "abccc123@abc.pl",
                        UserName = "abccc123@abc.pl",
                        NormalizedEmail = "ABCCC123@ABC.PL",
                        Firstname = "Aaaa",
                        Lastname = "Bbb",
                        Id = "d",
                        DormitoryManagerId = 1
                    },
                    new ApplicationUser()
                    {
                        Email = "admin@admin.admin",
                        UserName = "admin@admin.admin",
                        NormalizedEmail = "ADMIN@ADMIN.ADMIN",
                        Firstname = "Admin",
                        Lastname = "Admin",
                        Id = "e",
                    }
                };
            }
        }

        public List<ApplicationUser> GetAll()
        {
            return _users;
        }

        public ApplicationUser GetUserByEmail(string email)
        {
            return _users.SingleOrDefault(x => x.Email == email);
        }

        public List<ApplicationUser> GetUsersWithEmailLike(string email)
        {
            return _users.Where(x => x.Email.Contains(email)).ToList();
        }

        public Dormitory AssignDormitoryAsPorter(ApplicationUser user, Dormitory dormitory)
        {
            user.DormitoryPorter = dormitory;
            user.DormitoryPorterId = dormitory.DormitoryId;
            dormitory.Porters?.Add(user);
            return dormitory;
        }

        public void RemoveDormitoryPorter(ApplicationUser user, Dormitory dormitory)
        {
            dormitory.Porters.Remove(user);
            user.DormitoryPorter = null;
            user.DormitoryPorterId = null;
        }

        public List<ApplicationUser> FindDormitoryPorters(int dormitoryId)
        {
            return Users.Where(x => x.DormitoryPorterId == dormitoryId).ToList();
        }

        public ApplicationUser GetUserById(string id)
        {
            return Users.SingleOrDefault(x => x.Id == id);
        }
    }
}