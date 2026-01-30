using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCase.Queries;
public class AdminCasePropertyDto
{
    public Guid PropertyId { get; set; }

    public string PlotNo { get; set; } = string.Empty;
    public string? PlotNoAlt { get; set; }

    public string? StreetName { get; set; }
    public string? SubDivision { get; set; }

    public string? PropertyType { get; set; }
    public string? Phase { get; set; }
    public string? Extension { get; set; }

    public string? NominalArea { get; set; }
    public string? Area { get; set; }              // ACTUAL_SIZE (UI-friendly name)

    public string? StreetCode { get; set; }

    public string? PropertyPk { get; set; }
    public string? MemberPk { get; set; }
    public string? MemberNo { get; set; }

    public string? Category { get; set; }
    public string? MemberName { get; set; }
    public DateTime? ApplicationDate { get; set; }

    public string? OwnerCnic { get; set; }
    public string? CellNo { get; set; }

    public bool? AllResidentialPlot { get; set; }
}

