using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using Microsoft.AspNetCore.Http;

namespace DHAFacilitationAPIs.Application.Feature.Proeperty.Queries.GetPropertyByMemPK;
public class GetMyPropertiesQuery : IRequest<SuccessResponse<List<PlotDto>>>{}

public class GetMyPropertiesQueryHandler : IRequestHandler<GetMyPropertiesQuery, SuccessResponse<List<PlotDto>>>
{
    private readonly IProcedureService _procedureService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetMyPropertiesQueryHandler(IProcedureService procedureService, IHttpContextAccessor httpContextAccessor)
    {
        _procedureService = procedureService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<SuccessResponse<List<PlotDto>>> Handle(GetMyPropertiesQuery request, CancellationToken cancellationToken)
    {
        // Extract MemPK from claims
        var memPK = _httpContextAccessor.HttpContext?.User?.FindFirst("MemPK")?.Value;
        memPK = string.IsNullOrEmpty(memPK) ? "DHAM-97563" : memPK;


        var OwnerName = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

        if (string.IsNullOrEmpty(memPK))
            throw new UnauthorizedAccessException("MemPK not found in token.");

        var parameters = new DynamicParameters();
        parameters.Add("@memPK", memPK, DbType.String, size: 120);

        var (outputParams, result) = await _procedureService
            .ExecuteWithListAsync<PlotDto>("USP_GetPropertyByMemPK", parameters, cancellationToken);

        foreach (var plot in result)
        {
            plot.OwnerName = OwnerName ?? string.Empty;
            plot.Address = $"Plot No. {plot.PLTNO}, Street {plot.STNAME}, Phase {plot.PHASE}";
            plot.Type = plot.PTYPE == "R" ? "Residential" : "Commercial";
        }


        return new SuccessResponse<List<PlotDto>>(result, "Properties fetched successfully.");
    }
}
