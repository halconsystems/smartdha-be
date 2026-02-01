using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.Discount.Commands.AssignFacilityUnitDiscount;
public record AssignFacilityUnitDiscountDto(
    Guid FacilityUnitId,
    Guid DiscountId
);

