using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartLaundry.Models.DormitoryViewModels
{
    public class AnnouncementsList
    {
        public AnnouncementsList()
        {
        }

        public AnnouncementsList(List<Announcement> list, Dormitory dormitory)
        {
            Dormitory = dormitory;
            Announcements = list;
            AnnouncementToAdd = new Announcement()
            {
                Dormitory = dormitory,
                DormitoryId = dormitory.DormitoryId
            };
        }

        public Announcement AnnouncementToAdd { get; set; }

        public List<Announcement> Announcements;
        public Dormitory Dormitory;
    }
}
