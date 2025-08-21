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
    int NormalOccupancy,
    int MaxExtraOccupancy,
    string? Name,
    string? Description
) : IRequest<SuccessResponse<string>>;

// Handler
public class CreateRoomCommandHandler : IRequestHandler<CreateRoomCommand, SuccessResponse<string>>
{
    private readonly IOLMRSApplicationDbContext _ctx;

    public CreateRoomCommandHandler(IOLMRSApplicationDbContext ctx) => _ctx = ctx;

    public async Task<SuccessResponse<string>> Handle(CreateRoomCommand request, CancellationToken ct)
    {
        var no = request.No.Trim();

        // Ensure uniqueness per Club: (ClubId, No)
        var exists = await _ctx.Rooms
            .AsNoTracking()
            .AnyAsync(r => r.ClubId == request.ClubId && r.No == no, ct);

        if (exists)
            throw new InvalidOperationException($"Room number '{no}' already exists for this club.");

        var entity = new Domain.Entities.Room
        {
            ClubId = request.ClubId,
            RoomCategoryId = request.RoomCategoryId,
            ResidenceTypeId = request.ResidenceTypeId,
            No = no,
            NormalOccupancy = request.NormalOccupancy,
            MaxExtraOccupancy = request.MaxExtraOccupancy,
            Name = request.Name?.Trim(),
            Description = request.Description,
            IsGloballyAvailable = true,   // default as per your model
        };

        _ctx.Rooms.Add(entity);
        await _ctx.SaveChangesAsync(ct);

        return Success.Created(entity.Id.ToString());
    }
}


