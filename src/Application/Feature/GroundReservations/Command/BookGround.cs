using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities.GBMS;
using DHAFacilitationAPIs.Domain.Entities.LMS;
using DHAFacilitationAPIs.Domain.Enums.GBMS;
using static Dapper.SqlMapper;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DHAFacilitationAPIs.Application.Feature.GroundReservations.Command;

public class GroundReserveCommand : IRequest<SuccessResponse<string>>
{
    public Guid GroundId { get; set; }
    public string? Description { get; set; }
    public DateTime BookingDate { get; set; }
    [Required]
    public List<Guid> Slots { get; set; } = default!;
    public string? BuketId { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
}
public class GroundReserveCommandHandler
    : IRequestHandler<GroundReserveCommand, SuccessResponse<string>>
{
    private readonly IOLMRSApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GroundReserveCommandHandler(
        IOLMRSApplicationDbContext context, ICurrentUserService currentUserService  )
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<SuccessResponse<string>> Handle(GroundReserveCommand request, CancellationToken ct)
    {
        try
        {
            var random = new Random();
            string code = random.Next(0, 1000000).ToString("D6");


            decimal taxex = 0;
            decimal discount = 0;
            decimal totalAmount = 0;
            decimal AmountToCollecot = 0;

            var today = DateTime.UtcNow.ToString("yyyyMMdd");

            var lastOrder = await _context.GroundBookings
            .Where(x => x.BookingCode.StartsWith($"GB-{today}"))
            .OrderByDescending(x => x.BookingCode)
            .Select(x => x.BookingCode)
            .FirstOrDefaultAsync();

            int nextNumber = 1;

            if (!string.IsNullOrEmpty(lastOrder))
            {
                var lastSequence = lastOrder.Split('-').Last(); // 000009
                nextNumber = int.Parse(lastSequence) + 1;
            }

            var sequence = nextNumber.ToString("D6"); // 000010

            string UsedId = _currentUserService.UserId.ToString();
            var UserID = _currentUserService.UserId;

            var GroundDTDetails = _context.GroundSettings
                .Include(x => x.Setting)
                .AsNoTracking()
                .ToList();

            //var dtSettings = await _context.DiscountSettings.Where(x => GroundDTDetails.Select(x => x.SettingId).Contains(x.Id)).AsNoTracking()
            //    .ToListAsync();

            taxex = GroundDTDetails.Where(x => x.IsDiscount == false && x.DTCode != "HAN").Sum(x => Convert.ToInt32(x.Setting?.Value));


            discount = GroundDTDetails.Where(x => x.IsDiscount).Sum(x => Convert.ToInt32(x.Setting?.Value));

            totalAmount = totalAmount + taxex - discount;

            var slots = _context.GroundSlots.Where(x => request.Slots.Contains(x.Id) && x.Action == AvailabilityAction.Available && x.GroundId == request.GroundId).ToList();

            var todayDate = DateTime.Today.Date;

            var groundBooked = await _context.GroundBookings.Where(x => x.GroundId == request.GroundId && x.BookingDate.Date == todayDate)
                .AsNoTracking()
                .ToListAsync();

            var bookedSlots = await _context.GroundBookingSlots.Where(x => groundBooked.Select(x => x.Id).Contains(x.BookingId) && request.Slots.Contains(x.SlotId)).ToListAsync();


            var availableSlots = request.Slots
                .Except(bookedSlots.Select(x => x.SlotId))
                .ToList();

            if (bookedSlots != null)
                throw new KeyNotFoundException($"Some Slots Are Already Booked of '{request.BookingDate}'. \n Booked Slots are '{bookedSlots}'");

            GroundPaymentIpnLogs? OnlinePaymentLogs = null;
            if (request.PaymentMethod == PaymentMethod.Online)
            {
                if (string.IsNullOrWhiteSpace(request.BuketId))
                    throw new KeyNotFoundException("BasketId is required for online payment.");

                OnlinePaymentLogs = await _context.GroundPaymentIpnLogs.FirstOrDefaultAsync(x => x.BasketId == request.BuketId);

                if (OnlinePaymentLogs == null)
                    throw new KeyNotFoundException("Invalid or unpaid BasketId.");
            }

            var slotsPricing = slots
                .ToDictionary(x => x.Id, x => x.SlotPrice);

            var groundSlotData = request.Slots.Select(x =>
            {
                var itemPrice = Convert.ToDecimal(slotsPricing[x]);

                totalAmount += itemPrice;

                return new GroundBookingSlot
                {
                    SlotId = x,
                    SlotPrice = itemPrice.ToString(),
                };
            }).ToList();

            AmountToCollecot = Convert.ToDecimal(request.PaymentMethod == PaymentMethod.Cash ? totalAmount : request.PaymentMethod == PaymentMethod.Online && request.BuketId != null ? (totalAmount - OnlinePaymentLogs?.TransactionAmount) : 0);

            var gorunds = new Domain.Entities.GBMS.GroundBooking
            {
                GroundId = request.GroundId,
                BookingDescription = request.Description,
                BookingDate = request.BookingDate,
                TotalAmount = totalAmount.ToString(),
                SubTotal = Convert.ToInt32(totalAmount + taxex - discount).ToString(),
                PaymentMethod = request.PaymentMethod,
                Taxes = taxex.ToString(),
                Discount = discount.ToString(),
                AmountToCollect = request.PaymentMethod == PaymentMethod.Cash ? totalAmount.ToString() : request.PaymentMethod == PaymentMethod.Online && request.BuketId != null ? (totalAmount - OnlinePaymentLogs?.TransactionAmount).ToString() : 0.ToString(),
                CollectedAmount = request.PaymentMethod == PaymentMethod.Cash ? 0.ToString() : OnlinePaymentLogs?.TransactionAmount.ToString(),
                IsConfirm = AmountToCollecot == 0 ? true : false,
                BookingCode = $"GB-{today}-{sequence}",
                UserId = UserID
            };

            _context.GroundBookings.Add(gorunds);
            await _context.SaveChangesAsync(ct);

            foreach (var summary in groundSlotData)
            {
                summary.BookingId = gorunds.Id;
            }


            _context.GroundBookingSlots.AddRange(groundSlotData);
            await _context.SaveChangesAsync(ct);

            var DiscountTaxDetails = await _context.DiscountSettings
                .AsNoTracking()
                .ToListAsync();

            var GroundpaymentDTSetting = DiscountTaxDetails.Select(x => new GroundSetting
            {
                GroundId = gorunds.Id,
                SettingId = x.Id,
                IsDiscount = x.IsDiscount
            }).ToList();


            _context.GroundSettings.AddRange(GroundpaymentDTSetting);
            await _context.SaveChangesAsync(ct);

            // Return GUID safely
            return new SuccessResponse<string>(
                gorunds.BookingCode,
                "Ground Reserved successfully."
            );
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Ground Creation failed: {ex.Message}", ex);
        }
    }
}

