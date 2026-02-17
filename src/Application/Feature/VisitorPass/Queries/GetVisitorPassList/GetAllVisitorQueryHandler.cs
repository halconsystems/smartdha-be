using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.VisitorPass.Queries.GetVisitorPassbyId;

namespace DHAFacilitationAPIs.Application.Feature.VisitorPass.Queries.GetVisitorPassList;

public class GetAllVisitorPassQueryHandler
    : IRequestHandler<GetAllVisitorPassQuery, List<GetVisitorPassByIdResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ISmartdhaDbContext _smartdhaDbContext;
    public GetAllVisitorPassQueryHandler(IApplicationDbContext context, ISmartdhaDbContext smartdhaDbContext)
    {
        _context = context;
        _smartdhaDbContext = smartdhaDbContext;
    }

    public async Task<List<GetVisitorPassByIdResponse>> Handle(GetAllVisitorPassQuery request, CancellationToken cancellationToken)
    {
        return await _smartdhaDbContext.VisitorPasses
            .Select(x => new GetVisitorPassByIdResponse
            {
                Id = x.Id,
                Name = x.Name,
                CNIC = x.CNIC,
                VehicleLicensePlate = x.VehicleLicensePlate,
                VehicleLicenseNo = x.VehicleLicenseNo,
                VisitorPassType = x.VisitorPassType,
                FromDate = x.ValidFrom,
                ToDate = x.ValidTo,
                QRCode = x.QRCode
            })
            .ToListAsync(cancellationToken);
    }
}
