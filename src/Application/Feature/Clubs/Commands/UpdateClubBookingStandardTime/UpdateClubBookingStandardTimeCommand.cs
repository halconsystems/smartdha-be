using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Clubs.Commands.CreateClubBookingStandardTime;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.Clubs.Commands.UpdateClubBookingStandardTimeCommand;

public class UpdateClubBookingStandardTimeCommand : IRequest<SuccessResponse<Guid>>
{
    public Guid Id { get; set; }
    public ClubBookingStandardTimeDto Dto { get; set; } = default!;
}

public class UpdateClubBookingStandardTimeCommandHandler : IRequestHandler<UpdateClubBookingStandardTimeCommand, SuccessResponse<Guid>>
{
    private readonly IOLMRSApplicationDbContext _ctx;

    public UpdateClubBookingStandardTimeCommandHandler(IOLMRSApplicationDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<SuccessResponse<Guid>> Handle(UpdateClubBookingStandardTimeCommand request, CancellationToken ct)
    {
        var entity = await _ctx.ClubBookingStandardTimes
            .FirstOrDefaultAsync(x => x.Id == request.Id && (x.IsDeleted == false || x.IsDeleted == null), ct);

        if (entity == null)
            throw new KeyNotFoundException($"Standard time with Id {request.Id} not found.");

        // Do NOT allow ClubId change
        entity.CheckInTime = request.Dto.CheckInTime;
        entity.CheckOutTime = request.Dto.CheckOutTime;

        await _ctx.SaveChangesAsync(ct);

        return Success.Ok(entity.Id);
    }
}
