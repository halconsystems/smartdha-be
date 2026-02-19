using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.ResidenceProperty.Queries.GetPropertyByList;

public class GetAllPropertiesResponse
{
    public int serialNo { get; set; }   
    public CategoryType Category { get; set; }
    public PropertyType Type { get; set; }
    public Phase Phase { get; set; }
    public Zone Zone { get; set; }
    public string StreetNo { get; set; } = string.Empty;
    public string Khayaban { get; set; } = string.Empty;
    public int Floor { get; set; }
    public int PlotNo { get; set; }
    public string Plot { get; set; } = string.Empty;
    public ResidenceStatusDha PossessionType { get; set; }
}
