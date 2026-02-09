using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.Proeperty.Queries.GetPropertyByMemPK;
public record PlotInfoDto
(
    string? PropertyNo,
    string? Sector,
    string? Phase,
    string? Address,
    string? Name,
    string? CNIC
);
