using System.Data.Entity;
using System.Linq;
using Winform4System.DataAccess.Entities.SpareParts;
using Winform4System.DataAccess.Configuration;
using Winform4System.Core.Security;

namespace Winform4System.DataAccess
{
    public sealed class SparePartDbContext : DbContext
    {
        static SparePartDbContext()
        {
            Database.SetInitializer<SparePartDbContext>(null);
        }

        public SparePartDbContext()
            : base(new ConnectionStringProvider().Get())
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }

        public DbSet<SparePartInspectionBatch> InspectionBatches { get; set; }
        public DbSet<SparePartInspectionItem> InspectionItems { get; set; }
        public DbSet<SparePartMachineMaterial> MachineMaterials { get; set; }
        public DbSet<SparePartMachine> Machines { get; set; }
        public DbSet<SparePartMaterialPhoto> MaterialPhotos { get; set; }
        public DbSet<SparePartMaterial> Materials { get; set; }
        public DbSet<SparePartPrice> Prices { get; set; }
        public DbSet<SparePartRecoveryEvidence> RecoveryEvidence { get; set; }
        public DbSet<SparePartRecoveryGuide> RecoveryGuides { get; set; }
        public DbSet<SparePartRecoveryTicket> RecoveryTickets { get; set; }
        public DbSet<SparePartStorage> Storages { get; set; }
        public DbSet<SparePartTransaction> Transactions { get; set; }
        public DbSet<SparePartUnit> Units { get; set; }
        public DbSet<SparePartUser> Users { get; set; }
        public DbSet<SparePartDepartment> Departments { get; set; }
        public DbSet<SparePartInspectionAttachment> InspectionAttachments { get; set; }

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
            modelBuilder.Entity<SparePartUnit>().ToTable("sparepart_Unit");
            modelBuilder.Entity<SparePartStorage>().ToTable("sparepart_Storage");
            modelBuilder.Entity<SparePartMaterial>().ToTable("sparepart_Material");
            modelBuilder.Entity<SparePartMachine>().ToTable("sparepart_Machine");
            modelBuilder.Entity<SparePartMachineMaterial>().ToTable("sparepart_MachineMaterial");
            modelBuilder.Entity<SparePartTransaction>().ToTable("sparepart_Transaction");
            modelBuilder.Entity<SparePartPrice>().ToTable("sparepart_Price");
            modelBuilder.Entity<SparePartMaterialPhoto>().ToTable("sparepart_MaterialPhoto");
            modelBuilder.Entity<SparePartInspectionBatch>().ToTable("sparepart_InspectionBatch");
            modelBuilder.Entity<SparePartInspectionItem>().ToTable("sparepart_InspectionItem");
            modelBuilder.Entity<SparePartRecoveryTicket>().ToTable("sparepart_RecoveryTicket");
            modelBuilder.Entity<SparePartRecoveryEvidence>().ToTable("sparepart_RecoveryEvidence");
            modelBuilder.Entity<SparePartRecoveryGuide>().ToTable("sparepart_RecoveryGuide");
            modelBuilder.Entity<SparePartInspectionAttachment>().ToTable("sparepart_InspectionAttachment");
            modelBuilder.Entity<SparePartUser>().HasKey(item => item.Id).ToTable("vw_sparepart_User");
            modelBuilder.Entity<SparePartDepartment>().HasKey(item => item.Id).ToTable("vw_sparepart_Department");

            modelBuilder.Entity<SparePartMaterial>()
                .HasOptional(item => item.ReplacementMaterial)
                .WithMany(item => item.ReplacedByMaterials)
                .HasForeignKey(item => item.ReplacementMaterialId)
                .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }
    }
}
