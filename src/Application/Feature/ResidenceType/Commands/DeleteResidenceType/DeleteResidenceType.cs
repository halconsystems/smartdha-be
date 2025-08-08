using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.ResidenceType.Commands.DeleteResidenceType;
public record DeleteResidenceTypeCommand(Guid Id, bool HardDelete = false)
    : IRequest<SuccessResponse<string>>;

public class DeleteResidenceTypeCommandHandler : IRequestHandler<DeleteResidenceTypeCommand, SuccessResponse<string>>
{
    private readonly IOLMRSApplicationDbContext _ctx;
    public DeleteResidenceTypeCommandHandler(IOLMRSApplicationDbContext ctx) => _ctx = ctx;

    public async Task<SuccessResponse<string>> Handle(DeleteResidenceTypeCommand request, CancellationToken ct)
    {
        var entity = await _ctx.ResidenceTypes.FindAsync(new object?[] { request.Id }, ct);
        if (entity is null) throw new KeyNotFoundException("ResidenceType not found.");

        if (request.HardDelete) _ctx.ResidenceTypes.Remove(entity);
        else { entity.IsDeleted = true; entity.IsActive = false; entity.LastModified = DateTime.UtcNow; }

        await _ctx.SaveChangesAsync(ct);
        return Success.Delete(entity.Id.ToString());
    }
}
