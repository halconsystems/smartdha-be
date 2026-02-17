using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.LaggagePass.Queries.GetLaggagePassById;

public class GetLaggagePassByIdQueryHandler
    : IRequestHandler<GetLaggagePassByIdQuery, GetLaggagePassByIdResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ISmartdhaDbContext _smartdhaDbContext;

    public GetLaggagePassByIdQueryHandler(IApplicationDbContext context, ISmartdhaDbContext smartdhaDbContext)
    {
        _context = context;
        _smartdhaDbContext = smartdhaDbContext;
    }

    public async Task<GetLaggagePassByIdResponse> Handle(GetLaggagePassByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _smartdhaDbContext.LaggagePasses
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
            throw new Exception("Laggage Pass Not Found");

        return new GetLaggagePassByIdResponse
        {
            Id = entity.Id,
            Name = entity.Name,
            CNIC = entity.CNIC,
            VehicleLicensePlate = entity.VehicleLicensePlate,
            VehicleLicenseNo = entity.VehicleLicenseNo,
            Description = entity.Description,
            ValidFrom = entity.ValidFrom,
            ValidTo = entity.ValidTo,
            QRCode = entity.QRCode
        };
    }
}
