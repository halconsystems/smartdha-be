using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.CBMS.FacilityService.Queries;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Entities.CBMS;
using DHAFacilitationAPIs.Domain.Enums.PMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.ClubFacilities.Commands.UpdateFacilityToClub;

public record UpdateFacilityToClubCommand(
    Guid Id,
    Guid ClubId,
    Guid FacilityId,
    decimal? Price,
    bool IsAvailable,
    bool IsPriceVisible,
    bool HasAction,
    string? ActionName,
    string? ActionType
) : IRequest<ApiResult<Guid>>;
public class UpdateFacilityToClubCommandHandler
    : IRequestHandler<UpdateFacilityToClubCommand, ApiResult<Guid>>
{
    private readonly ICBMSApplicationDbContext _ctx;

    public UpdateFacilityToClubCommandHandler(ICBMSApplicationDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<ApiResult<Guid>> Handle(
        UpdateFacilityToClubCommand request,
        CancellationToken ct)
    {
        var existClubFacility = await _ctx.ClubFacilities.FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (existClubFacility == null) return ApiResult<Guid>.Fail("Club Facility not found.");

        existClubFacility.ClubId = request.ClubId;
        existClubFacility.FacilityId = request.FacilityId;
        existClubFacility.Price = request.Price;
        existClubFacility.IsAvailable = request.IsAvailable;
        existClubFacility.IsPriceVisible = request.IsPriceVisible;
        existClubFacility.HasAction = request.HasAction;
        existClubFacility.ActionName = request.ActionName;
        existClubFacility.ActionType = request.ActionType;

        try
        {
            await _ctx.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            return ApiResult<Guid>.Fail($"Error adding facility to club: {ex.Message}");
        }
        return ApiResult<Guid>.Ok(existClubFacility.Id,"Club Facility Updated Succesfully");
    }
}

