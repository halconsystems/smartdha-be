using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.ApprovalProcess.Queries.GetProcessStepsByProcessId;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace DHAFacilitationAPIs.Application.Feature.ApprovalRequest.Commands;
public record CreateApprovalRequestCommand : IRequest<SuccessResponse<long>>
{
    public required string PLot_ID { get; set; }
    public required string PLOT_NO { get; set; }

    public required string MemPK { get; set; }
    public int ProcessId { get; set; }
    public string? Remarks { get; set; }
}
public class CreateApprovalRequestCommandHandler : IRequestHandler<CreateApprovalRequestCommand, SuccessResponse<long>>
{
    private readonly IApplicationDbContext _context;
    private readonly IProcedureService _procedureService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateApprovalRequestCommandHandler(
        IApplicationDbContext context,
        IProcedureService procedureService,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _procedureService = procedureService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<SuccessResponse<long>> Handle(CreateApprovalRequestCommand request, CancellationToken cancellationToken)
    {
        // Generate unique TrackingId (e.g., using timestamp or sequence logic)
        long trackingId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        var cnicClaim = _httpContextAccessor.HttpContext?.User?.FindFirstValue("CNIC");
        if (cnicClaim == null || String.IsNullOrWhiteSpace(cnicClaim))
        {
            cnicClaim = "3220384127171";
        }



        // Call stored procedure to get steps for the given ProcessId
        var parameters = new DynamicParameters();
        parameters.Add("@ProcessID", request.ProcessId, DbType.Int32);

        var (_, steps) = await _procedureService.ExecuteWithListAsync<ProcessStepDto>(
            "USP_GetProcessStepsByProcessId", parameters, cancellationToken);

        if (steps == null || steps.Count == 0)
            throw new Exception("No steps found for the given process.");

        // Create main tracking request
        var tracking = new RequestTracking
        {

            MemPK = request.MemPK,
            CNIC= cnicClaim,
            TrackingId = trackingId,
            Status = "Pending",
            Remarks=request.Remarks,
            ProcessId= request.ProcessId,
            PLot_ID=request.PLot_ID,
            PLOT_NO=request.PLOT_NO
        };

        _context.RequestTrackings.Add(tracking);


        steps = steps.OrderBy(x => x.Sequence).ToList();

        var processSteps = steps.Select(step => new RequestProcessStep
        {
            RequestTrackingId = tracking.Id,
            TrackingId= trackingId,
            StepName = step.ProcessStep,
            Sequence= step.Sequence,
            Remarks=request.Remarks,
            Status = "Pending"
        }).ToList();


        _context.RequestProcessSteps.AddRange(processSteps);
        await _context.SaveChangesAsync(cancellationToken);

        return new SuccessResponse<long>(trackingId, "Request submitted successfully with all process steps.");
    }
}

