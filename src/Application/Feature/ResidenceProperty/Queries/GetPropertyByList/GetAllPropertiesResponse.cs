using System;
using System.Collections.Generic;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.ResidenceProperty.Queries.GetPropertyByList;

public class GetAllPropertiesResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int Category { get; set; }
    public int Type { get; set; }
    public int Phase { get; set; }
    public int Zone { get; set; }
    public string StreetNo { get; set; } = string.Empty;
    public string Khayaban { get; set; } = string.Empty;
    public int Floor { get; set; }
    public int PlotNo { get; set; }
    public string Plot { get; set; } = string.Empty;
    public int PossessionType { get; set; }
}
