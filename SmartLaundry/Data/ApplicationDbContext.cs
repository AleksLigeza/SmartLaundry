using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartLaundry.Models;

namespace SmartLaundry.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Dormitory> Dormitories { get; set; }
        public DbSet<Laundry> Laundries { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<WashingMachine> WashingMachines { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Announcement> Announcements { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>()
                .HasOne(x => x.DormitoryManager)
                .WithOne(x => x.Manager)
                .HasForeignKey<ApplicationUser>(x => x.DormitoryManagerId);

            builder.Entity<ApplicationUser>()
                .HasOne(u => u.DormitoryPorter)
                .WithMany(d => d.Porters)
                .HasForeignKey(u => u.DormitoryPorterId);

            builder.Entity<ApplicationUser>()
                .HasOne(x => x.Room)
                .WithMany(x => x.Occupants)
                .HasForeignKey(x => x.RoomId);

            builder.Entity<Room>()
                .HasOne(x => x.Dormitory)
                .WithMany(x => x.Rooms)
                .HasForeignKey(x => x.DormitoryId);

            builder.Entity<Reservation>()
                .HasOne(x => x.Room)
                .WithMany(x => x.Reservations)
                .HasForeignKey(x => x.RoomId);

            builder.Entity<Reservation>()
                .HasOne(x => x.WashingMachine)
                .WithMany(x => x.Reservations)
                .HasForeignKey(x => x.WashingMachineId);
    
            builder.Entity<WashingMachine>()
                .HasOne(x => x.Laundry)
                .WithMany(x => x.WashingMachines)
                .HasForeignKey(x => x.LaundryId);

            builder.Entity<Laundry>()
                .HasOne(x => x.Dormitory)
                .WithMany(x => x.Laundries)
                .HasForeignKey(x => x.DormitoryId);

            builder.Entity<Announcement>()
                .HasOne(x => x.Dormitory)
                .WithMany(x => x.Announcements)
                .HasForeignKey(x => x.DormitoryId);
        }
    }
}
