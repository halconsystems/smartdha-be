using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Entities.LMS;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using NotFoundException = DHAFacilitationAPIs.Application.Common.Exceptions.NotFoundException;

namespace DHAFacilitationAPIs.Application.Feature.ShopVehicles.Command;


public record AssignDriverToShopCommand(
    Guid ShopId,
    Guid UserId
) : IRequest<SuccessResponse<string>>;


public class AssignDriverToShopCommandHandler
    : IRequestHandler<AssignDriverToShopCommand, SuccessResponse<string>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILaundrySystemDbContext _laundrySystemDb;
    private readonly ICurrentUserService _currentUser;
    private readonly IPanicRealtime _realtime;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly INotificationService _notificationService;
    private readonly IVehicleLocationStore _vehicleLocationStore;

    public AssignDriverToShopCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IPanicRealtime realtime,
        UserManager<ApplicationUser> userManager,
        INotificationService notificationService,
        IVehicleLocationStore vehicleLocationStore,
        ILaundrySystemDbContext laundrySystemDb)
    {
        _context = context;
        _currentUser = currentUser;
        _realtime = realtime;
        _userManager = userManager;
        _notificationService = notificationService;
        _vehicleLocationStore = vehicleLocationStore;
        _laundrySystemDb = laundrySystemDb;
    }

    public async Task<SuccessResponse<string>> Handle(AssignDriverToShopCommand request, CancellationToken ct)
    {
        string UsedId = _currentUser.UserId.ToString();

        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId.ToString(), ct)
            ?? throw new NotFoundException("User not found.");

        var shopDetail = await _laundrySystemDb.Shops
            .FirstOrDefaultAsync(x => x.Id == request.ShopId, ct)
            ?? throw new NotFoundException("Shop Not Found");

        var entity = new ShopDrivers
        {
            ShopId = request.ShopId,
            DriverId = request.UserId
        };
        _laundrySystemDb.ShopDrivers.Add(entity);
        await _context.SaveChangesAsync(ct);

        return Success.Created(entity.Id.ToString());
    }
}



