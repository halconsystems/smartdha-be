using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.VisitorPass.Queries.GetVisitorPassbyId;

public class GetVisitorPassByIdQueryHandler
    : IRequestHandler<GetVisitorPassByIdQuery, GetVisitorPassByIdResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ISmartdhaDbContext _smartdhaDbContext;
    public GetVisitorPassByIdQueryHandler(IApplicationDbContext context, ISmartdhaDbContext smartdhaDbContext)
    {
        _context = context;
        _smartdhaDbContext = smartdhaDbContext;
    }

    public async Task<GetVisitorPassByIdResponse> Handle(GetVisitorPassByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _smartdhaDbContext.VisitorPasses
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
            throw new Exception("Visitor Pass Not Found");

        return new GetVisitorPassByIdResponse
        {
            Id = entity.Id,
            Name = entity.Name,
            CNIC = entity.CNIC,
            VehicleLicensePlate = entity.VehicleLicensePlate,
            VehicleLicenseNo = entity.VehicleLicenseNo,
            VisitorPassType = entity.VisitorPassType,
            ValidFrom = entity.ValidFrom,
            ValidTo = entity.ValidTo,
            QRCode = entity.QRCode
        };
    }
}
