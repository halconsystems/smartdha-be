using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Entities.Smartdha;
using MediatR;


namespace DHAFacilitationAPIs.Application.Feature.Vehicles.Queries.GetVehicleByList;

public class GetVehicleListQueryHandler
    : IRequestHandler<GetVehicleListQuery, List<Domain.Entities.Smartdha.Vehicle>>
{
    private readonly IApplicationDbContext _context;
    private readonly ISmartdhaDbContext _smartdhaDbContext;
    private readonly IUser _loggedInUser;

    public GetVehicleListQueryHandler(IApplicationDbContext context, ISmartdhaDbContext smartdhaDbContext, IUser loggedInUser)
    {
        _context = context;
        _smartdhaDbContext = smartdhaDbContext;
        _loggedInUser = loggedInUser;
    }

    public async Task<List<Domain.Entities.Smartdha.Vehicle>> Handle(GetVehicleListQuery request, CancellationToken cancellationtoken)
    {
        return await _smartdhaDbContext.Vehicles
            .Where(x => x.IsActive == true && request.Id == _loggedInUser.Id).ToListAsync(cancellationtoken);
    }
}
