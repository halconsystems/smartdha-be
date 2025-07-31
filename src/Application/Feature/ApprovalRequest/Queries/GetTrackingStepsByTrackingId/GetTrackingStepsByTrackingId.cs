using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.ApprovalProcess.Queries.GetProcessStepsByProcessId;
using DHAFacilitationAPIs.Application.Feature.ApprovalRequest.Queries.GetRequestTrackings;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.ApprovalRequest.Queries.GetTrackingStepsByTrackingId;

public record GetTrackingStepsByTrackingIdQuery(long TrackingId) : IRequest<SuccessResponse<List<TrackingStepDto>>> { };
public class GetTrackingStepsByTrackingIdQueryHandler
    : IRequestHandler<GetTrackingStepsByTrackingIdQuery, SuccessResponse<List<TrackingStepDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IProcedureService _procedureService;

    public GetTrackingStepsByTrackingIdQueryHandler(IApplicationDbContext context, IProcedureService procedureService)
    {
        _context = context;
        _procedureService = procedureService;
    }

    public async Task<SuccessResponse<List<TrackingStepDto>>> Handle(GetTrackingStepsByTrackingIdQuery request, CancellationToken cancellationToken)
    {
        //var steps = await _context.RequestProcessSteps
        //    .Where(x => x.TrackingId == request.TrackingId)
        //    .OrderBy(x => x.Sequence)
        //    .Select(x => new TrackingStepDto
        //    {
        //        StepName = x.StepName,
        //        Status = x.Status,
        //        Sequence = x.Sequence,
        //        Remarks = x.Remarks,
        //        ProcessDateTime = x.LastModified
        //    })
        //    .ToListAsync(cancellationToken);

        var processList = await _procedureService.ExecuteWithoutParamsAsync<ProcessNameDto>(
    "USP_GetProcessNames",
    cancellationToken
);

        var stepEntities = await (from step in _context.RequestProcessSteps
                                  join tracking in _context.RequestTrackings
                                      on step.TrackingId equals tracking.TrackingId
                                  where step.TrackingId == request.TrackingId
                                  orderby step.Sequence
                                  select new
                                  {
                                      Step = step,
                                      tracking.ProcessId
                                  }).ToListAsync(cancellationToken);

        var steps = stepEntities.Select(x => new TrackingStepDto
        {
            ProcessName = processList.FirstOrDefault(p => p.Id == x.ProcessId)?.Name ?? string.Empty,
            TrackingId = x.Step.TrackingId,
            StepName = x.Step.StepName,
            Status = x.Step.Status,
            Sequence = x.Step.Sequence,
            Remarks = x.Step.Remarks,
            ProcessDateTime = x.Step.LastModified
        }).ToList();





        return new SuccessResponse<List<TrackingStepDto>>(steps, "Steps retrieved successfully.");
    }
}

