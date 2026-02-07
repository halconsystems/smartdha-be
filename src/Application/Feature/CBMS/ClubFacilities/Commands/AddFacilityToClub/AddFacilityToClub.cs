using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.CBMS;
using DHAFacilitationAPIs.Domain.Enums.CBMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.ClubFacilities.Commands.AddFacilityToClub;
public record AddFacilityToClubCommand(
    Guid ClubId,
    Guid FacilityId,
    decimal? Price,
    bool IsAvailable,
    bool IsPriceVisible,
    bool HasAction,
    FacilityActionType FacilityActionType,
    BookingMode BookingMode
) : IRequest<ApiResult<Guid>>;
public class AddFacilityToClubCommandHandler
    : IRequestHandler<AddFacilityToClubCommand, ApiResult<Guid>>
{
    private readonly ICBMSApplicationDbContext _ctx;

    public AddFacilityToClubCommandHandler(ICBMSApplicationDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<ApiResult<Guid>> Handle(
        AddFacilityToClubCommand request,
        CancellationToken ct)
    {

        var (actionName, actionType) = FacilityActionMapper.ToUiAndType(request.FacilityActionType);

        var clubFacility = new ClubFacility
        {
            ClubId = request.ClubId,
            FacilityId = request.FacilityId,
            Price = request.Price,
            IsAvailable = request.IsAvailable,
            IsPriceVisible = request.IsPriceVisible,
            HasAction = request.HasAction,
            ActionName = actionName,
            ActionType = actionType,
            FacilityActionType = request.FacilityActionType,
            BookingMode = request.BookingMode,
        };

        _ctx.ClubFacilities.Add(clubFacility);
        try
        {
            await _ctx.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            return ApiResult<Guid>.Fail($"Error adding facility to club: {ex.Message}");
        }
        return ApiResult<Guid>.Ok(clubFacility.Id);
    }
}

public static class FacilityActionMapper
{
    private static readonly Dictionary<FacilityActionType, (string Name, string Type)> Map =
    new()
    {
            { FacilityActionType.Book,        ("Book Now", "Book") },
            { FacilityActionType.Reserve,     ("Reserve", "Reserve") },
            { FacilityActionType.ContactUs,   ("Contact Us", "ContactUs") },
            { FacilityActionType.ViewDetails, ("View Detail", "ViewDetails") }
    };

    public static (string? ActionName, string? ActionType) ToUiAndType(
        FacilityActionType actionType)
    {
        if (actionType == FacilityActionType.None)
            return (null, null);

        return Map.TryGetValue(actionType, out var result)
            ? result
            : (null, null);
    }
}

