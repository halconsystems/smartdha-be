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
    private readonly ICurrentUserService _currentUser;
    private readonly ILaundrySystemDbContext _laundrySystemDb;

    public OrdersProcessAtShopCommandHandler(
        ICurrentUserService currentUser,
        ILaundrySystemDbContext laundrySystemDb)
    {
        _currentUser = currentUser;
        _laundrySystemDb = laundrySystemDb;
    }

    public async Task<Guid> Handle(OrdersProcessAtShopCommand request, CancellationToken ct)
    {
        try
        {
            // Get Panic
            var order = await _laundrySystemDb.Orders
                .FirstOrDefaultAsync(p => p.Id == request.OrderId, ct)
                ?? throw new NotFoundException(nameof(Domain.Entities.LMS.Orders), request.OrderId.ToString());

            var DeliveryDetails = await _laundrySystemDb.DeliveryDetails
                .FirstOrDefaultAsync(x => x.OrderId == order.Id, ct)
                ?? throw new NotFoundException(nameof(Domain.Entities.LMS.DeliveryDetails), request.OrderId.ToString());

            Domain.Entities.LMS.OrderDispatch? dispatch = null;
            // Fetch dispatch
            dispatch = await _laundrySystemDb.OrderDispatches
                .Include(d => d.Orders)
                .FirstOrDefaultAsync(d => d.OrdersId == request.OrderId && (d.Status == OrderDispatchStatus.DeliveredToShop || d.Status == OrderDispatchStatus.WashnPressProcess), ct)
                ?? throw new NotFoundException("Dispatch not Reached On Shop Yet", request.OrderId.ToString());

            if (request.WashProcess)    
            {
                order.WashnPressProcessAt = DateTime.Now;
                order.OrderStatus = OrderStatus.InProgress;

                dispatch.Status = OrderDispatchStatus.WashnPressProcess;

            }
            else if(dispatch.Status == OrderDispatchStatus.WashnPressProcess)
            {
                order.ParcelReadyAt = DateTime.Now;
                order.OrderStatus = OrderStatus.InProgress;

                dispatch.Status = OrderDispatchStatus.ParcelReady;

            }


            await _laundrySystemDb.SaveChangesAsync(ct);
            return dispatch.Id;
        }
        catch (Exception e)
        {
            throw new InvalidOperationException(e.ToString());
        }

    }
}





