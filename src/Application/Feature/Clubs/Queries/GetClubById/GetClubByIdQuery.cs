using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using static System.Reflection.Metadata.BlobBuilder;

namespace DHAFacilitationAPIs.Application.Feature.Clubs.Queries.GetClubById;
public record GetClubByIdQuery(Guid Id) : IRequest<SuccessResponse<ClubDto>>;

public class GetClubByIdQueryHandler
    : IRequestHandler<GetClubByIdQuery, SuccessResponse<ClubDto>>
{
    private readonly IOLMRSApplicationDbContext _ctx;
    private readonly IMapper _mapper;

    public GetClubByIdQueryHandler(IOLMRSApplicationDbContext ctx, IMapper mapper)
        => (_ctx, _mapper) = (ctx, mapper);

    public async Task<SuccessResponse<ClubDto>> Handle(GetClubByIdQuery request, CancellationToken ct)
    {
        var dto = await _ctx.Clubs
            .AsNoTracking()
            .Where(x => x.Id == request.Id && (x.IsDeleted == null || x.IsDeleted == false))
            .ProjectTo<ClubDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);

        if (dto is null)
            throw new NotFoundException(nameof(Club), request.Id.ToString());

        return Success.Ok(dto);
    }
}

