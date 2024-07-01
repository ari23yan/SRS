using Microsoft.EntityFrameworkCore;
using SurgeryRoomScheduler.Domain.Entities.Account;
using SurgeryRoomScheduler.Domain.Entities.Common;
using SurgeryRoomScheduler.Domain.Entities.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SurgeryRoomScheduler.Data.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> dbContext) : base(dbContext){}
        public DbSet<User> Users  { get; set; }
        public DbSet<Otp> Otps  { get; set; }
        public DbSet<UserDetail> UserDetails { get; set; }
        public DbSet<Role> Roles  { get; set; }
        public DbSet<Permission> Permissions  { get; set; }
        public DbSet<RolePermission> RolePermissions  { get; set; }
        public DbSet<PermissionGroup> PermissionGroup { get; set; }
        public DbSet<PermissionGroup_Permission> permissionGroup_Permissions { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<RoleMenu> RoleMenus { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<ReservationConfirmation> ReservationConfirmations { get; set; }
        public DbSet<ReservationConfirmationLog> ReservationConfirmationLogs { get; set; }
        public DbSet<ReservationConfirmationType> ReservationConfirmationTypes { get; set; }
        public DbSet<ReservationRejectionAndCancellationReason> ReservationRejectionAndCancellationReasons { get; set; }
        public DbSet<ReservationRejection> ReservationRejections { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Timing> Timings { get; set; }
        public DbSet<JobLog> JobLogs { get; set; }
        public DbSet<SurgeryName> SurgeryNames { get; set; }
        public DbSet<Insurance> Insurances { get; set; }
        public DbSet<DoctorRoom> DoctorRooms { get; set; }
        public DbSet<UsersAccessLog> UsersAccessLogs  { get; set; }
        public DbSet<ApplicationLog> ApplicationLogs  { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.ReservationConfirmation)
                .WithOne(rc => rc.Reservation)
                .HasForeignKey<ReservationConfirmation>(rc => rc.ReservationId);

            base.OnModelCreating(modelBuilder);
        }

    }
}
