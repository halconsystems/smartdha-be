using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryCategory;

namespace DHAFacilitationAPIs.Application.Feature.Orders.Queries;


public record GetAllOrderListQuery : IRequest<List<OrderDTO>>;
public class GetAllOrderListQueryHandler : IRequestHandler<GetAllOrderListQuery, List<OrderDTO>>
{
    private readonly ILaundrySystemDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetAllOrderListQueryHandler(ILaundrySystemDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<OrderDTO>> Handle(GetAllOrderListQuery request, CancellationToken ct)
    {
        var userID = _currentUser.UserId.ToString();
        if (string.IsNullOrWhiteSpace(userID))
            throw new UnauthorizedAccessException("User not authenticated.");

        var orders = await _context.Orders.Where(x => x.UserId == userID)
            .Include(x => x.LaundryPackaging)
            .Include(x => x.LaundryService)
            .Include(x => x.Shops)
            .AsNoTracking()
            .ToListAsync(ct);

        var ordersumarries = await _context.OrderSummaries.Where(x => orders.Select(x => x.Id).Contains(x.OrderId)).AsNoTracking().ToListAsync(ct);

        var OrderspaymentsDT = await _context.PaymentDTSettings.Where(x => orders.Select(x => x.Id).Contains(x.OrderId)).AsNoTracking().ToListAsync(ct);

        var DeliveryDetails = await _context.DeliveryDetails.Where(x => orders.Select(x => x.Id).Contains(x.OrderId)).AsNoTracking().ToListAsync(ct);

        if(DeliveryDetails == null)
            throw new KeyNotFoundException("Delivery Not Found.");

        var confirmedOrder = await _context.ConfirmedOrders.Where(x => orders.Select(x => x.Id).Contains(x.OrderId))
            .AsNoTracking().FirstOrDefaultAsync(ct);


        var result = orders.Select(x => new OrderDTO
        {
            Id = x.Id,
            ServiceName = x.LaundryService?.DisplayName,
            PackageName = x.LaundryPackaging?.DisplayName,
            OrderDate = x.Created,
            TotalPrice = DeliveryDetails.FirstOrDefault(d => d.OrderId == x.Id)?.Total.ToString(),
            ItemCount = ordersumarries.Sum(x => Convert.ToDecimal(x.ItemCount)).ToString(),
            PaymentMethod = DeliveryDetails.FirstOrDefault(d => d.OrderId == x.Id)?.PaymentMethod,
            OrderStatus = x.OrderStatus,
            ShopName = x.Shops?.DisplayName,
            OrderType = x.OrderType,
            OrderUniqueId = x.UniqueFormID
        }).ToList();

        return result;
    }
}




