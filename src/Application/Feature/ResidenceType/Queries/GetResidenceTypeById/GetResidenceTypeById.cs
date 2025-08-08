using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.ResidenceType.Queries.GetResidenceTypeById;
// Query + Handler (DTO + SuccessResponse)
public record GetResidenceTypeByIdQuery(Guid Id)
    : IRequest<SuccessResponse<ResidenceTypeDto?>>;

public class GetResidenceTypeByIdQueryHandler
    : IRequestHandler<GetResidenceTypeByIdQuery, SuccessResponse<ResidenceTypeDto?>>
{
    private readonly IOLMRSApplicationDbContext _ctx;
    private readonly IMapper _mapper;

    public GetResidenceTypeByIdQueryHandler(IOLMRSApplicationDbContext ctx, IMapper mapper)
        => (_ctx, _mapper) = (ctx, mapper);

    public async Task<SuccessResponse<ResidenceTypeDto?>> Handle(GetResidenceTypeByIdQuery request, CancellationToken ct)
    {
        var dto = await _ctx.ResidenceTypes
            .AsNoTracking()
            .Where(x => x.Id == request.Id && (x.IsDeleted == false || x.IsDeleted == null))
            .ProjectTo<ResidenceTypeDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);

        if (dto is null)
            throw new NotFoundException(nameof(ResidenceType), request.Id.ToString());

        return new SuccessResponse<ResidenceTypeDto?>(dto);
        // If you added the helper: return Success.Ok(dto);
        // Or throw if not found:
        // if (dto is null) throw new NotFoundException(nameof(ResidenceType), request.Id);
    }
}

