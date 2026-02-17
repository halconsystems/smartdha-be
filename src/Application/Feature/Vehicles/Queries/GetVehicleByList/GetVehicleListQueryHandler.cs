using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Vehicles.Queries;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Entities.Smartdha;
using MediatR;


namespace DHAFacilitationAPIs.Application.Feature.Vehicles.QueryHandler;

public class GetVehicleListQueryHandler
    : IRequestHandler<GetVehicleListQuery, List<Domain.Entities.Smartdha.Vehicle>>
{
    private readonly IApplicationDbContext _context;
    private readonly ISmartdhaDbContext _smartdhaDbContext;

    public GetVehicleListQueryHandler(IApplicationDbContext context, ISmartdhaDbContext smartdhaDbContext)
    {
        _context = context;
        _smartdhaDbContext = smartdhaDbContext;
    }

    public Task<List<Vehicle>> Handle(GetVehicleListQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    //public async Task<List<Domain.Entities.Smartdha.Vehicle>> Handle(GetVehicleListQuery request, CancellationToken cancellationToken)
    //{
    //    return await _smartdhaDbContext.Vehicles
    //        .Where(x => x.ApplicationUserId == request.ApplicationUserId)
    //        .ToListAsync(cancellationToken);
    //}
}
