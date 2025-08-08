using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.Room.Commands.CreateRoom;
public record CreateRoomCommand(
    Guid ClubId,
    Guid RoomCategoryId,
    Guid ResidenceTypeId,
    string No,
    string? Name
) : IRequest<SuccessResponse<string>>;

public class CreateRoomCommandHandler : IRequestHandler<CreateRoomCommand, SuccessResponse<string>>
{
    private readonly IOLMRSApplicationDbContext _ctx;

    public CreateRoomCommandHandler(IOLMRSApplicationDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<SuccessResponse<string>> Handle(CreateRoomCommand request, CancellationToken ct)
    {
        var entity = new Domain.Entities.Room
        {
            ClubId = request.ClubId,
            RoomCategoryId = request.RoomCategoryId,
            ResidenceTypeId = request.ResidenceTypeId,
            No = request.No,
            Name = request.Name,
            IsActive = true,
            IsDeleted = false,
            Created = DateTime.UtcNow
        };

        _ctx.Rooms.Add(entity);
        await _ctx.SaveChangesAsync(ct);

        return Success.Created(entity.Id.ToString());
    }
}

