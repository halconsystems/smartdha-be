using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.LuggagePass.Queries.GetLuggagePassById;

namespace DHAFacilitationAPIs.Application.Feature.LuggagePass.Queries.GetLuggagePassByList;

public class GetAllLuggagePassQueryHandler
    : IRequestHandler<GetAllLuggagePassQuery, List<GetLuggagePassByIdResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ISmartdhaDbContext _smartdhaDbContext;
    private readonly IUser _loggedInUser;

    public GetAllLuggagePassQueryHandler(IApplicationDbContext context, ISmartdhaDbContext smartdhaDbContext, IUser loggedInUser)
    {
        _context = context;
        _smartdhaDbContext = smartdhaDbContext;
        _loggedInUser = loggedInUser;
    }

    public async Task<List<GetLuggagePassByIdResponse>> Handle(GetAllLuggagePassQuery request, CancellationToken cancellationToken)
    {
            return await _smartdhaDbContext.LuggagePasses.Where(x => x.IsActive == true && request.Id == _loggedInUser.Id)
                .Select(x => new GetLuggagePassByIdResponse
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
