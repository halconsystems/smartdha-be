using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.ClubServices.Command;

public record ActiveInActiveProcessCommand(Guid Id, bool Active)
    : IRequest<ApiResult<bool>>;
public class ActiveInActiveProcessCommandHandler
    : IRequestHandler<ActiveInActiveProcessCommand, ApiResult<bool>>
{
    private readonly IOLMRSApplicationDbContext _db;

    public ActiveInActiveProcessCommandHandler(IOLMRSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<bool>> Handle(
        ActiveInActiveProcessCommand request,
        CancellationToken ct)
    {
        var entity = await _db.Set<Domain.Entities.CBMS.ClubServiceProcess>()
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (entity == null)
            return ApiResult<bool>.Fail("Club Service Process not found.");

        entity.IsActive = request.Active;


        await _db.SaveChangesAsync(ct);
        if(request.Active == false)
        {
            return ApiResult<bool>.Ok(true, "Club Service Process InActive successfully.");
        }
        else
        {
            return ApiResult<bool>.Ok(true, $"Club Service Process Active successfully.");
        }
        
    }
}
