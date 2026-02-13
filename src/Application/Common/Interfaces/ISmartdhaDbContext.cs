using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Entities.Smartdha;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DHAFacilitationAPIs.Application.Common.Interfaces;
public interface ISmartdhaDbContext
{
    DbSet<Vehicle> Vehicles { get; }
    DbSet<UserFamily> UserFamilies { get; }
    DbSet<ResidentProperty> ResidentProperties { get; set; }
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    DatabaseFacade Database { get; }
}
