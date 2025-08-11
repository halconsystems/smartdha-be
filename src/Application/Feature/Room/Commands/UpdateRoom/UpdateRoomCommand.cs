using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.Room.Commands.UpdateRoom;
public class UpdateRoomCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public Guid ClubId { get; set; }
    public Guid RoomCategoryId { get; set; }
    public Guid ResidenceTypeId { get; set; }

    [MaxLength(50)]
    public string No { get; set; } = default!;

    [MaxLength(100)]
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool IsGloballyAvailable { get; set; }
}

public class UpdateRoomCommandHandler : IRequestHandler<UpdateRoomCommand, bool>
{
    private readonly IOLMRSApplicationDbContext _context;

    public UpdateRoomCommandHandler(IOLMRSApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateRoomCommand request, CancellationToken cancellationToken)
    {
        var room = await _context.Rooms.FindAsync(new object[] { request.Id }, cancellationToken);

        if (room == null)
            return false;

        room.ClubId = request.ClubId;
        room.RoomCategoryId = request.RoomCategoryId;
        room.ResidenceTypeId = request.ResidenceTypeId;
        room.No = request.No;
        room.Name = request.Name;
        room.Description = request.Description;
        room.IsGloballyAvailable = request.IsGloballyAvailable;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

