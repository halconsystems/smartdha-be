using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Property.Queries;
using DHAFacilitationAPIs.Application.Feature.Vehicles.Queries.GetVehicleById;

namespace DHAFacilitationAPIs.Application.Feature.Vehicles.Queries.GetVehicleById;

public class GetVehicleByIdQueryHandler
    : IRequestHandler<GetVehicleByIdQuery, GetVehicleByIdResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ISmartdhaDbContext _smartdhaDbContext;

    public GetVehicleByIdQueryHandler(IApplicationDbContext context, ISmartdhaDbContext smartdhaDbContext)
    {
        _context = context;
        _smartdhaDbContext = smartdhaDbContext;
    }

    public async Task<GetVehicleByIdResponse> Handle(
       GetVehicleByIdQuery request,
       CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _smartdhaDbContext.Vehicles   
                .Where(x => x.Id == request.Id
                         && x.IsActive == true
                         && x.IsDeleted == false)
                .FirstOrDefaultAsync(cancellationToken);

            if (entity == null)
            {
                return new GetVehicleByIdResponse
                {
                    Success = false,
                    Message = "No Active Vehicle Found"
                };
            }

            return new GetVehicleByIdResponse
            {
                Success = true,
                Message = "Vehicle Found Successfully",
                Data = entity
            };
        }
        catch (Exception ex)
        {
            return new GetVehicleByIdResponse
            {
                Success = false,
                Message = $"An error occurred: {ex.Message}"
            };
        }
    }

}
