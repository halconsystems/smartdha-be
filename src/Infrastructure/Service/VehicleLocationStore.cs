using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.ViewModels;
using Microsoft.Extensions.Options;

namespace DHAFacilitationAPIs.Infrastructure.Service;
public class VehicleLocationStore : IVehicleLocationStore
{
    private readonly string _vehicleLocationPath;
    private readonly string _publicBaseUrl;

    public VehicleLocationStore(IOptions<FileStorageOptions> options)
    {
        var opt = options.Value;  // THIS should NOT be null

        var root = opt.RootPath ?? throw new Exception("RootPath missing from FileStorage config");
        _publicBaseUrl = (opt.PublicBaseUrl ?? throw new Exception("PublicBaseUrl missing"))
            .TrimEnd('/') + "/";


        _vehicleLocationPath = Path.Combine(root, "vehicle-locations");

        if (!Directory.Exists(_vehicleLocationPath))
            Directory.CreateDirectory(_vehicleLocationPath);
    }

    private string GetFilePath(Guid vehicleId)
        => Path.Combine(_vehicleLocationPath, $"{vehicleId}.json");

    public async Task SaveLocationAsync(Guid vehicleId, double lat, double lng, DateTime timestamp)
    {
        var dto = new VehicleLocationDto
        {
            VehicleId = vehicleId,
            Latitude = lat,
            Longitude = lng,
            LastLocationUpdateAt = timestamp
        };

        string json = JsonSerializer.Serialize(dto, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        var filePath = GetFilePath(vehicleId);

        await File.WriteAllTextAsync(filePath, json);
    }

    public async Task<VehicleLocationDto?> GetLocationAsync(Guid vehicleId)
    {
        var filePath = GetFilePath(vehicleId);

        if (!File.Exists(filePath))
            return null;

        string json = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<VehicleLocationDto>(json);
    }

    public string GetPublicUrl(Guid vehicleId)
    {
        return $"{_publicBaseUrl}vehicle-locations/{vehicleId}.json";
    }
}

