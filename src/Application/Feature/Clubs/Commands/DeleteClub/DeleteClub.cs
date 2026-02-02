using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.Clubs.Commands.DeleteClub;
public record DeleteClubCommand(Guid Id) : IRequest<SuccessResponse<string>>;

public class DeleteClubCommandHandler : IRequestHandler<DeleteClubCommand, SuccessResponse<string>>
{
    private readonly ICBMSApplicationDbContext _ctx;
    public DeleteClubCommandHandler(ICBMSApplicationDbContext ctx) => _ctx = ctx;

    public async Task<SuccessResponse<string>> Handle(DeleteClubCommand request, CancellationToken ct)
    {
        var entity = await _ctx.Clubs.FindAsync(new object?[] { request.Id }, ct);
        if (entity is null) throw new KeyNotFoundException("Club not found.");

        
            entity.IsDeleted = true;
            entity.IsActive = false;
            entity.LastModified = DateTime.Now;
        

        await _ctx.SaveChangesAsync(ct);
        return Success.Delete(entity.Id.ToString());
    }
}

