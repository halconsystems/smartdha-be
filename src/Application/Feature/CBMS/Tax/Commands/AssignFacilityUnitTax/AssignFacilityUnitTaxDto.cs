using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.Tax.Commands.AssignFacilityUnitTax;
public record AssignFacilityUnitTaxDto(
    Guid FacilityUnitId,
    Guid TaxId
);

