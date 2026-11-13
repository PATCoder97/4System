using System.Data.Entity;
using Winform4System.DataAccess.Entities;

namespace Winform4System.DataAccess
{
    [DbConfigurationType(typeof(Winform4System.DataAccess.Configuration.PostgresConfiguration))]
    public sealed class Winform4SystemDbContext : DbContext
    {
        static Winform4SystemDbContext()
        {
            Database.SetInitializer<Winform4SystemDbContext>(null);
        }

        public Winform4SystemDbContext(string connectionString)
            : base(new Npgsql.NpgsqlConnection(connectionString), true)
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }

        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<EmployeeProfile> EmployeeProfiles { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<JobTitle> JobTitles { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<SecurityGroup> SecurityGroups { get; set; }
        public DbSet<GroupRole> GroupRoles { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<ApplicationFunction> ApplicationFunctions { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<SystemSetting> SystemSettings { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserGroup>().HasKey(item => new { item.UserId, item.GroupId });
            modelBuilder.Entity<GroupRole>().HasKey(item => new { item.GroupId, item.RoleId });
            modelBuilder.Entity<RolePermission>().HasKey(item => new { item.RoleId, item.PermissionId });
            modelBuilder.Properties<byte[]>().Where(p => p.Name == "RowVersion").Configure(p => p.HasColumnType("bytea"));
            modelBuilder.HasDefaultSchema("public");
            modelBuilder.Properties<string>().Where(p => p.Name.EndsWith("UserId") || p.Name == "EmployeeCode").Configure(p => p.HasColumnType("varchar").HasMaxLength(10));
            base.OnModelCreating(modelBuilder);
        }
    }
}
