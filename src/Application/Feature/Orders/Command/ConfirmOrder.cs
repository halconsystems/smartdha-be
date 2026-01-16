//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using DHAFacilitationAPIs.Application.Common.Interfaces;
//using DHAFacilitationAPIs.Application.ViewModels;
//using DHAFacilitationAPIs.Domain.Entities.LMS;
//using DHAFacilitationAPIs.Domain.Enums;

//namespace DHAFacilitationAPIs.Application.Feature.Orders.Command;

//public class ConfirmOrderCommand :IRequest<SuccessResponse<string>>
//{
//    [Required]
//    public Guid OrderId { get; set; }
//    [Required]
//    public string PickupAddress { get; set; } = default!;

//    [Required]
//    public string DeliverAddress { get; set; } = default!;

//    [Required]
//    public Guid ShopId { get; set; } = default!;
//    public Domain.Entities.LMS.Shops? Shops { get; set; }
//    [Required]
//    public PaymentMethod PaymentMethod { get; set; }
//    public OrderStatus OrderStatus { get; set; } = OrderStatus.Confirmed;


//} 
//public class ConfirmOrderCommandHandler : IRequestHandler<ConfirmOrderCommand, SuccessResponse<string>>
//{
//    private readonly ILaundrySystemDbContext _context;
//    private readonly ICurrentUserService _currentUserService;

//    public ConfirmOrderCommandHandler(ILaundrySystemDbContext context, ISmsService otpService, ICurrentUserService currentUserService)
//    {
//        _context = context;
//        _currentUserService = currentUserService;
//    }

//    public async Task<SuccessResponse<string>> Handle(ConfirmOrderCommand command,CancellationToken ct)
//    {
//        try
//        {
//            var orderDetails = await _context.Orders.FirstOrDefaultAsync(x => x.Id == command.OrderId);
//            var DeliveryDetails = await _context.DeliveryDetails.FirstOrDefaultAsync(x => x.OrderId == command.OrderId);

//            if (orderDetails == null)
//                throw new KeyNotFoundException("Order not found.");
//            if(DeliveryDetails == null)
//                throw new KeyNotFoundException("Delivery Details not found.");

           

//            if(command.PaymentMethod == PaymentMethod.Cash)
//            {
//                DeliveryDetails.paidAmount = entity.CollectedAmount;
//                DeliveryDetails.RemainingBalance = DeliveryDetails.Total - DeliveryDetails.paidAmount;
//                await _context.SaveChangesAsync(ct);
//            }

//            return Success.Created(entity.Id.ToString());
//        }
//        catch (Exception)
//        {

//            throw;
//        }
//    }


//}
