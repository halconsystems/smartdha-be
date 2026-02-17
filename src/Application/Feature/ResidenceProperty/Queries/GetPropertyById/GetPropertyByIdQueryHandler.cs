using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Property.Queries;
using DHAFacilitationAPIs.Application.Feature.ResidenceProperty.Queries.GetPropertyById;
using DHAFacilitationAPIs.Domain.Entities.Smartdha;

namespace DHAFacilitationAPIs.Application.Feature.Property.Queryhandler;

public class GetPropertyByIdQueryHandler
    : IRequestHandler<GetPropertyByIdQuery, GetPropertyByIdResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ISmartdhaDbContext _smartdhaDbContext;

    public GetPropertyByIdQueryHandler(IApplicationDbContext context, ISmartdhaDbContext smartdhaDbContext)
    {
        _context = context;
        _smartdhaDbContext = smartdhaDbContext;
    }

    public async Task<GetPropertyByIdResponse> Handle(
          GetPropertyByIdQuery request,
          CancellationToken cancellationToken)
    {
        var entity = await _smartdhaDbContext.ResidentProperties
            .Where(x => x.Id == request.Id
                     && x.IsActive == true
                     && x.IsDeleted == false)
            .FirstOrDefaultAsync(cancellationToken);

        if (entity == null)
        {
            return new GetPropertyByIdResponse
            {
                Success = false,
                Message = "No Active Property Found"
            };
        }

        return new GetPropertyByIdResponse
        {
            Success = true,
            Message = "Property Found Successfully",
            Data = entity
        };
    }

}

