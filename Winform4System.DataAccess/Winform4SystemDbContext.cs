using System.Data.Entity;
using Winform4System.DataAccess.Entities;

namespace Winform4System.DataAccess
{
    public sealed class Winform4SystemDbContext : DbContext
    {
        static Winform4SystemDbContext()
        {
            Database.SetInitializer<Winform4SystemDbContext>(null);
        }

        public Winform4SystemDbContext(string connectionString)
            : base(connectionString)
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }

        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<EmployeeProfile> EmployeeProfiles { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<SecurityGroup> SecurityGroups { get; set; }
        public DbSet<GroupRole> GroupRoles { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<ApplicationFunction> ApplicationFunctions { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserGroup>().HasKey(item => new { item.UserId, item.GroupId });
            modelBuilder.Entity<GroupRole>().HasKey(item => new { item.GroupId, item.RoleId });
            modelBuilder.Entity<RolePermission>().HasKey(item => new { item.RoleId, item.PermissionId });
            base.OnModelCreating(modelBuilder);
        }
    }
}
