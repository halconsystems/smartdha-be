using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Common;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Entities.PMS;
using DHAFacilitationAPIs.Domain.Enums.PMS;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DHAFacilitationAPIs.Infrastructure.Data;
public class PMSApplicationDbContext : DbContext, IPMSApplicationDbContext
{
    private readonly IUser _loggedInUser;
    public PMSApplicationDbContext(DbContextOptions<PMSApplicationDbContext> options, IUser loggedInUser) : base(options)
    {
        _loggedInUser = loggedInUser;
    }
    public DbSet<Directorate> Directorates => Set<Directorate>();
    public DbSet<ServiceCategory> ServiceCategories => Set<ServiceCategory>();
    public DbSet<ServiceProcess> ServiceProcesses => Set<ServiceProcess>();
    public DbSet<ProcessStep> ProcessSteps => Set<ProcessStep>();
    public DbSet<PrerequisiteDefinition> PrerequisiteDefinitions => Set<PrerequisiteDefinition>();
    public DbSet<ProcessPrerequisite> ProcessPrerequisites => Set<ProcessPrerequisite>();
    public DbSet<UserProperty> Properties => Set<UserProperty>();
    public DbSet<PropertyCase> PropertyCases => Set<PropertyCase>();
    public DbSet<CaseStepHistory> CaseStepHistories => Set<CaseStepHistory>();
    public DbSet<CasePrerequisiteValue> CasePrerequisiteValues => Set<CasePrerequisiteValue>();
    public DbSet<CaseDocument> CaseDocuments => Set<CaseDocument>();
    public DbSet<CaseVoucher> CaseVouchers => Set<CaseVoucher>();
    public DbSet<CasePayment> CasePayments => Set<CasePayment>();
    public DbSet<FeeDefinition> FeeDefinitions => Set<FeeDefinition>();
    public DbSet<FeeSlab> FeeSlabs => Set<FeeSlab>();
    public DbSet<CaseFee> CaseFees => Set<CaseFee>();
    public DbSet<PrerequisiteOption> PrerequisiteOptions => Set<PrerequisiteOption>();
    public DbSet<FeeOption> FeeOptions => Set<FeeOption>();
    public DbSet<FeeCategory> FeeCategories => Set<FeeCategory>();
    public DbSet<CaseFeeReceipt> CaseFeeReceipts => Set<CaseFeeReceipt>();
    public DbSet<FeeSetting> FeeSettings => Set<FeeSetting>();
    public DbSet<CaseResultDocument> CaseResultDocuments => Set<CaseResultDocument>();
    public DbSet<CaseRejectRequirement> CaseRejectRequirements => Set<CaseRejectRequirement>();
    public DbSet<MessageTemplate> MessageTemplates => Set<MessageTemplate>();
    public DbSet<CaseMessageLog> CaseMessageLogs => Set<CaseMessageLog>();
    public DbSet<NumberSequence> NumberSequences => Set<NumberSequence>();
    public DbSet<ProcessStepAudit> ProcessStepAudits => Set<ProcessStepAudit>();


    /* =========================
       SaveChanges – Auditing
       ========================= */

    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<BaseAuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.Created = now;
                entry.Entity.CreatedBy = _loggedInUser.Id;
                entry.Entity.IsActive = true;
                entry.Entity.IsDeleted = false;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.LastModified = now;
                entry.Entity.LastModifiedBy = _loggedInUser.Id;
            }

            // Soft Delete support
            if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entry.Entity.IsDeleted = true;
                entry.Entity.IsActive = false;
                entry.Entity.LastModified = now;
                entry.Entity.LastModifiedBy = _loggedInUser.Id;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    /* =========================
       Model Configuration
       ========================= */

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply configurations if you use IEntityTypeConfiguration<>
        modelBuilder.ApplyConfigurationsFromAssembly(
        typeof(CBMSApplicationDbContext).Assembly,
        t => t.Namespace!.Contains("Data.PMS")
    );

        // Global filter: exclude soft deleted records
        //foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        //{
        //    if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
        //    {
        //        modelBuilder.Entity(entityType.ClrType)
        //            .Property<int>("Ser")
        //            .ValueGeneratedOnAdd()
        //            .HasColumnOrder(1);

        //        modelBuilder.Entity(entityType.ClrType)
        //            .Property<Guid>("Id")
        //            .HasColumnOrder(2);
        //    }
        //}


        // Directorate unique code
        modelBuilder.Entity<Directorate>()
            .HasIndex(x => x.Code).IsUnique();

        // Category unique code
        modelBuilder.Entity<ServiceCategory>()
            .HasIndex(x => x.Code).IsUnique();

        // Process unique code per category
        modelBuilder.Entity<ServiceProcess>()
            .HasIndex(x => new { x.CategoryId, x.Code }).IsUnique();

        // Steps unique StepNo per process
        modelBuilder.Entity<ProcessStep>()
            .HasIndex(x => new { x.ProcessId, x.StepNo }).IsUnique();

        // PrerequisiteDefinition unique Code
        modelBuilder.Entity<PrerequisiteDefinition>()
            .HasIndex(x => x.Code).IsUnique();

        // ProcessPrerequisite unique per process+prerequisite
        modelBuilder.Entity<ProcessPrerequisite>()
            .HasIndex(x => new { x.ProcessId, x.PrerequisiteDefinitionId }).IsUnique();

        // Case unique CaseNo
        modelBuilder.Entity<PropertyCase>()
            .HasIndex(x => x.CaseNo).IsUnique();

        // CasePrerequisiteValue unique per case+prerequisite
        modelBuilder.Entity<CasePrerequisiteValue>()
            .HasIndex(x => new { x.CaseId, x.PrerequisiteDefinitionId }).IsUnique();

        // Voucher one active voucher per case (optional)
        modelBuilder.Entity<CaseVoucher>()
            .HasIndex(x => x.CaseId);

        modelBuilder.Entity<FeeDefinition>()
    .HasIndex(x => x.ProcessId);

        modelBuilder.Entity<FeeSlab>()
            .HasIndex(x => new { x.FeeDefinitionId, x.FromArea, x.ToArea });

        modelBuilder.Entity<CaseFee>()
            .HasIndex(x => x.CaseId)
            .IsUnique(); // one fee snapshot per case

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseAuditableEntity).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(ApplicationDbContext)
                    .GetMethod(nameof(ApplyGlobalFilters),
                        BindingFlags.NonPublic | BindingFlags.Static)
                    ?.MakeGenericMethod(entityType.ClrType);

                method?.Invoke(null, new object[] { modelBuilder });
            }
        }



        modelBuilder.Entity<NumberSequence>(entity =>
        {
            entity.ToTable("NumberSequences");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Prefix)
                .IsRequired()
                .HasMaxLength(10);

            entity.Property(x => x.SequenceDate)
                .IsRequired();

            entity.Property(x => x.LastNumber)
                .IsRequired();

            entity.HasIndex(x => new { x.Prefix, x.SequenceDate })
                .IsUnique();
        });

       


        base.OnModelCreating(modelBuilder);
    }
    private static void ApplyGlobalFilters<TEntity>(ModelBuilder modelBuilder)
    where TEntity : BaseAuditableEntity
    {
        modelBuilder.Entity<TEntity>()
            .HasQueryFilter(e => e.IsActive==true && !e.IsDeleted !=true);
    }
}
