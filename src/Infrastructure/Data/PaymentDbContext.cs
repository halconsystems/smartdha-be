using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Common;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Entities.BillsPayment;
using Microsoft.EntityFrameworkCore;

namespace DHAFacilitationAPIs.Infrastructure.Data;
public class PaymentDbContext : DbContext, IPaymentDbContext
{
    private readonly IUser _loggedInUser;
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options, IUser loggedInUser) : base(options)
    {
        _loggedInUser = loggedInUser;
    }

    public DbSet<PayMerchant> PayMerchants => Set<PayMerchant>();
    public DbSet<PayMerchantRule> PayMerchantRules => Set<PayMerchantRule>();
    public DbSet<PayBill> PayBills => Set<PayBill>();
    public DbSet<PayTransaction> PayTransactions => Set<PayTransaction>();
    public DbSet<PaymentIpnLog> PaymentIpnLogs => Set<PaymentIpnLog>();
    public DbSet<PayLateFeePolicy> PayLateFeePolicies => Set<PayLateFeePolicy>();

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply configurations if you use IEntityTypeConfiguration<>
        modelBuilder.ApplyConfigurationsFromAssembly(
       typeof(PaymentDbContext).Assembly,
       t => t.Namespace!.Contains("Data.Payment"));

        modelBuilder.Entity<PayMerchant>()
            .HasIndex(x => x.Code)
            .IsUnique();

        modelBuilder.Entity<PayBill>()
            .HasIndex(x => new { x.SourceSystem, x.SourceVoucherId })
            .IsUnique();

        modelBuilder.Entity<PayTransaction>()
            .HasIndex(x => x.BasketId)
            .IsUnique();

        modelBuilder.Entity<PayTransaction>()
            .HasOne(x => x.PayBill)
            .WithMany(x => x.Transactions)
            .HasForeignKey(x => x.PayBillId);

        modelBuilder.Entity<PayMerchantRule>()
            .HasIndex(x => new
            {
                x.SourceSystem,
                x.EntityType,
                x.EntityId,
                x.CategoryCode,
                x.IsActive,
                x.Priority
            });


        base.OnModelCreating(modelBuilder);
    }
}

