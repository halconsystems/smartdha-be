using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Dto;

namespace DHAFacilitationAPIs.Application.Feature.LuggagePass.Queries.GetAllLuggagePass;

public class GetAllLuggagePassResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }

    public List<LuggagePassDto> UpcomingLuggage { get; set; } = new();
    public List<LuggagePassDto> PreviousLuggage { get; set; } = new();
}
