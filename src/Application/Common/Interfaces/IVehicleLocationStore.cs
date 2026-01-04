using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Models;

namespace DHAFacilitationAPIs.Application.Common.Interfaces;
public interface IVehicleLocationStore
{
    Task SaveLocationAsync(Guid vehicleId, double latitude, double longitude, DateTime timestamp);
    Task<VehicleLocationDto?> GetLocationAsync(Guid vehicleId);
    string GetPublicUrl(Guid vehicleId);
}

