using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.ClubServices.Commands.UpdateService;
public record UpdateServiceCommand(Guid Id, string Name, string? Description, bool? IsActive)
    : IRequest<SuccessResponse<string>>;

public class UpdateServiceCommandHandler : IRequestHandler<UpdateServiceCommand, SuccessResponse<string>>
{
    private readonly IOLMRSApplicationDbContext _ctx;
    public UpdateServiceCommandHandler(IOLMRSApplicationDbContext ctx) => _ctx = ctx;

    public async Task<SuccessResponse<string>> Handle(UpdateServiceCommand request, CancellationToken ct)
    {
        var entity = await _ctx.Services.FindAsync(new object?[] { request.Id }, ct);
        if (entity is null) throw new KeyNotFoundException("Service not found.");

        entity.Name = request.Name;
        entity.Description = request.Description;
        if (request.IsActive.HasValue) entity.IsActive = request.IsActive;
        entity.LastModified = DateTime.UtcNow;

        await _ctx.SaveChangesAsync(ct);
        return Success.Update(entity.Id.ToString());
    }
}
