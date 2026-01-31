using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.CBMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.ClubFacilities.Commands.AddFacilityToClub;
public record AddFacilityToClubCommand(
    Guid ClubId,
    Guid FacilityId,
    decimal? Price,
    bool IsAvailable,
    bool IsPriceVisible,
    bool HasAction,
    string? ActionName,
    string? ActionType
) : IRequest<ApiResult<Guid>>;
public class AddFacilityToClubCommandHandler
    : IRequestHandler<AddFacilityToClubCommand, ApiResult<Guid>>
{
    private readonly IOLMRSApplicationDbContext _ctx;

    public AddFacilityToClubCommandHandler(IOLMRSApplicationDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<ApiResult<Guid>> Handle(
        AddFacilityToClubCommand request,
        CancellationToken ct)
    {
        var clubFacility = new ClubFacility
        {
            ClubId = request.ClubId,
            FacilityId = request.FacilityId,
            Price = request.Price,
            IsAvailable = request.IsAvailable,
            IsPriceVisible = request.IsPriceVisible,
            HasAction = request.HasAction,
            ActionName = request.ActionName,
            ActionType = request.ActionType
        };

        _ctx.ClubFacilities.Add(clubFacility);
        await _ctx.SaveChangesAsync(ct);

        return ApiResult<Guid>.Ok(clubFacility.Id);
    }
}

