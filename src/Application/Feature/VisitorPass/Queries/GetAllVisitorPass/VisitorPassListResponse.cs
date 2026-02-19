using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Dto;



namespace DHAFacilitationAPIs.Application.Feature.VisitorPass.Queries.GetVisitorPassGroupedQuery;

public class VisitorPassListResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }

    public List<VisitorPassDto> UpcomingVisitors { get; set; } = new();
    public List<VisitorPassDto> PreviousVisitors { get; set; } = new();
}

