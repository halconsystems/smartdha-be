using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.OrderDispatch.Command.Delivery;
public record OrdersProcessAtShopCommand(
    Guid OrderId,
    bool WashProcess,
    bool WashingComplete
) : IRequest<Guid>;


public class OrdersProcessAtShopCommandHandler
    : IRequestHandler<OrdersProcessAtShopCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IOrderRealTime _realtime;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly INotificationService _notificationService;
    private readonly IVehicleLocationStore _vehicleLocationStore;
    private readonly ILaundrySystemDbContext _laundrySystemDb;

    public OrdersProcessAtShopCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IOrderRealTime realtime,
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

    public async Task<Guid> Handle(OrdersProcessAtShopCommand request, CancellationToken ct)
    {
        await using var transaction = await _laundrySystemDb.Database.BeginTransactionAsync(ct);
        try
        {
            // Get Panic
            var order = await _laundrySystemDb.Orders
                .Include(x => x.OrderType)
                .FirstOrDefaultAsync(p => p.Id == request.OrderId, ct)
                ?? throw new NotFoundException(nameof(Domain.Entities.LMS.Orders), request.OrderId.ToString());

            var DeliveryDetails = await _laundrySystemDb.DeliveryDetails
                .FirstOrDefaultAsync(x => x.OrderId == order.Id, ct)
                ?? throw new NotFoundException(nameof(Domain.Entities.LMS.DeliveryDetails), request.OrderId.ToString());

            Domain.Entities.LMS.OrderDispatch? dispatch = null;
            // Fetch dispatch
            dispatch = await _laundrySystemDb.OrderDispatches
                .Include(d => d.Orders)
                .FirstOrDefaultAsync(d => d.OrderId == request.OrderId, ct)
                ?? throw new NotFoundException("Dispatch not found", request.OrderId.ToString());

            if (request.WashProcess)
            {
                order.WashnPressProcessAt = DateTime.Now;
                order.OrderStatus = OrderStatus.InProgress;

                dispatch.Status = OrderDispatchStatus.WashnPressProcess;

            }
            else
            {
                order.ParcelReadyAt = DateTime.Now;
                order.OrderStatus = OrderStatus.InProgress;

                dispatch.Status = OrderDispatchStatus.ParcelReady;

            }


            var id = order.UserId.ToString();
            var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.Id == id, ct);

            await _context.SaveChangesAsync(ct);
            return dispatch.Id;
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(ct);
            throw new InvalidOperationException(e.ToString());
        }

    }
}





