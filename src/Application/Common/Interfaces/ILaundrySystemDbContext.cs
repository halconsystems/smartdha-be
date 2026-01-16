using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Entities.LMS;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DHAFacilitationAPIs.Application.Common.Interfaces;

public interface ILaundrySystemDbContext
{
    DbSet<LaundryService> LaundryServices { get; }
    DbSet<LaundryPackaging> LaundryPackagings { get; }
    DbSet<LaundryCategory> LaundryCategories { get; }
    DbSet<LaundryItems> LaundryItems { get; }
    DbSet<Shops> Shops { get; }
    DbSet<Orders> Orders { get; }
    DbSet<OrderSummary> OrderSummaries { get; }
    DbSet<PaymentDTSetting> PaymentDTSettings { get; }
    DbSet<DeliveryDetails> DeliveryDetails { get; }
    DbSet<OrderDTSetting> OrderDTSettings { get; }
    DbSet<ConfirmedOrder> ConfirmedOrders { get; }
    DbSet<OrderPaymentIpnLogs> OrderPaymentIpnLogs { get; }

    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    DatabaseFacade Database { get; }
}
