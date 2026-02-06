using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Common;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Entities.LMS;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DHAFacilitationAPIs.Infrastructure.Data;

public class LaundrySystemDbContext : DbContext ,  ILaundrySystemDbContext
{
    private readonly IUser _loggedInUser;

    public LaundrySystemDbContext(DbContextOptions<LaundrySystemDbContext> options, IUser loggedInUser) : base(options)
    {
        _loggedInUser = loggedInUser;
    }

    public DbSet<LaundryService> LaundryServices => Set<LaundryService>();
    public DbSet<LaundryPackaging> LaundryPackagings => Set<LaundryPackaging>();
    public DbSet<LaundryCategory> LaundryCategories => Set<LaundryCategory>();
    public DbSet<LaundryItems> LaundryItems => Set<LaundryItems>();
    public DbSet<Shops> Shops => Set<Shops>();
    public DbSet<Orders> Orders => Set<Orders>();
    public DbSet<OrderSummary> OrderSummaries => Set<OrderSummary>();
    public DbSet<PaymentDTSetting> PaymentDTSettings => Set<PaymentDTSetting>();
    public DbSet<DeliveryDetails> DeliveryDetails => Set<DeliveryDetails>();
    public DbSet<OrderDTSetting> OrderDTSettings => Set<OrderDTSetting>();
    public DbSet<ConfirmedOrder> ConfirmedOrders => Set<ConfirmedOrder>();
    public DbSet<OrderPaymentIpnLogs> OrderPaymentIpnLogs => Set<OrderPaymentIpnLogs>();
    public DbSet<ShopVehicles> ShopVehicles => Set<ShopVehicles>();
    public DbSet<ShopDrivers> ShopDrivers => Set<ShopDrivers>();
    public DbSet<OrderDispatch> OrderDispatches => Set<OrderDispatch>();
    public DbSet<ShopDTSetting> ShopDTSettings => Set<ShopDTSetting>();
    public DbSet<ShopVehicleAssignmentHistory> ShopVehicleAssignmentHistories => Set<ShopVehicleAssignmentHistory>();

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
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LaundrySystemDbContext).Assembly);

        // Global filter: exclude soft deleted records
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property<int>("Ser")
                    .ValueGeneratedOnAdd()
                    .HasColumnOrder(1);

                modelBuilder.Entity(entityType.ClrType)
                    .Property<Guid>("Id")
                    .HasColumnOrder(2);
            }
        }


        // Directorate unique code
        modelBuilder.Entity<LaundryService>()
            .HasIndex(x => x.Code).IsUnique();

        // Category unique code
        modelBuilder.Entity<LaundryPackaging>()
            .HasIndex(x => x.Code).IsUnique();

        // Process unique code per category
        modelBuilder.Entity<LaundryCategory>()
            .HasIndex(x => x.Code).IsUnique();

        // Steps unique StepNo per process
        modelBuilder.Entity<LaundryItems>()
            .HasIndex(x => x.Code).IsUnique();

        // PrerequisiteDefinition unique Code
        modelBuilder.Entity<Shops>()
            .HasIndex(x => x.Code).IsUnique();

        // ProcessPrerequisite unique per process+prerequisite
        modelBuilder.Entity<Orders>()
            .HasIndex(x => new { x.UniqueFormID, x.Id }).IsUnique();

        // Case unique CaseNo
        modelBuilder.Entity<OrderSummary>()
            .HasIndex(x => x.Id).IsUnique();

        // CasePrerequisiteValue unique per case+prerequisite
        modelBuilder.Entity<PaymentDTSetting>()
            .HasIndex(x =>  x.Id).IsUnique();

        // Voucher one active voucher per case (optional)
        modelBuilder.Entity<DeliveryDetails>()
            .HasIndex(x => x.Id);

        base.OnModelCreating(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }
}
