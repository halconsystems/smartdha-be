using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.Clubs.Commands.UpdateClub;
public record UpdateClubCommand(Guid Id, string Name, string DisplayName, string? Description, string? Location, string? ContactNumber, string? AccountNo, string? AccountNoAccronym, bool? IsActive, string? Email)
    : IRequest<SuccessResponse<string>>;

public class UpdateClubCommandHandler : IRequestHandler<UpdateClubCommand, SuccessResponse<string>>
{
    private readonly ICBMSApplicationDbContext _ctx;
    public UpdateClubCommandHandler(ICBMSApplicationDbContext ctx) => _ctx = ctx;

    public async Task<SuccessResponse<string>> Handle(UpdateClubCommand request, CancellationToken ct)
    {
        var entity = await _ctx.Clubs.FindAsync(new object?[] { request.Id }, ct);
        if (entity is null) throw new KeyNotFoundException("Club not found.");

        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.DisplayName = request.DisplayName;
        entity.Location = request.Location;
        entity.ContactNumber = request.ContactNumber;
        entity.AccountNo = request.AccountNo;
        entity.AccountNoAccronym = request.AccountNoAccronym;
        if (request.IsActive.HasValue) entity.IsActive = request.IsActive;
        entity.LastModified = DateTime.Now;
        entity.Email = request.Email;

        await _ctx.SaveChangesAsync(ct);
        return Success.Update(entity.Id.ToString());
    }
}
