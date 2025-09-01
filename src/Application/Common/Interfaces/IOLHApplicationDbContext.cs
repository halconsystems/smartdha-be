using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DHAFacilitationAPIs.Application.Common.Interfaces;
public interface IOLHApplicationDbContext
{
    //Hydrent Related DbSets

    DbSet<OLH_BowserCapacity> BowserCapacitys { get; }

    DbSet<OLH_BowserCapacityRate> BowserCapacityRates { get; }

    DbSet<OLH_BowserDriverShift> OLH_BowserDriverShifts { get; }

    DbSet<OLH_BowserRequest> OLH_BowserRequests { get; }

    DbSet<OLH_DriverInfo> DriverInfos { get; }

    DbSet<OLH_DriverStatus> DriverStatuses { get; }

    DbSet<OLH_Shift> OLH_Shifts { get; }

    DbSet<OLH_Vehicle> OLH_Vehicles { get; }

    DbSet<OLH_VehicleOwner> OLH_VehicleOwners { get; }

    DbSet<OLH_VehicleStatus> OLH_VehicleStatuses { get; }

    DbSet<OLH_VehicleType> OLH_VehicleTypes { get; }

    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    DatabaseFacade Database { get; }
}
