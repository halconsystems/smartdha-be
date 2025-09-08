using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Panic;
public record PanicDetailDto(
    Guid Id, string CaseNo, int EmergencyCode, string EmergencyName,
    decimal Latitude, decimal Longitude, PanicStatus Status,
    DateTime CreatedUtc, DateTime? AcknowledgedAtUtc, DateTime? ResolvedAtUtc, DateTime? CancelledAtUtc,
    string RequestedByUserId, string? AssignedToUserId, string? Notes, string? MediaUrl);
