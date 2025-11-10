using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Clubs.Commands.CreateClubBookingStandardTime;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Clubs.Queries.GetClubBookingStandardTimes;
public class GetClubBookingStandardTimesQuery : IRequest<List<object>>
{
    public Guid? ClubId { get; set; }
    public ClubType ClubType { get; set; }
}

public class GetClubBookingStandardTimesQueryHandler : IRequestHandler<GetClubBookingStandardTimesQuery, List<object>>
{
    private readonly IOLMRSApplicationDbContext _context;

    public GetClubBookingStandardTimesQueryHandler(IOLMRSApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<object>> Handle(GetClubBookingStandardTimesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.ClubBookingStandardTimes
            .Where(x => x.IsActive == true && x.IsDeleted == false && x.Club.ClubType == request.ClubType);

        if (request.ClubId.HasValue)
        {
            query = query.Where(x => x.ClubId == request.ClubId.Value);
        }

        var result = await query
            .Select(x => new
            {
                x.Id,
                x.ClubId,
                ClubName = x.Club.Name,
                x.CheckInTime,
                x.CheckOutTime
            })
            .ToListAsync(cancellationToken);

        if (!result.Any())
            throw new NotFoundException(nameof(ClubBookingStandardTime), "No Club Booking Standard Times ");

        return result.Cast<object>().ToList(); 
    }
}
