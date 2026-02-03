using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.MemberShipCategory.Queries;

namespace DHAFacilitationAPIs.Application.Feature.MemberShipRequest.Queries;

public record GetMemberRequestListQuery : IRequest<List<MemberRequestDTO>>;
public class GetMemberRequestListQueryHandler : IRequestHandler<GetMemberRequestListQuery, List<MemberRequestDTO>>
{
    private readonly IApplicationDbContext _context;

    public GetMemberRequestListQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<MemberRequestDTO>> Handle(GetMemberRequestListQuery request, CancellationToken ct)
    {
        var memberRequest = await _context.MemberRequests
            .Include(x => x.MemberShipCatergories)
            .AsNoTracking().ToListAsync(ct);

        var MemberShipCatergories = await _context.MemberShipCatergories.Where(x => memberRequest.Select(x => x.MemberShipCategory).Contains(x.Id)).ToListAsync(ct);

        var MemberShipPurpose = await _context.MemberShips.Where(x => MemberShipCatergories.Select(x => x.MemberShipId).Contains(x.Id)).ToListAsync(ct);

        var result = memberRequest.Select(x => new MemberRequestDTO
        {
            Id = x.Id,
            Name = x.Name,
            CNIC = x.CNIC,
            Email = x.Email,
            IsActive = x.IsActive,
            IsDelete = x.IsDeleted,
            Purpose = MemberShipPurpose?.FirstOrDefault(p => p.Id == x.MemberShipCategory)?.DisplayName,
            status = x.Status
        }).ToList();

        return result;
    }
}
