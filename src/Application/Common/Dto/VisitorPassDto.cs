using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Common.Dto;

public class VisitorPassDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CNIC { get; set; } = string.Empty;
    public string? VehicleNo { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
}

