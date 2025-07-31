using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Proeperty.Queries.GetPropertyByMemPK;
using DHAFacilitationAPIs.Application.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace DHAFacilitationAPIs.Application.Feature.ApprovalRequest.Queries.GetRequestTrackings;
public class GetRequestTrackingsQuery : IRequest<SuccessResponse<List<PlotWithTrackingDto>>>{};

public class GetRequestTrackingsQueryHandler : IRequestHandler<GetRequestTrackingsQuery, SuccessResponse<List<PlotWithTrackingDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IProcedureService _procedureService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetRequestTrackingsQueryHandler(IApplicationDbContext context, IUser currentUser, IProcedureService procedureService,IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _procedureService = procedureService;
        _httpContextAccessor=httpContextAccessor;
    }

    public async Task<SuccessResponse<List<PlotWithTrackingDto>>> Handle(GetRequestTrackingsQuery request, CancellationToken cancellationToken)
    {
        string cnic = "";
        var usercnic = _httpContextAccessor.HttpContext?.User?.FindFirstValue("CNIC");
        if (string.IsNullOrEmpty(usercnic))
            throw new UnauthorizedAccessException("User is not authenticated.");
        cnic =usercnic;

        if (string.IsNullOrEmpty(cnic))
            throw new UnauthorizedAccessException("CNIC not found in token.");

        var trackings = await _context.RequestTrackings
            .Where(x => x.CNIC == cnic && x.MemPK != null)
            .ToListAsync(cancellationToken);

        var result = new List<PlotWithTrackingDto>();

        var processList = await _procedureService.ExecuteWithoutParamsAsync<ProcessNameDto>(
     "USP_GetProcessNames",
     cancellationToken
 );
      


        foreach (var tracking in trackings)
        {
            var getAllStep = await _context.RequestProcessSteps
    .Where(x => x.TrackingId == tracking.TrackingId)
    .ToListAsync();

            string status;

            if (getAllStep.Any(x => x.Status == "Rejected"))
            {
                status = "Rejected";
            }
            else if (getAllStep.All(x => x.Status == "Approved"))
            {
                status = "Approved";
            }
            else if (getAllStep.All(x => x.Status == "Pending"))
            {
                status = "Pending";
            }
            else
            {
                status = "Pending";
            }



            var parameters = new DynamicParameters();
            parameters.Add("@memPK", tracking.MemPK, DbType.String, size: 120);

            var (_, plots) = await _procedureService.ExecuteWithListAsync<PlotDto>(
                "USP_GetPropertyByMemPK", parameters, cancellationToken);

            var matchingPlot = plots.FirstOrDefault(p =>
            (!string.IsNullOrEmpty(tracking.PLOT_NO) && p.PLOT_NO == tracking.PLOT_NO) &&
            (!string.IsNullOrEmpty(tracking.PLot_ID) && p.PLot_ID == tracking.PLot_ID));

            if (matchingPlot != null)
            {
                result.Add(new PlotWithTrackingDto
                {
                    // Plot Info
                    PLot_ID = matchingPlot.PLot_ID,
                    PLOT_NO = matchingPlot.PLOT_NO,
                    STNAME = matchingPlot.STNAME,
                    PLTNO = matchingPlot.PLTNO,
                    SUBDIV = matchingPlot.SUBDIV,
                    PTYPE = matchingPlot.PTYPE,
                    PHASE = matchingPlot.PHASE,
                    EXT = matchingPlot.EXT,
                    NOMAREA = matchingPlot.NOMAREA,
                    ACTUAL_SIZE = matchingPlot.ACTUAL_SIZE,
                    STREET1COD = matchingPlot.STREET1COD,
                    PLOTPK = matchingPlot.PLOTPK,
                    MEMPK = matchingPlot.MEMPK,

                    // Tracking Info
                    TrackingId = tracking.TrackingId,
                    TrackingStatus = status,
                    TrackingRemarks = tracking.Remarks,
                    TrackingDescription = tracking.Description,
                    ProcessName = processList
                  .FirstOrDefault(x => x.Id == tracking.ProcessId)?
                  .Name ?? string.Empty

                });
            }
        }

        return new SuccessResponse<List<PlotWithTrackingDto>>(result, "Properties with tracking info fetched successfully.");
    }

}


