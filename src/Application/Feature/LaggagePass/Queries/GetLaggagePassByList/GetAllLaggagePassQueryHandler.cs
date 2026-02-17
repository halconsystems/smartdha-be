using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.LaggagePass.Queries.GetLaggagePassById;

namespace DHAFacilitationAPIs.Application.Feature.LaggagePass.Queries.GetLaggagePassByList;

public class GetAllLaggagePassQueryHandler
    : IRequestHandler<GetAllLaggagePassQuery, List<GetLaggagePassByIdResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ISmartdhaDbContext _smartdhaDbContext;

    public GetAllLaggagePassQueryHandler(IApplicationDbContext context, ISmartdhaDbContext smartdhaDbContext)
    {
        _context = context;
        _smartdhaDbContext = smartdhaDbContext;
    }

    public async Task<List<GetLaggagePassByIdResponse>> Handle(GetAllLaggagePassQuery request, CancellationToken cancellationToken)
    {
        return await _smartdhaDbContext.LaggagePasses
            .Select(x => new GetLaggagePassByIdResponse
            {
                Id = x.Id,
                Name = x.Name,
                CNIC = x.CNIC,
                VehicleLicensePlate = x.VehicleLicensePlate,
                VehicleLicenseNo = x.VehicleLicenseNo,
                Description = x.Description,
                ValidFrom = x.ValidFrom,
                ValidTo = x.ValidTo,
                QRCode = x.QRCode
            })
            .ToListAsync(cancellationToken);
    }
}
