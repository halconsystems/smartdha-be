using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Application.Feature.Refund.Queries.GetRefundPolicy;
public record GetRefundPolicyQuery(Guid ClubId) : IRequest<SuccessResponse<List<RefundPolicy>>>;

public class GetRefundPolicyQueryHandler : IRequestHandler<GetRefundPolicyQuery, SuccessResponse<List<RefundPolicy>>>
{
    private readonly IOLMRSApplicationDbContext _context;

    public GetRefundPolicyQueryHandler(IOLMRSApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<List<RefundPolicy>>> Handle(GetRefundPolicyQuery request, CancellationToken cancellationToken)
    {
        if (request.ClubId == Guid.Empty)
            throw new ArgumentException("ClubId must be provided.");

        var clubExists = await _context.Clubs.AnyAsync(c => c.Id == request.ClubId, cancellationToken);
        if (!clubExists)
            throw new NotFoundException(nameof(Club), request.ClubId.ToString());

        var policies = await _context.RefundPolicies
            .Where(rp => rp.ClubId == request.ClubId)
            .OrderBy(rp => rp.EffectiveFrom)
            .ToListAsync(cancellationToken);

        if (policies == null || !policies.Any())
        {
            throw new NotFoundException(nameof(RefundPolicy), $"No refund policies found for ClubId: {request.ClubId}");
        }

        return new SuccessResponse<List<RefundPolicy>>(policies);
    }
}
