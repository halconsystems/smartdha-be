using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Panic;
public record PanicRequestListDto(
    Guid Id, string CaseNo, int EmergencyCode, string EmergencyName,
    double Latitude, double Longitude, PanicStatus Status, DateTime Created);
