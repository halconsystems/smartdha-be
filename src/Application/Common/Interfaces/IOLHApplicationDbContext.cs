using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Entities.CBMS;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DHAFacilitationAPIs.Application.Common.Interfaces;
public interface IOLHApplicationDbContext
{
    //Hydrent Related DbSets

    DbSet<OLH_BowserCapacity> BowserCapacitys { get; }

    DbSet<OLH_BowserCapacityRate> BowserCapacityRates { get; }

    DbSet<OLH_BowserDriverShift> BowserDriverShifts { get; }

    DbSet<OLH_BowserRequest> BowserRequests { get; }

    DbSet<OLH_DriverInfo> DriverInfos { get; }

    DbSet<OLH_DriverStatus> DriverStatuses { get; }

    DbSet<OLH_Shift> Shifts { get; }

    DbSet<OLH_Vehicle> Vehicles { get; }

    public DbSet<OLH_VehicleMake> VehicleMakes { get; set; }   
    public DbSet<OLH_VehicleModel> VehicleModels { get; set; } 


    DbSet<OLH_VehicleOwner> VehicleOwners { get; }

    DbSet<OLH_VehicleStatus> VehicleStatuses { get; }

    DbSet<OLH_VehicleType> VehicleTypes { get; }
    //New Table created by sl
    DbSet<OLH_Phase> Phases { get; }
    DbSet<OLH_PhaseCapacity> PhaseCapacities { get; }
    DbSet<OLH_DriverShift> DriverShifts { get; }
    DbSet<OLH_BowserRequestStatusHistory> BowserRequestStatusHistorys { get; }
    DbSet<OLH_BowserAssignmentHistory> BowserAssignmentHistorys { get; }
    DbSet<OLH_Payment> Payments { get; }
    DbSet<OLH_Refund> Refunds { get; }
    DbSet<OLH_BowserRequestNextStatus> BowserRequestsNextStatus { get; }
    


    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    DatabaseFacade Database { get; }
}
