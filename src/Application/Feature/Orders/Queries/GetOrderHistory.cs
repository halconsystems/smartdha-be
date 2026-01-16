using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryItems;

namespace DHAFacilitationAPIs.Application.Feature.Orders.Queries;

public record GetOrderHistoryIdQuery(Guid OrderId) : IRequest<OrderHistoryDTO>;
public class GetOrderHistoryIdQueryHandler : IRequestHandler<GetOrderHistoryIdQuery, OrderHistoryDTO>
{
    private readonly ILaundrySystemDbContext _context;

    private readonly ICurrentUserService _currentUser;

    public GetOrderHistoryIdQueryHandler(ILaundrySystemDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<OrderHistoryDTO> Handle(GetOrderHistoryIdQuery request, CancellationToken ct)
    {
        var userID = _currentUser.UserId.ToString();
        if (string.IsNullOrWhiteSpace(userID))
            throw new UnauthorizedAccessException("User not authenticated.");


        var orders = await _context.Orders
            .Where(x => x.Id == request.OrderId && x.UserId == userID)
            .Include(x => x.LaundryPackaging)
            .Include(x => x.LaundryService)
            .AsNoTracking()
            .FirstOrDefaultAsync(ct);

        if(orders == null)
            throw new UnauthorizedAccessException("Order Not Found.");

        var ordersumarries = await _context.OrderSummaries.Where(x => x.OrderId == orders.Id)
            .Include(x => x.LaundryItems!)
                .ThenInclude(x => x.LaundryCategory)
            .AsNoTracking().ToListAsync();

        var OrderspaymentsDT = await _context.PaymentDTSettings.Where(x => x.OrderId == orders.Id).AsNoTracking().ToListAsync();

        var DeliveryDetails = await _context.DeliveryDetails.Where(x => x.OrderId == orders.Id).AsNoTracking().FirstOrDefaultAsync();

        if (DeliveryDetails == null)
            throw new KeyNotFoundException("Delivery Not Found.");

        var confirmedOrder = await _context.ConfirmedOrders.Where(x => x.OrderId == orders.Id)
            .Include(x => x.Shops)
            .AsNoTracking().FirstOrDefaultAsync();


        var result = new OrderHistoryDTO
        {
            Id = orders.Id,
            OrderUniqueId = orders.UniqueFormID,
            ServiceName = orders.LaundryService?.DisplayName,
            OrderDate = orders.Created,
            OrderType = orders.OrderType,
            OrderSummaries = ordersumarries,
            PaymentDTSettings = OrderspaymentsDT,
            PackageName = orders.LaundryPackaging?.DisplayName,
            DeliveryDetails = DeliveryDetails,
        };

        return result;
    }
}






