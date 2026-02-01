using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums.CBMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.Discount.Commands.CreateDiscount;
public record CreateDiscountDto(
    string Name,
    string Code,
    DiscountType Type,
    decimal Value,
    DateOnly? ValidFrom,
    DateOnly? ValidTo,
    decimal? MinOrderAmount
);

