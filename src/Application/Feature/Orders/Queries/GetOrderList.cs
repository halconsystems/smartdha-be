using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryCategory;

namespace DHAFacilitationAPIs.Application.Feature.Orders.Queries;


public record GetAllOrderListQuery : IRequest<List<OrderDTO>>;
public class GetAllOrderListQueryHandler : IRequestHandler<GetAllOrderListQuery, List<OrderDTO>>
{
    private readonly ILaundrySystemDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IApplicationDbContext _applicationDb;

    public GetAllOrderListQueryHandler(ILaundrySystemDbContext context, ICurrentUserService currentUser, IApplicationDbContext applicationDb)
    {
        _context = context;
        _currentUser = currentUser;
        _applicationDb = applicationDb;
    }

    public async Task<List<OrderDTO>> Handle(GetAllOrderListQuery request, CancellationToken ct)
    {
        List<Domain.Entities.LMS.Orders> orders = new List<Domain.Entities.LMS.Orders>();
        List<Domain.Entities.LMS.OrderSummary> ordersumarries = new List<Domain.Entities.LMS.OrderSummary>();
        List<Domain.Entities.LMS.PaymentDTSetting> OrderspaymentsDT = new List<Domain.Entities.LMS.PaymentDTSetting>();
        List<Domain.Entities.LMS.DeliveryDetails> DeliveryDetails = new List<Domain.Entities.LMS.DeliveryDetails>();
        var userID = _currentUser.UserId.ToString();
        if (string.IsNullOrWhiteSpace(userID))
            throw new UnauthorizedAccessException("User not authenticated.");

        var roles = await _applicationDb.AppUserRoles
            .Include(ur => ur.Role)
            .Where(ur => ur.UserId == userID)
            .Select(ur => ur.Role.Name)
            .ToListAsync(ct);

        bool isSuperAdmin = roles.Contains("Shop Owner") || roles.Contains("SuperAdministrator");

        if (isSuperAdmin)
        {
            orders = await _context.Orders
           .Include(x => x.LaundryPackaging)
           .Include(x => x.LaundryService)
           .Include(x => x.Shops)
           .AsNoTracking()
           .ToListAsync(ct);
        }
        else
        {
            orders = await _context.Orders.Where(x => x.UserId == userID)
                .Include(x => x.LaundryPackaging)
                .Include(x => x.LaundryService)
                .Include(x => x.Shops)
                .AsNoTracking()
                .ToListAsync(ct);
        }

        var OrderShop = await _context.Shops.Where(x => orders.Select(x => x.ShopId).Contains(x.Id)).AsNoTracking().ToListAsync(ct);

        ordersumarries = await _context.OrderSummaries.Where(x => orders.Select(x => x.Id).Contains(x.OrderId)).AsNoTracking().ToListAsync(ct);

        OrderspaymentsDT = await _context.PaymentDTSettings.Where(x => orders.Select(x => x.Id).Contains(x.OrderId)).AsNoTracking().ToListAsync(ct);

        DeliveryDetails = await _context.DeliveryDetails.Where(x => orders.Select(x => x.Id).Contains(x.OrderId)).AsNoTracking().ToListAsync(ct);

        if(DeliveryDetails == null)
            throw new KeyNotFoundException("Delivery Not Found.");

        //var confirmedOrder = await _context.ConfirmedOrders.Where(x => orders.Select(x => x.Id).Contains(x.OrderId))
        //    .AsNoTracking().FirstOrDefaultAsync(ct);


        var result = orders.Select(x => new OrderDTO
        {
            Id = x.Id,
            ServiceName = x.LaundryService?.DisplayName,
            PackageName = x.LaundryPackaging?.DisplayName,
            OrderDate = x.Created,
            TotalPrice = DeliveryDetails.FirstOrDefault(d => d.OrderId == x.Id)?.Total.ToString(),
            ItemCount = ordersumarries
                .Sum(x => decimal.TryParse(x.ItemCount, out var val) ? val : 0)
                .ToString(),
            PaymentMethod = DeliveryDetails.FirstOrDefault(d => d.OrderId == x.Id)?.PaymentMethod,
            OrderStatus = x.OrderStatus,
            ShopName = OrderShop?.FirstOrDefault(s => s.Id == x.ShopId)?.DisplayName,
            OrderType = x.OrderType,
            OrderUniqueId = x.UniqueFormID
        }).ToList();

        return result;
    }
}




