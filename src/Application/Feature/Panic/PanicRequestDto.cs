using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Panic;
public record PanicRequestDto(
    Guid Id, string CaseNo, int EmergencyCode, string EmergencyName,
    decimal Latitude, decimal Longitude, PanicStatus Status, DateTime CreatedUtc);
