using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums.CBMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.Tax.Commands.CreateTax;
public record CreateTaxDto(
    string Name,
    string Code,
    TaxType Type,
    decimal Value
);

