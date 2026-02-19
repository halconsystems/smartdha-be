using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Dto;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.VisitorPass.Queries.GetVisitorPassGroupedQuery;

namespace DHAFacilitationAPIs.Application.Feature.LuggagePass.Queries.GetAllLuggagePass;

public class GetAllLuggagePassQueryHandler : IRequestHandler<GetAllLuggagePassQuery, GetAllLuggagePassResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ISmartdhaDbContext _smartdhaDbContext;

    public GetAllLuggagePassQueryHandler(IApplicationDbContext context, ISmartdhaDbContext smartdhaDbContext)
    {
        _context = context;
        _smartdhaDbContext = smartdhaDbContext;
    }

    public async Task<GetAllLuggagePassResponse> Handle(
        GetAllLuggagePassQuery request,
        CancellationToken cancellationToken)
    {

        var visitors = await _smartdhaDbContext.LuggagePasses
            .Where(x => x.IsDeleted == false)
            .ToListAsync(cancellationToken);

        var today = DateTime.Now.Date;

        var upcoming = await _smartdhaDbContext.LuggagePasses
            .Where(x => x.IsActive == true &&
                        x.ValidFrom.Date <= today &&
                        x.ValidTo.Date >= today)
            .ToListAsync(cancellationToken);

        var previous = await _smartdhaDbContext.LuggagePasses
            .Where(x => x.IsActive == true &&
                        x.ValidTo.Date < today)
            .ToListAsync(cancellationToken);


        return new GetAllLuggagePassResponse
        {
            Success = true,
            Message = "Visitor list fetched successfully",
            UpcomingLuggage = upcoming.Select(x => new LuggagePassDto
            {
                Id = x.Id,
                Name = x.Name,
                CNIC = x.CNIC,
                VehicleInfo = $"{x.VehicleLicensePlate}-{x.VehicleLicenseNo}",
                VisitDetail = x.LuggagePassType,
                FromDate = x.ValidFrom,
                ToDate = x.ValidTo
            }).ToList(),

            PreviousLuggage = previous.Select(x => new LuggagePassDto
            {
                Id = x.Id,
                Name = x.Name,
                CNIC = x.CNIC,
                VehicleInfo = $"{x.VehicleLicensePlate}-{x.VehicleLicenseNo}",
                VisitDetail = x.LuggagePassType,
                FromDate = x.ValidFrom,
                ToDate = x.ValidTo
            }).ToList()
        };
    }
}
