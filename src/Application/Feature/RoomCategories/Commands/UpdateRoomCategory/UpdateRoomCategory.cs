using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.RoomCategories.Commands.UpdateRoomCategory;
public record UpdateRoomCategoryCommand(Guid Id, string Name, string? Description, bool? IsActive)
    : IRequest<SuccessResponse<string>>;

public class UpdateRoomCategoryCommandHandler : IRequestHandler<UpdateRoomCategoryCommand, SuccessResponse<string>>
{
    private readonly IOLMRSApplicationDbContext _ctx;
    public UpdateRoomCategoryCommandHandler(IOLMRSApplicationDbContext ctx) => _ctx = ctx;

    public async Task<SuccessResponse<string>> Handle(UpdateRoomCategoryCommand request, CancellationToken ct)
    {
        var entity = await _ctx.RoomCategories.FindAsync(new object?[] { request.Id }, ct);
        if (entity is null) throw new KeyNotFoundException("RoomCategory not found.");

        entity.Name = request.Name;
        entity.Description = request.Description;
        if (request.IsActive.HasValue) entity.IsActive = request.IsActive;
        entity.LastModified = DateTime.Now;

        await _ctx.SaveChangesAsync(ct);
        return SuccessResponse<string>.FromMessage("Updated", "RoomCategory updated.");
    }
}
