using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.User.Queries.GetAllNonMemberPurposes;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.MembershipPurpose.Queries.GetAllMembershipPurposes;
public record GetAllMembershipPurposesQuery() : IRequest<SuccessResponse<List<MembershipPurposeDto>>>;

public class GetAllMembershipPurposesQueryHandler : IRequestHandler<GetAllMembershipPurposesQuery, SuccessResponse<List<MembershipPurposeDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetAllMembershipPurposesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<List<MembershipPurposeDto>>> Handle(GetAllMembershipPurposesQuery request, CancellationToken cancellationToken)
    {
        var list = await _context.MembershipPurposes
            .AsNoTracking()
            .OrderBy(x => x.Title)
            .Select(x => new MembershipPurposeDto
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description
            })
            .ToListAsync(cancellationToken);

        return new SuccessResponse<List<MembershipPurposeDto>>(list);
    }
}

