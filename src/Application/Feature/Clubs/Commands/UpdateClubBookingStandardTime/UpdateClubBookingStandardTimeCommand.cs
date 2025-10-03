using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Clubs.Commands.CreateClubBookingStandardTime;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.Clubs.Commands.UpdateClubBookingStandardTimeCommand;

public class UpdateClubBookingStandardTimeCommand() : IRequest<SuccessResponse<Guid>>
{
    [Required] public Guid Id { get; set; }
    public TimeOnly? CheckInTime { get; set; }
    public TimeOnly? CheckOutTime { get; set; }
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
        if (request.CheckInTime.HasValue)
            entity.CheckInTime = request.CheckInTime.Value;

        if (request.CheckOutTime.HasValue)
            entity.CheckOutTime = request.CheckOutTime.Value;

        await _ctx.SaveChangesAsync(ct);

        return Success.Ok(entity.Id);
    }
}
