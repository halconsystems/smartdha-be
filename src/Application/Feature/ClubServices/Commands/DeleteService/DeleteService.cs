using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.ClubServices.Commands.DeleteService;
public record DeleteServiceCommand(Guid Id, bool HardDelete = false)
    : IRequest<SuccessResponse<string>>;

public class DeleteServiceCommandHandler : IRequestHandler<DeleteServiceCommand, SuccessResponse<string>>
{
    private readonly IOLMRSApplicationDbContext _ctx;
    public DeleteServiceCommandHandler(IOLMRSApplicationDbContext ctx) => _ctx = ctx;

    public async Task<SuccessResponse<string>> Handle(DeleteServiceCommand request, CancellationToken ct)
    {
        var entity = await _ctx.Services.FindAsync(new object?[] { request.Id }, ct);
        if (entity is null) throw new KeyNotFoundException("Service not found.");

        if (request.HardDelete) _ctx.Services.Remove(entity);
        else { entity.IsDeleted = true; entity.IsActive = false; entity.LastModified = DateTime.UtcNow; }

        await _ctx.SaveChangesAsync(ct);
        return SuccessResponse<string>.FromMessage("Deleted", request.HardDelete ? "Hard deleted." : "Soft deleted.");
    }
}
