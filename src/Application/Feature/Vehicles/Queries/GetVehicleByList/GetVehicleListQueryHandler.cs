using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DHAFacilitationAPIs.Application.Feature.Vehicles.Queries.GetVehicleByList;

public class GetVehicleListQueryHandler
    : IRequestHandler<GetVehicleListQuery, Result<List<GetVehicleListResponse>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ISmartdhaDbContext _smartdhaDbContext;
    private readonly IUser _loggedInUser;

    public GetVehicleListQueryHandler(
        IApplicationDbContext context,
        ISmartdhaDbContext smartdhaDbContext,
        IUser loggedInUser)
    {
        _context = context;
        _smartdhaDbContext = smartdhaDbContext;
        _loggedInUser = loggedInUser;
    }

    public async Task<Result<List<GetVehicleListResponse>>> Handle(
        GetVehicleListQuery request,
        CancellationToken cancellationToken)
    {
        var vehicles = await _smartdhaDbContext.Vehicles.Where(v => v.IsActive == true
                && v.CreatedBy == request.Id)
    .ToListAsync(cancellationToken);


        if (!vehicles.Any())
            return Result<List<GetVehicleListResponse>>
                .Failure(new[] { "No vehicles found" });

        var response = vehicles.Select(vehicle => new GetVehicleListResponse
        {
            LicenseNo = vehicle.LicenseNo,
            License = vehicle.License ?? string.Empty,
            Year = vehicle.Year ?? string.Empty,
            Color = vehicle.Color ?? string.Empty,
            Make = vehicle.Make ?? string.Empty,
            Model = vehicle.Model ?? string.Empty,
            ETagId = vehicle.ETagId,
            ValidTo = vehicle.ValidTo,
            ValidFrom = vehicle.ValidFrom
        }).ToList();

        return Result<List<GetVehicleListResponse>>.Success(response);
    }
}
