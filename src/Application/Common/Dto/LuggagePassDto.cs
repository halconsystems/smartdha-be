using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Common.Dto;

public class LuggagePassDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CNIC { get; set; } = string.Empty;
    public string? VehicleInfo { get; set; }
    public LuggagePassType? LuggagePassType { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
