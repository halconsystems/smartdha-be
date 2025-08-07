using Application.Feature.RoomBooking.Queries.Clubs.Dtos;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.Clubs;

public class GetUserClubsQuery : IRequest<List<UserClubDto>>
{
    public Guid UserId { get; set; } = default!;  // Pass this from claims or query param
}

public class GetUserClubsQueryHandler : IRequestHandler<GetUserClubsQuery, List<UserClubDto>>
{
    private readonly IOLMRSApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetUserClubsQueryHandler(IOLMRSApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<UserClubDto>> Handle(GetUserClubsQuery request, CancellationToken cancellationToken)
    {
        return await _context.UserClubMembership
            .Include(x => x.ClubId)
            .Where(x => x.UserId == request.UserId && x.IsActive == true && x.IsDeleted == false)
            .ProjectTo<UserClubDto>(_mapper.ConfigurationProvider)
            .Distinct()
            .ToListAsync(cancellationToken);
    }
}
