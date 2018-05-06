using SmartLaundry.Data.Interfaces;
using SmartLaundry.Models;
using System.Collections.Generic;
using System.Linq;

namespace SmartLaundry.Data.Repositories
{
    public class AnnouncementRepository : IAnnouncementRepository
    {
        private readonly ApplicationDbContext _context;

        public AnnouncementRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<Announcement> Announcements => _context.Announcements;

        public void CreateAnnouncement(Announcement announcement)
        {
            _context.Announcements.Add(announcement);
            _context.SaveChanges();
        }

        public void DeleteAnnouncement(int announcementId)
        {
            var announcement = _context.Announcements.Single(x => x.Id == announcementId);
            _context.Announcements.Remove(announcement);
            _context.SaveChanges();
        }

        public List<Announcement> GetDormitoryAnnouncements(int dormitoryId)
        {
            return _context.Announcements.Where(x => x.DormitoryId == dormitoryId).ToList();
        }

        public Announcement GetAnnouncementById(int id)
        {
            return _context.Announcements.Find(id);
        }
    }
}