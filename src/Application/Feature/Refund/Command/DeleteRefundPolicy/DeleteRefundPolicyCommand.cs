using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DHAFacilitationAPIs.Application.Feature.Room.Commands.DeleteRoomCharges;

public class DeleteRefundPolicyCommand : IRequest<SuccessResponse<string>>
{
    public Guid Id { get; set; }            // RoomCharge Id
    public bool HardDelete { get; set; } = false;
}

public class DeleteRefundPolicyCommandHandler : IRequestHandler<DeleteRefundPolicyCommand, SuccessResponse<string>>
{
    private readonly IOLMRSApplicationDbContext _ctx;

    public DeleteRefundPolicyCommandHandler(IOLMRSApplicationDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<SuccessResponse<string>> Handle(DeleteRefundPolicyCommand request, CancellationToken ct)
    {
        var entity = await _ctx.RefundPolicies
            .FirstOrDefaultAsync(rc => rc.Id == request.Id, ct);

        if (entity is null)
            throw new KeyNotFoundException("RefundPolicy not found.");

        if (request.HardDelete)
        {
            _ctx.RefundPolicies.Remove(entity);
        }
        else
        {
            entity.IsDeleted = true;      // assumes soft delete flags exist in RoomCharge entity
            entity.IsActive = false;
            entity.LastModified = DateTime.Now;
        }

        await _ctx.SaveChangesAsync(ct);

        return Success.Delete(entity.Id.ToString());
    }
}
