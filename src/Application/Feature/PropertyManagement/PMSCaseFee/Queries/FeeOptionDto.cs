using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCaseFee.Queries;
public record FeeOptionDto(
    Guid Id,
    string Code,
    string Name,
    int ProcessingDays,
    decimal Amount,
    int SorOrder
);


