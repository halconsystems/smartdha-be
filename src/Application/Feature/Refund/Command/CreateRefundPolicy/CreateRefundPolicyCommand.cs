using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Refund.Command.CreateRefundPolicy.Dto;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.Refund.Command.AddRefundPolicy;
public record CreateRefundPolicyCommand(CreateRefundPolicyDto Dto) : IRequest<Guid>;

public class CreateRefundPolicyCommandHandler : IRequestHandler<CreateRefundPolicyCommand, Guid>
{
    private readonly IOLMRSApplicationDbContext _context;

    public CreateRefundPolicyCommandHandler(IOLMRSApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateRefundPolicyCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        // Verify club exists
        var club = await _context.Clubs
            .FirstOrDefaultAsync(c => c.Id == dto.ClubId, cancellationToken);

        if (club == null)
            throw new NotFoundException(nameof(Club), dto.ClubId.ToString());

        // Map dto → entity
        var policy = new RefundPolicy
        {
            ClubId = dto.ClubId,
            HoursBeforeCheckIn = dto.HoursBeforeCheckIn,
            RefundPercent = dto.RefundPercent,
            RefundDeposit = dto.RefundDeposit,
            EffectiveFrom = dto.EffectiveFrom,
            EffectiveTo = dto.EffectiveTo
        };

        _context.RefundPolicies.Add(policy);
        await _context.SaveChangesAsync(cancellationToken);

        return policy.Id;
    }
}

