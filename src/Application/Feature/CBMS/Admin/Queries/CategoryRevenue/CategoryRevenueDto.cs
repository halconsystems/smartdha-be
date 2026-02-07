using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.Admin.Queries.CategoryRevenue;
public record CategoryRevenueDto(
    Guid CategoryId,
    string CategoryName,
    decimal Revenue
);

