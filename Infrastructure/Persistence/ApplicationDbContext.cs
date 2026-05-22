using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<MasterIndustrial> MasterIndustrials { get; set; }

        public DbSet<DemandPlan> DemandPlans { get; set; }

        public DbSet<DowntimeCode> DowntimeCodes { get; set; }

        public DbSet<HourlyProductionRecord> HourlyProductionRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MasterIndustrial>(entity =>
            {
                entity.ToTable("MasterIndustrial");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.ParentPartNumber).HasColumnName("parentPartNumber").IsRequired(false);
                entity.Property(e => e.ChildPartNumber).HasColumnName("childPartNumber").IsRequired(false);
                entity.Property(e => e.ExternalDiameter).HasColumnName("externalDiameter").IsRequired(false);
                entity.Property(e => e.WallThickness).HasColumnName("wallThickness").IsRequired(false);
                entity.Property(e => e.Development).HasColumnName("development").IsRequired(false);
                entity.Property(e => e.Description).HasColumnName("description").IsRequired(false);
                entity.Property(e => e.Type).HasColumnName("type").IsRequired(false);
                entity.Property(e => e.Family).HasColumnName("family").IsRequired(false);
                entity.Property(e => e.Client).HasColumnName("client").IsRequired(false);
                entity.Property(e => e.Line).HasColumnName("line").IsRequired(false);
                entity.Property(e => e.PartOfPurchase).HasColumnName("partOfPurchase").IsRequired(false);
                entity.Property(e => e.QuantityXQuantity).HasColumnName("quantityXQuantity").IsRequired(false);
                entity.Property(e => e.Operation).HasColumnName("operation").IsRequired(false);
                entity.Property(e => e.Sequence).HasColumnName("sequence").IsRequired(false);
                entity.Property(e => e.ProcessComments).HasColumnName("processComments").IsRequired(false);
                entity.Property(e => e.MajorSetup).HasColumnName("majorSetup").IsRequired(false);
                entity.Property(e => e.MinorSetup).HasColumnName("minorSetup").IsRequired(false);
                entity.Property(e => e.OperSetup).HasColumnName("operSetup").HasColumnType("decimal(10,3)").IsRequired(false);
                entity.Property(e => e.TCiclo).HasColumnName("tCiclo").HasColumnType("decimal(10,3)").IsRequired(false);
                entity.Property(e => e.Oper).HasColumnName("oper").HasColumnType("decimal(10,3)").IsRequired(false);
                entity.Property(e => e.PzsHr).HasColumnName("pzsHr").IsRequired(false);
                entity.Property(e => e.Verification).HasColumnName("verification").IsRequired(false);
            });

            modelBuilder.Entity<DemandPlan>(entity =>
            {
                entity.ToTable("DemandPlans");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PartNumber).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Quantity).IsRequired();
                entity.Property(e => e.ProductionDate).IsRequired();
                entity.Property(e => e.UploadDate).HasDefaultValueSql("GETDATE()");
            });

            modelBuilder.Entity<DowntimeCode>(entity =>
            {
                entity.ToTable("DowntimeCodes");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Code).IsUnique();
                entity.Property(e => e.Area).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Type).HasMaxLength(5).IsRequired();
                entity.Property(e => e.Code).HasMaxLength(10).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(255).IsRequired();
            });

            modelBuilder.Entity<HourlyProductionRecord>(entity =>
            {
                entity.ToTable("HourlyProductionRecords");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Date).IsRequired();
                entity.Property(e => e.Shift).IsRequired();
                entity.Property(e => e.Line).IsRequired();
                entity.Property(e => e.Hour).IsRequired();
                entity.Property(e => e.PartNumber).HasMaxLength(100).IsRequired();
                entity.Property(e => e.ActualPieces).IsRequired();
                entity.Property(e => e.EmployeeNumber).HasMaxLength(50).IsRequired();

                entity.HasOne(d => d.DowntimeCode)
                      .WithMany(p => p.HourlyProductionRecords)
                      .HasForeignKey(d => d.DowntimeCodeId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}