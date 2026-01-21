using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities.GBMS;

namespace DHAFacilitationAPIs.Application.Feature.GroundReservations.Command;

public record GroundComfimBookingCommand 
(
    [Required]
    string BookingCode,
    bool Confirm,
    [Required]
    string CashReceived
) : IRequest<SuccessResponse<Guid>>;
public class GroundComfimBookingCommandHandler
    : IRequestHandler<GroundComfimBookingCommand, SuccessResponse<Guid>>
{
    private readonly IOLMRSApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GroundComfimBookingCommandHandler(
        IOLMRSApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<SuccessResponse<Guid>> Handle(GroundComfimBookingCommand request, CancellationToken ct)
    {
        try
        {    
            var groundBooking = await _context.GroundBookings.FirstOrDefaultAsync(x => x.BookingCode == request.BookingCode && x.BookingDate >= DateTime.Today);

            if(groundBooking == null)
                throw new KeyNotFoundException("Booking not found.");

            if(Convert.ToInt32(groundBooking.AmountToCollect) > 0)
            {
                groundBooking.CollectedAmount = request.CashReceived;
                groundBooking.AmountToCollect = (Convert.ToInt32(groundBooking.TotalAmount) - Convert.ToInt32(request.CashReceived)).ToString();
                groundBooking.IsConfirm = request.Confirm;

                await _context.SaveChangesAsync(ct);

                // Return GUID safely
                return new SuccessResponse<Guid>(
                    groundBooking.Id,
                    "Ground Booking Confirm successfully."
                );
            }
            else
            {
                // Return GUID safely
                return new SuccessResponse<Guid>(
                    groundBooking.Id,
                    $"First Paid Remaining Amount of '{groundBooking.TotalAmount}' successfully."
                );
            }
           


        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Ground Booking Cofirmation failed: {ex.Message}", ex);
        }
    }
}

