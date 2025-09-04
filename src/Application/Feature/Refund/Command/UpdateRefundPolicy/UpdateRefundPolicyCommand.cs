using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.Refund.Command.UpdateRefundPolicy;
public class UpdateRefundPolicyCommand : IRequest<SuccessResponse<Guid>>
{
    public Guid Id { get; set; }   // RefundPolicy Id to update
    public Guid ClubId { get; set; }
    public int HoursBeforeCheckIn { get; set; }
    public decimal RefundPercent { get; set; }
    public bool RefundDeposit {  get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime EffectiveTo { get; set; }
}

public class UpdateRefundPolicyCommandHandler : IRequestHandler<UpdateRefundPolicyCommand, SuccessResponse<Guid>>
{
    private readonly IOLMRSApplicationDbContext _context;

    public UpdateRefundPolicyCommandHandler(IOLMRSApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<Guid>> Handle(UpdateRefundPolicyCommand request, CancellationToken cancellationToken)
    {
        var policy = await _context.RefundPolicies
            .FirstOrDefaultAsync(rp => rp.Id == request.Id, cancellationToken);

        if (policy == null)
            throw new NotFoundException(nameof(RefundPolicy), request.Id.ToString());

        policy.HoursBeforeCheckIn = request.HoursBeforeCheckIn;
        policy.RefundPercent = request.RefundPercent;
        policy.RefundDeposit = request.RefundDeposit;
        policy.EffectiveFrom = request.EffectiveFrom;
        policy.EffectiveTo = request.EffectiveTo;


        _context.RefundPolicies.Update(policy);
        await _context.SaveChangesAsync(cancellationToken);

        return new SuccessResponse<Guid>(policy.Id, "Refund policy updated successfully.");
    }
}
