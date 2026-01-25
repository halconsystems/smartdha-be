using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.ClubServices.Command;

public record DeleteClubServiceProcessCommand(Guid Id)
    : IRequest<ApiResult<bool>>;
public class DeleteClubServiceProcessCommandHandler
    : IRequestHandler<DeleteClubServiceProcessCommand, ApiResult<bool>>
{
    private readonly IOLMRSApplicationDbContext _db;

    public DeleteClubServiceProcessCommandHandler(IOLMRSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<bool>> Handle(
        DeleteClubServiceProcessCommand request,
        CancellationToken ct)
    {
        var entity = await _db.Set<Domain.Entities.CBMS.ClubServiceProcess>()
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (entity == null)
            return ApiResult<bool>.Fail("Club Service Process not found.");

        entity.IsDeleted = true;
        entity.IsActive = false;

        await _db.SaveChangesAsync(ct);

        return ApiResult<bool>.Ok(true, "Club Service Process deleted successfully.");
    }
}

