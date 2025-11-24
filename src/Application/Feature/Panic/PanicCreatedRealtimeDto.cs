using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Panic;
public sealed record PanicCreatedRealtimeDto(
    Guid Id,
    string CaseNo,
    int EmergencyCode,
    string EmergencyName,
    double Latitude,
    double Longitude,
    PanicStatus Status,
    DateTime CreatedUtc,
    string Address,
    string Note,
    string MobileNumber,

    // 👇 User details
    string RequestedByName,
    string RequestedByEmail,
    string RequestedByPhone,
    UserType RequestedByUserType
);
