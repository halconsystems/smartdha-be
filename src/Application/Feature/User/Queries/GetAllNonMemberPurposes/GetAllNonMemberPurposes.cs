using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.User.Queries.GetAllNonMemberPurposes;
public record GetAllNonMemberPurposesQuery : IRequest<SuccessResponse<List<MembershipPurposeDto>>>;


public class GetAllNonMemberPurposesQueryHandler : IRequestHandler<GetAllNonMemberPurposesQuery, SuccessResponse<List<MembershipPurposeDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IOLMRSApplicationDbContext _appContext;

    public GetAllNonMemberPurposesQueryHandler(IApplicationDbContext context, IOLMRSApplicationDbContext appContext)
    {
        _context = context;
        _appContext = appContext;
    }

    public async Task<SuccessResponse<List<MembershipPurposeDto>>> Handle(GetAllNonMemberPurposesQuery request, CancellationToken cancellationToken)
    {

       
        var getAll= await _context.MembershipPurposes
            .Where(p => p.IsActive==true)
            .Select(p => new MembershipPurposeDto
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description
            })
            .ToListAsync(cancellationToken);

        return new SuccessResponse<List<MembershipPurposeDto>>(
                        getAll,
                     "NonMember-Purpose Detail for dropdown.",
                     "NonMember-Purpose"
                    );

    }
}

