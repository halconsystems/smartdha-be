using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.RoomCategories.Commands.DeleteRoomCategory;
public record DeleteRoomCategoryCommand(Guid Id)
    : IRequest<SuccessResponse<string>>;

public class DeleteRoomCategoryCommandHandler : IRequestHandler<DeleteRoomCategoryCommand, SuccessResponse<string>>
{
    private readonly IOLMRSApplicationDbContext _ctx;
    public DeleteRoomCategoryCommandHandler(IOLMRSApplicationDbContext ctx) => _ctx = ctx;

    public async Task<SuccessResponse<string>> Handle(DeleteRoomCategoryCommand request, CancellationToken ct)
    {
        var entity = await _ctx.RoomCategories.FindAsync(new object?[] { request.Id }, ct);
        if (entity is null) throw new KeyNotFoundException("RoomCategory not found.");

        // Check dependencies: rooms that reference this category
        var hasRooms = await _ctx.Rooms
            .AnyAsync(r => r.RoomCategoryId == request.Id && r.IsDeleted == false, ct);

        if (hasRooms)
            throw new InvalidOperationException(
                "Cannot delete this room category because it is attached to one or more rooms"
            );

        entity.IsDeleted = true;
        entity.IsActive = false; 
        entity.LastModified = DateTime.Now;

        await _ctx.SaveChangesAsync(ct);
        return Success.Delete(entity.Id.ToString());
    }
}
