using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryCategory;
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
            .Include(x => x.Shops)
            .AsNoTracking()
            .FirstOrDefaultAsync(ct);

        if(orders == null)
            throw new UnauthorizedAccessException("Order Not Found.");

        var ordersumarries = await _context.OrderSummaries.Where(x => x.OrderId == orders.Id)
            .AsNoTracking().ToListAsync();

        var OrderShops = await _context.Shops.Where(x => x.Id == orders.ShopId).AsNoTracking().FirstOrDefaultAsync();

        var OrderspaymentsDT = await _context.PaymentDTSettings.Where(x => x.OrderId == orders.Id).AsNoTracking().ToListAsync();

        var laundryItems = await _context.LaundryItems.Where(x => ordersumarries.Select(x => x.ItemId).Contains(x.Id)).AsNoTracking().ToListAsync();

        var laundryCategorires = await _context.LaundryCategories.Where(x => laundryItems.Select(x => x.CategoryId).Contains(x.Id)).AsNoTracking().ToListAsync();

        var DeliveryDetails = await _context.DeliveryDetails.Where(x => x.OrderId == orders.Id).AsNoTracking().FirstOrDefaultAsync();
        
        var OrderDispatch = await _context.OrderDispatches.Where(x => x.OrderId == orders.Id).AsNoTracking().FirstOrDefaultAsync();

        if (DeliveryDetails == null)
            throw new KeyNotFoundException("Delivery Not Found.");


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
            OrderStatus = orders.OrderStatus,
            OrderDispatches = OrderDispatch,
            Shops = OrderShops,
            LaundryCategories = laundryCategorires
                .Select(x => new LaundryCategoryDTO
                {
                    Id = x.Id,
                    Name = x.Name,
                    DisplayName = x.DisplayName,
                }).ToList(),
            LaundryItems = laundryItems
                .Select(x => new LaundryItemsDTO
                {
                    Id = x.Id,
                    Name = x.Name,
                    ItemPrice = x.ItemPrice,
                    CategoryID = x.CategoryId.ToString()
                })
        .ToList()
        }; 
        return result;
    }
}






