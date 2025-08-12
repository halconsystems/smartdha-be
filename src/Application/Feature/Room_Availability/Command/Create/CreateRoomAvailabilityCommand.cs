using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Application.Feature.Room_Availability.Command.Create;
public class CreateRoomAvailabilityCommand : IRequest<SuccessResponse<Guid>>
{
    [Required]
    public Guid RoomId { get; set; }

    [Required]
    public DateTime FromDate { get; set; }

    [Required]
    public DateTime ToDate { get; set; }

    [Required]
    public AvailabilityAction Action { get; set; }

    public string? Reason { get; set; }
}

public class CreateRoomAvailabilityCommandHandler : IRequestHandler<CreateRoomAvailabilityCommand, SuccessResponse<Guid>>
{
    private readonly IOLMRSApplicationDbContext _ctx;

    public CreateRoomAvailabilityCommandHandler(IOLMRSApplicationDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<SuccessResponse<Guid>> Handle(CreateRoomAvailabilityCommand request, CancellationToken ct)
    {
        // Optional: Validate Room exists
        var roomExists = await _ctx.Rooms.AnyAsync(r => r.Id == request.RoomId, ct);
        if (!roomExists)
            throw new KeyNotFoundException("Room not found.");

        var entity = new RoomAvailability
        {
            Id = Guid.NewGuid(),
            RoomId = request.RoomId,
            FromDate = request.FromDate,
            ToDate = request.ToDate,
            Action = request.Action,
            Reason = request.Reason,
            Created = DateTime.UtcNow
        };

        await _ctx.RoomAvailabilities.AddAsync(entity, ct);
        await _ctx.SaveChangesAsync(ct);

        return Success.Created(entity.Id);
    }
}


