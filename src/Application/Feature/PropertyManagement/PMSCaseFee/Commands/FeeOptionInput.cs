using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCaseFee.Commands;
public record FeeOptionInput(
    Guid? FeeCategoryId,   // NULL for OptionBased
    string Code,
    string Name,
    int ProcessingDays,
    decimal Amount,
    int SortOrder
);

