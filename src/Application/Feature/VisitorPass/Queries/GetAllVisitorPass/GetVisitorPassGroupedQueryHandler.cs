using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Dto;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.VisitorPass.Queries.GetVisitorPassGroupedQuery;

public class GetVisitorPassGroupedQueryHandler
    : IRequestHandler<GetVisitorPassGroupedQuery, VisitorPassListResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ISmartdhaDbContext _smartdhaDbContext;

    public GetVisitorPassGroupedQueryHandler(IApplicationDbContext context, ISmartdhaDbContext smartdhaDbContext)
    {
        _context = context;
        _smartdhaDbContext = smartdhaDbContext;
    }

    public async Task<VisitorPassListResponse> Handle(
        GetVisitorPassGroupedQuery request,
        CancellationToken cancellationToken)
    {

        var visitors = await _smartdhaDbContext.VisitorPasses
            .Where(x => x.IsDeleted == false)
            .ToListAsync(cancellationToken);

        var today = DateTime.Now.Date;

        var upcoming = await _smartdhaDbContext.VisitorPasses
            .Where(x => x.IsActive == true &&
                        x.ValidFrom.Date <= today &&
                        x.ValidTo.Date >= today)
            .ToListAsync(cancellationToken);

        var previous = await _smartdhaDbContext.VisitorPasses
            .Where(x => x.IsActive == true &&
                        x.ValidTo.Date < today)
            .ToListAsync(cancellationToken);


        return new VisitorPassListResponse
        {
            Success = true,
            Message = "Visitor list fetched successfully",
            UpcomingVisitors = upcoming.Select(x => new VisitorPassDto
            {
                Id = x.Id,
                Name = x.Name,
                CNIC = x.CNIC,
                VehicleInfo = $"{x.VehicleLicensePlate}-{x.VehicleLicenseNo}",
                VisitorPassType = x.VisitorPassType,
                FromDate = x.ValidFrom,
                ToDate = x.ValidTo
            }).ToList(),

            PreviousVisitors = previous.Select(x => new VisitorPassDto
            {
                Id = x.Id,
                Name = x.Name,
                CNIC = x.CNIC,
                VehicleInfo = $"{x.VehicleLicensePlate}-{x.VehicleLicenseNo}",
                VisitorPassType = x.VisitorPassType,
                FromDate = x.ValidFrom,
                ToDate = x.ValidTo
            }).ToList()
        };
    }
}

