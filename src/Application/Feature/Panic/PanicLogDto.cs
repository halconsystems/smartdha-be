using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Panic;
public record PanicLogDto(
    DateTime CreatedUtc, string ActionByUserId, string Action, string? Remarks,
    PanicStatus FromStatus, PanicStatus ToStatus);
