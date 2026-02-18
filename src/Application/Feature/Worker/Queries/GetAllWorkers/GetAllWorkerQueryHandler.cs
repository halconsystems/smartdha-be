using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.UserFamily.Queries.AllUserFamily;

namespace DHAFacilitationAPIs.Application.Feature.Worker.Queries.GetAllWorkers;
public class GetAllWorkerQueryHandler : IRequestHandler<GetAllWorkerQuery, Result<List<GetAllWorkerQueryResponse>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ISmartdhaDbContext _smartdhaDbContext;
    private readonly IUser _loggedInUser;

    public GetAllWorkerQueryHandler(IApplicationDbContext context, ISmartdhaDbContext smartdhaDbContext, IUser loggedInUser)
    {
        _context = context;
        _smartdhaDbContext = smartdhaDbContext;
        _loggedInUser = loggedInUser;
    }

    public async Task<Result<List<GetAllWorkerQueryResponse>>> Handle(GetAllWorkerQuery request, CancellationToken cancellationToken)
    {
        var workers = await _smartdhaDbContext.Workers.Where(w=>w.IsActive == true && request.Id == w.CreatedBy).ToListAsync(cancellationToken);

        if (!workers.Any())
            return Result<List<GetAllWorkerQueryResponse>>
                .Failure(new[] { "No workers found" });

        var response = workers.Select(worker => new GetAllWorkerQueryResponse
        {
            DOB = worker.DateOfBirth,
            Name = worker.Name,
            PhoneNo = worker.PhoneNumber!,
            WorkerCardNo = worker.WorkerCardNumber!,
            CNIC = worker.CNIC!,
            Image = worker.ProfilePicture ?? string.Empty,
            PoliceVerification = worker.PoliceVerification ?? false,
            JobType = worker.JobType,
        }).ToList();

        return Result<List<GetAllWorkerQueryResponse>>.Success(response);
    }
}
