using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Panic;
public record PanicRequestDto(
    Guid Id, string CaseNo, int EmergencyCode, string EmergencyName,
    double Latitude, double Longitude, PanicStatus Status, DateTime CreatedUtc, bool? TakeReview); // New Vehicle Fields);
public record PanicRequestsDto(
    Guid Id, string CaseNo, int EmergencyCode, string EmergencyName,
    double Latitude, double Longitude, PanicStatus Status, DateTime CreatedUtc, bool? TakeReview, // New Vehicle Fields
    Guid? VehicleId,
    string? VehicleName,
    string? VehicleRegistrationNo,
    string? VehicleType,
    string? VehicleStatus,
    double? VehicleLatitude,
    double? VehicleLongitude,
    DateTime? VehicleLastUpdated,
    // DRIVER INFO
    string DriverUserId,
    string DriverName,
    string? DriverEmail,
    string? DriverPhone,
    string? DriverCnic
    );
