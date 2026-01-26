using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Application.Feature.Orders.Queries;

public record GetOrderListByShopIdQuery(Guid ShopiD) : IRequest<List<OrderDTO>>;
public class GetOrderListByShopIdQueryHandler : IRequestHandler<GetOrderListByShopIdQuery, List<OrderDTO>>
{
    private readonly ILaundrySystemDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetOrderListByShopIdQueryHandler(ILaundrySystemDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<OrderDTO>> Handle(GetOrderListByShopIdQuery request, CancellationToken ct)
    {


        var orders = _context.Orders.Where(x => x.ShopId == request.ShopiD)
            .Include(x => x.LaundryPackaging)
            .Include(x => x.LaundryService)
            .Include(x => x.Shops)
            .AsNoTracking()
            .ToList();

        var ordersumarries = await _context.OrderSummaries.Where(x => orders.Select(x => x.Id).Contains(x.OrderId)).AsNoTracking().ToListAsync();

        var OrderspaymentsDT = await _context.PaymentDTSettings.Where(x => orders.Select(x => x.Id).Contains(x.OrderId)).AsNoTracking().ToListAsync();

        var DeliveryDetails = await _context.DeliveryDetails.Where(x => orders.Select(x => x.Id).Contains(x.OrderId)).AsNoTracking().FirstOrDefaultAsync();

        if (DeliveryDetails == null)
            throw new KeyNotFoundException("Delivery Not Found.");

        var confirmedOrder = await _context.ConfirmedOrders.Where(x => orders.Select(x => x.Id).Contains(x.OrderId))
            .AsNoTracking().FirstOrDefaultAsync();


        var result = orders.Select(x => new OrderDTO
        {
            Id = x.Id,
            ServiceName = x.LaundryService?.DisplayName,
            PackageName = x.LaundryPackaging?.DisplayName,
            OrderDate = x.Created,
            TotalPrice = DeliveryDetails?.Total.ToString(),
            ItemCount = ordersumarries.Sum(x => Convert.ToDecimal(x.ItemCount)).ToString(),
            PaymentMethod = DeliveryDetails?.PaymentMethod,
            OrderStatus = x.OrderStatus,
            ShopName = x.Shops?.DisplayName,
            OrderType = x.OrderType,
            OrderUniqueId = x.UniqueFormID
        }).ToList();

        return result;
    }
}




