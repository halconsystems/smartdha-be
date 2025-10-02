using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.Clubs.Commands.DeleteClubBookingStandardTimeCommand;
public class DeleteClubBookingStandardTimeCommand : IRequest<SuccessResponse<Guid>>
{
    public Guid Id { get; set; }
}

public class DeleteClubBookingStandardTimeCommandHandler
    : IRequestHandler<DeleteClubBookingStandardTimeCommand, SuccessResponse<Guid>>
{
    private readonly IOLMRSApplicationDbContext _ctx;

    public DeleteClubBookingStandardTimeCommandHandler(IOLMRSApplicationDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<SuccessResponse<Guid>> Handle(DeleteClubBookingStandardTimeCommand request, CancellationToken ct)
    {
        var entity = await _ctx.ClubBookingStandardTimes
            .FirstOrDefaultAsync(x => x.Id == request.Id && (x.IsDeleted == false || x.IsDeleted == null), ct);

        if (entity == null)
            throw new KeyNotFoundException($"Standard time with Id {request.Id} not found.");

        entity.IsActive = false;
        entity.IsDeleted = true;
        entity.LastModified = DateTime.Now;

        await _ctx.SaveChangesAsync(ct);

        return Success.Ok(entity.Id);
    }
}
