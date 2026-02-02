using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Entities.BillsPayment;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DHAFacilitationAPIs.Application.Common.Interfaces;
public interface IPaymentDbContext
{
    DbSet<PayMerchant> PayMerchants { get; }
    DbSet<PayMerchantRule> PayMerchantRules { get; }
    DbSet<PayBill> PayBills { get; }
    DbSet<PayTransaction> PayTransactions { get; }
    DbSet<PaymentIpnLog> PaymentIpnLogs { get; }


    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    DatabaseFacade Database { get; }
}
