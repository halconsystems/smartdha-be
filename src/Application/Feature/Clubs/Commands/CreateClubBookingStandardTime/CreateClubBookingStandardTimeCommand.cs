using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace DHAFacilitationAPIs.Application.Feature.Clubs.Commands.CreateClubBookingStandardTime;
public record CreateClubBookingStandardTimeCommand() : IRequest<SuccessResponse<Guid>>
{
    public ClubBookingStandardTimeDto Dto { get; set; } = default!;
}

public class CreateClubBookingStandardTimeCommandHandler : IRequestHandler<CreateClubBookingStandardTimeCommand, SuccessResponse<Guid>>
{
    private readonly IOLMRSApplicationDbContext _ctx;

    public CreateClubBookingStandardTimeCommandHandler(IOLMRSApplicationDbContext ctx) => _ctx = ctx;

    public async Task<SuccessResponse<Guid>> Handle(CreateClubBookingStandardTimeCommand request, CancellationToken ct)
    {
        // optional: ensure no duplicate per club
        var exists = await _ctx.ClubBookingStandardTimes
            .AnyAsync(x => x.ClubId == request.Dto.ClubId && (x.IsDeleted == false || x.IsDeleted == null), ct);

        if (exists)
        {
            return new SuccessResponse<Guid>(
                Guid.Empty,
                "Club booking standard time already exists for this club."
            )
            {
                Status = StatusCodes.Status409Conflict
            };
        }

        var entity = new ClubBookingStandardTime
        {
            ClubId = request.Dto.ClubId,
            CheckInTime = request.Dto.CheckInTime,
            CheckOutTime = request.Dto.CheckOutTime,
            IsActive = true,
            IsDeleted = false,
            Created = DateTime.Now
        };

        await _ctx.ClubBookingStandardTimes.AddAsync(entity, ct);
        await _ctx.SaveChangesAsync(ct);

        return Success.Created(entity.Id);
    }
}
