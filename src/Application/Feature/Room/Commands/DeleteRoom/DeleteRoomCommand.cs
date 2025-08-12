using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.Room.Commands.DeleteRoom;
public class DeleteRoomCommand : IRequest<SuccessResponse<string>>
{
    public Guid Id { get; set; }
    public bool HardDelete { get; set; } = false;
}

public class DeleteRoomCommandHandler : IRequestHandler<DeleteRoomCommand, SuccessResponse<string>>
{
    private readonly IOLMRSApplicationDbContext _ctx;

    public DeleteRoomCommandHandler(IOLMRSApplicationDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<SuccessResponse<string>> Handle(DeleteRoomCommand request, CancellationToken ct)
    {
        var entity = await _ctx.Rooms.FindAsync(new object?[] { request.Id }, ct);

        if (entity is null)
            throw new KeyNotFoundException("Room not found.");

        if (request.HardDelete)
        {
            _ctx.Rooms.Remove(entity);
        }
        else
        {
            entity.IsDeleted = true;         // assuming this exists on the Room entity
            entity.IsActive = false;         // assuming this exists too
            entity.LastModified = DateTime.Now;
        }

        await _ctx.SaveChangesAsync(ct);

        return Success.Delete(entity.Id.ToString());
    }
}
