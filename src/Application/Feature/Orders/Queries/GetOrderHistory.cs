using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryCategory;
using DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryItems;
using DHAFacilitationAPIs.Application.Feature.OrderTaxDiscount.Queries;
using DHAFacilitationAPIs.Domain.Entities.LMS;

namespace DHAFacilitationAPIs.Application.Feature.Orders.Queries;

public record GetOrderHistoryIdQuery(Guid OrderId) : IRequest<OrderHistoryDTO>;
public class GetOrderHistoryIdQueryHandler : IRequestHandler<GetOrderHistoryIdQuery, OrderHistoryDTO>
{
    private readonly ILaundrySystemDbContext _context;

    private readonly ICurrentUserService _currentUser;
    private readonly IApplicationDbContext _applicationDb;

    public GetOrderHistoryIdQueryHandler(ILaundrySystemDbContext context, ICurrentUserService currentUser, IApplicationDbContext applicationDb)
    {
        _context = context;
        _currentUser = currentUser;
        _applicationDb = applicationDb;
    }

    public async Task<OrderHistoryDTO> Handle(GetOrderHistoryIdQuery request, CancellationToken ct)
    {
        Domain.Entities.LMS.Orders? orders = new Domain.Entities.LMS.Orders();
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
            .Where(x => x.Id == request.OrderId)
            .Include(x => x.LaundryPackaging)
            .Include(x => x.LaundryService)
            .Include(x => x.Shops)
            .FirstOrDefaultAsync(ct);
        }
        else
        {
            orders = await _context.Orders.Where(x => x.UserId == userID)
            .Where(x => x.Id == request.OrderId)
            .Include(x => x.LaundryPackaging)
            .Include(x => x.LaundryService)
            .Include(x => x.Shops)
            .FirstOrDefaultAsync(ct);
        }


        if (orders == null)
            throw new UnauthorizedAccessException("Order Not Found.");

        var ordersumarries = await _context.OrderSummaries.Where(x => x.OrderId == orders.Id)
            .AsNoTracking().ToListAsync();

        var OrderShops = await _context.Shops.Where(x => x.Id == orders.ShopId).AsNoTracking().FirstOrDefaultAsync();

        var OrderService = await _context.LaundryServices.Where(x => x.Id == orders.ServiceId).AsNoTracking().FirstOrDefaultAsync();

        var OrderPackage = await _context.LaundryPackagings.Where(x => x.Id == orders.PackageId).AsNoTracking().FirstOrDefaultAsync();

        var OrderspaymentsDT = await _context.PaymentDTSettings.Where(x => x.OrderId == orders.Id).AsNoTracking().ToListAsync();

        var laundryItems = await _context.LaundryItems.Where(x => ordersumarries.Select(x => x.ItemId).Contains(x.Id)).AsNoTracking().ToListAsync();

        var laundryCategorires = await _context.LaundryCategories.Where(x => laundryItems.Select(x => x.CategoryId).Contains(x.Id)).AsNoTracking().ToListAsync();

        var DeliveryDetails = await _context.DeliveryDetails.Where(x => x.OrderId == orders.Id).AsNoTracking().FirstOrDefaultAsync();
        
        var OrderDispatch = await _context.OrderDispatches.Where(x => x.OrdersId == orders.Id).AsNoTracking().FirstOrDefaultAsync();

        var OrderDT = await _context.OrderDTSettings.Where(x => x.Name != Domain.Enums.Settings.Hanger).AsNoTracking().ToListAsync();

        if (DeliveryDetails == null)
            throw new KeyNotFoundException("Delivery Not Found.");


        var result = new OrderHistoryDTO
        {
            Id = orders.Id,
            OrderUniqueId = orders.UniqueFormID,
            ServiceName = OrderService?.DisplayName,
            TotalPrice = orders.CollectedAmount.ToString(),
            OrderDate = orders.Created,
            OrderType = orders.OrderType,
            OrderSummaries = ordersumarries,
            PaymentDTSettings = OrderspaymentsDT,
            PackageName = OrderPackage?.DisplayName,
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
        .ToList(),
            OrderDTs = OrderDT.Select(x => new OrderDTSettingDTO
            {
                DisplayName = x.DisplayName,
                DTCode = x.DTCode,
                IsDiscount = x.IsDiscount,
            }).ToList()
        }; 
        return result;
    }
}






