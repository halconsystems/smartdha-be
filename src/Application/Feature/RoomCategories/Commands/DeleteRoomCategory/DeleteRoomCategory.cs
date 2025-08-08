using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.RoomCategories.Commands.DeleteRoomCategory;
public record DeleteRoomCategoryCommand(Guid Id, bool HardDelete = false)
    : IRequest<SuccessResponse<string>>;

public class DeleteRoomCategoryCommandHandler : IRequestHandler<DeleteRoomCategoryCommand, SuccessResponse<string>>
{
    private readonly IOLMRSApplicationDbContext _ctx;
    public DeleteRoomCategoryCommandHandler(IOLMRSApplicationDbContext ctx) => _ctx = ctx;

    public async Task<SuccessResponse<string>> Handle(DeleteRoomCategoryCommand request, CancellationToken ct)
    {
        var entity = await _ctx.RoomCategories.FindAsync(new object?[] { request.Id }, ct);
        if (entity is null) throw new KeyNotFoundException("RoomCategory not found.");

        if (request.HardDelete) _ctx.RoomCategories.Remove(entity);
        else { entity.IsDeleted = true; entity.IsActive = false; entity.LastModified = DateTime.Now; }

        await _ctx.SaveChangesAsync(ct);
        return Success.Delete(entity.Id.ToString());
    }
}
