using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Queries.GetAllPanicResponders;

public record GetAllPanicRespondersQuery(Guid? EmergencyTypeId = null) : IRequest<SuccessResponse<List<PanicResponderDto>>>;

public class GetAllPanicRespondersQueryHandler : IRequestHandler<GetAllPanicRespondersQuery, SuccessResponse<List<PanicResponderDto>>>
{
    private readonly IApplicationDbContext _ctx;

    public GetAllPanicRespondersQueryHandler(IApplicationDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<SuccessResponse<List<PanicResponderDto>>> Handle(GetAllPanicRespondersQuery request, CancellationToken ct)
    {
        var query = _ctx.PanicResponders
            .Include(x => x.EmergencyType)
            .Where(x => x.IsActive == true && x.IsDeleted == false)
            .AsQueryable();

        if (request.EmergencyTypeId.HasValue)
            query = query.Where(x => x.EmergencyTypeId == request.EmergencyTypeId.Value);

        var responders = await query
            .Select(x => new PanicResponderDto
            {
                Id = x.Id,
                Name = x.Name,
                CNIC = x.CNIC,
                PhoneNumber = x.PhoneNumber,
                Email = x.Email,
                Gender = x.Gender,
                EmergencyTypeId = x.EmergencyTypeId,
                EmergencyTypeName = x.EmergencyType.Name
            })
            .OrderBy(x => x.Name)
            .ToListAsync(ct);

        return Success.Ok(responders);
    }
}
