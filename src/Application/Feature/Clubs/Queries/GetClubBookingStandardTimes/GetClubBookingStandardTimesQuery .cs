using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Clubs.Commands.CreateClubBookingStandardTime;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.Clubs.Queries.GetClubBookingStandardTimes;
public class GetClubBookingStandardTimesQuery : IRequest<List<ClubBookingStandardTimeDto>>;

public class GetClubBookingStandardTimesQueryHandler : IRequestHandler<GetClubBookingStandardTimesQuery, List<ClubBookingStandardTimeDto>>
{
    private readonly IOLMRSApplicationDbContext _context;

    public GetClubBookingStandardTimesQueryHandler(IOLMRSApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ClubBookingStandardTimeDto>> Handle(GetClubBookingStandardTimesQuery request, CancellationToken cancellationToken)
    {
        var result = await _context.ClubBookingStandardTimes
            .Where(x => x.IsActive == true && x.IsDeleted == false)
            .Select(x => new ClubBookingStandardTimeDto(
                x.ClubId,
                x.CheckInTime,
                x.CheckOutTime
            ))
            .ToListAsync(cancellationToken);

        if (!result.Any())
            throw new NotFoundException(nameof(ClubBookingStandardTime), "No Club Booking Standard Times ");

        return result;
    }
}
