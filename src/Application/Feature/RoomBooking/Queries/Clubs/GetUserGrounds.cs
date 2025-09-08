using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Feature.RoomBooking.Queries.Clubs.Dtos;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.Clubs;
public class GetGroundClubsQuery : IRequest<List<UserClubDto>>
{
    // No UserId needed anymore
}

public class GetGroundClubsQueryHandler : IRequestHandler<GetGroundClubsQuery, List<UserClubDto>>
{
    private readonly IOLMRSApplicationDbContext _context;

    public GetGroundClubsQueryHandler(IOLMRSApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<UserClubDto>> Handle(GetGroundClubsQuery request, CancellationToken cancellationToken)
    {
        var clubs = await _context.Clubs
            .AsNoTracking()
            .Where(c => c.ClubType == ClubType.Ground && c.IsActive == true && c.IsDeleted != true)
            .Select(c => new UserClubDto
            {
                ClubId = c.Id.ToString(),
                Name = c.Name
            })
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return clubs;
    }
}

