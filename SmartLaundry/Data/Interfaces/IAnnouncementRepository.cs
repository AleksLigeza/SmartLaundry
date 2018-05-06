using SmartLaundry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartLaundry.Data.Interfaces
{
    public interface IAnnouncementRepository
    {
        IQueryable<Announcement> Announcements { get; }

        List<Announcement> GetDormitoryAnnouncements(int dormitoryId);
        void CreateAnnouncement(Announcement announcement);
        void DeleteAnnouncement(int announcementId);
        Announcement GetAnnouncementById(int id);
    }
}