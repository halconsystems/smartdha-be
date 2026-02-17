using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.UserFamily.Queries.UserFamilyById;
using DHAFacilitationAPIs.Application.Feature.Worker.Queries.GetAllWorkers;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Worker.Queries.GetWorkerById;
public class GetWorkerByIdQueryHandler : IRequestHandler<GetWorkerByIdQuery, Result<GetWorkerByIdResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ISmartdhaDbContext _smartdhaDbContext;

    public GetWorkerByIdQueryHandler(IApplicationDbContext context, ISmartdhaDbContext smartdhaDbContext)
    {
        _context = context;
        _smartdhaDbContext = smartdhaDbContext;
    }

    public async Task<Result<GetWorkerByIdResponse>> Handle(
     GetWorkerByIdQuery request,
     CancellationToken cancellationToken)
    {
        var workerData = await _smartdhaDbContext.Workers.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (workerData == null)
            return Result<GetWorkerByIdResponse>
                .Failure(new[] { "Worker not found" });

        var userResponse = new GetWorkerByIdResponse
        {
            DOB = workerData.DateOfBirth,
            Name = workerData.Name,
            PhoneNo = workerData.PhoneNumber!,
            JobType = workerData.JobType,
            CNIC = workerData.CNIC!,
            Image = workerData.ProfilePicture ?? string.Empty,
            WorkerCardNo = workerData.WorkerCardNumber!
        };

        return Result<GetWorkerByIdResponse>.Success(userResponse);
    }
}
