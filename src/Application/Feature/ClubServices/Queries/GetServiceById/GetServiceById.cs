using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.ClubServices.Queries.GetServiceById;
public record GetServiceByIdQuery(Guid Id) : IRequest<SuccessResponse<ServiceDto>>;

public class GetServiceByIdQueryHandler : IRequestHandler<GetServiceByIdQuery, SuccessResponse<ServiceDto>>
{
    private readonly IOLMRSApplicationDbContext _ctx;
    private readonly IMapper _mapper;
    public GetServiceByIdQueryHandler(IOLMRSApplicationDbContext ctx, IMapper mapper) => (_ctx, _mapper) = (ctx, mapper);

    public async Task<SuccessResponse<ServiceDto>> Handle(GetServiceByIdQuery request, CancellationToken ct)
    {
        var dto = await _ctx.Clubs
            .AsNoTracking()
            .Where(x => x.Id == request.Id && (x.IsDeleted == false || x.IsDeleted == null))
            .ProjectTo<ServiceDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);

        if (dto is null)
            throw new NotFoundException(nameof(Services), request.Id.ToString());

        return Success.Ok(dto);
    }
}
