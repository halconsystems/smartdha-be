using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.ResidenceType.Commands.DeleteResidenceType;
public record DeleteResidenceTypeCommand(Guid Id)
    : IRequest<SuccessResponse<string>>;

public class DeleteResidenceTypeCommandHandler : IRequestHandler<DeleteResidenceTypeCommand, SuccessResponse<string>>
{
    private readonly IOLMRSApplicationDbContext _ctx;
    public DeleteResidenceTypeCommandHandler(IOLMRSApplicationDbContext ctx) => _ctx = ctx;

    public async Task<SuccessResponse<string>> Handle(DeleteResidenceTypeCommand request, CancellationToken ct)
    {
        var entity = await _ctx.ResidenceTypes.FindAsync(new object?[] { request.Id }, ct);
        if (entity is null) throw new KeyNotFoundException("ResidenceType not found.");

        //Check for dependent rooms
        var hasRooms = await _ctx.Rooms
            .AnyAsync(r => r.ResidenceTypeId == request.Id && r.IsDeleted == false, ct);

        if (hasRooms)
            throw new InvalidOperationException(
                "Cannot delete this residence type because it is attached to one or more rooms."
            );


        entity.IsDeleted = true; entity.IsActive = false; entity.LastModified = DateTime.Now; 

        await _ctx.SaveChangesAsync(ct);
        return Success.Delete(entity.Id.ToString());
    }
}
