using System.Data.Entity;
using System.Linq;
using DataAccessLayer;
using Winform4System.DataAccess.Configuration;
using Winform4System.Core.Security;

namespace DataAccessLayer
{
    public sealed class DBDocumentManagementSystemEntities : DbContext
    {
        static DBDocumentManagementSystemEntities()
        {
            Database.SetInitializer<DBDocumentManagementSystemEntities>(null);
        }

        public DBDocumentManagementSystemEntities()
            : base(new ConnectionStringProvider().Get())
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }

        public DbSet<dt309_InspectionBatch> dt309_InspectionBatch { get; set; }
        public DbSet<dt309_InspectionBatchMaterial> dt309_InspectionBatchMaterial { get; set; }
        public DbSet<dt309_MachineMaterials> dt309_MachineMaterials { get; set; }
        public DbSet<dt309_Machines> dt309_Machines { get; set; }
        public DbSet<dt309_MaterialPhoto> dt309_MaterialPhoto { get; set; }
        public DbSet<dt309_Materials> dt309_Materials { get; set; }
        public DbSet<dt309_Prices> dt309_Prices { get; set; }
        public DbSet<dt309_RecoveryEvidence> dt309_RecoveryEvidence { get; set; }
        public DbSet<dt309_RecoveryGuides> dt309_RecoveryGuides { get; set; }
        public DbSet<dt309_RecoveryTickets> dt309_RecoveryTickets { get; set; }
        public DbSet<dt309_Storages> dt309_Storages { get; set; }
        public DbSet<dt309_Transactions> dt309_Transactions { get; set; }
        public DbSet<dt309_Units> dt309_Units { get; set; }
        public DbSet<dm_User> dm_User { get; set; }
        public DbSet<dm_Departments> dm_Departments { get; set; }
        public DbSet<dm_Group> dm_Group { get; set; }
        public DbSet<dm_GroupUser> dm_GroupUser { get; set; }
        public DbSet<dm_Attachment> dm_Attachment { get; set; }

        public override int SaveChanges()
        {
            bool hasAdded = ChangeTracker.Entries().Any(entry => entry.State == EntityState.Added);
            bool hasModified = ChangeTracker.Entries().Any(entry => entry.State == EntityState.Modified);
            bool hasDeleted = ChangeTracker.Entries().Any(entry => entry.State == EntityState.Deleted);

            if (hasAdded)
                CurrentAuthorization.Demand("ASSET.SPARE_PART.CREATE");
            if (hasModified)
                CurrentAuthorization.Demand("ASSET.SPARE_PART.UPDATE");
            if (hasDeleted)
                CurrentAuthorization.Demand("ASSET.SPARE_PART.DELETE");

            return base.SaveChanges();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<dm_User>().HasKey(item => item.Id).ToTable("vw_309_User");
            modelBuilder.Entity<dm_Departments>().HasKey(item => item.Id).ToTable("vw_309_Department");
            modelBuilder.Entity<dm_Group>().HasKey(item => item.Id).ToTable("vw_309_Group");
            modelBuilder.Entity<dm_GroupUser>().HasKey(item => item.Id).ToTable("vw_309_GroupUser");
            modelBuilder.Entity<dm_Attachment>().HasKey(item => item.Id).ToTable("dt309_Attachment");

            modelBuilder.Entity<dt309_Materials>()
                .HasOptional(item => item.dt309_Materials2)
                .WithMany(item => item.dt309_Materials1)
                .HasForeignKey(item => item.ReplacementMaterialId)
                .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }
    }
}
