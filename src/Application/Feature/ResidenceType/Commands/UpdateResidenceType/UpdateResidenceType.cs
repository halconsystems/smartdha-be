using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.ResidenceType.Commands.UpdateResidenceType;
public record UpdateResidenceTypeCommand(Guid Id, string Name, string? Description, bool? IsActive)
    : IRequest<SuccessResponse<string>>;

public class UpdateResidenceTypeCommandHandler : IRequestHandler<UpdateResidenceTypeCommand, SuccessResponse<string>>
{
    private readonly IOLMRSApplicationDbContext _ctx;
    public UpdateResidenceTypeCommandHandler(IOLMRSApplicationDbContext ctx) => _ctx = ctx;

    public async Task<SuccessResponse<string>> Handle(UpdateResidenceTypeCommand request, CancellationToken ct)
    {
        var entity = await _ctx.ResidenceTypes.FindAsync(new object?[] { request.Id }, ct);
        if (entity is null) throw new KeyNotFoundException("ResidenceType not found.");

        entity.Name = request.Name;
        entity.Description = request.Description;
        if (request.IsActive.HasValue) entity.IsActive = request.IsActive;
        entity.LastModified = DateTime.UtcNow;

        await _ctx.SaveChangesAsync(ct);
        return SuccessResponse<string>.FromMessage("Updated", "ResidenceType updated.");
    }
}
