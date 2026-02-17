using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.LuggagePass.Queries.GetLuggagePassById;

public class GetLuggagePassByIdQueryHandler
    : IRequestHandler<GetLuggagePassByIdQuery, GetLuggagePassByIdResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ISmartdhaDbContext _smartdhaDbContext;

    public GetLuggagePassByIdQueryHandler(IApplicationDbContext context, ISmartdhaDbContext smartdhaDbContext)
    {
        _context = context;
        _smartdhaDbContext = smartdhaDbContext;
    }

    public async Task<GetLuggagePassByIdResponse> Handle(GetLuggagePassByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _smartdhaDbContext.LuggagePasses
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
            throw new Exception("Luggage Pass Not Found");

        return new GetLuggagePassByIdResponse
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
