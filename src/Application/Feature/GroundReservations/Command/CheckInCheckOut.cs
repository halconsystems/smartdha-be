using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.GroundReservations.Command;

public record CheckInCheckOutCommand
(
    [Required]
    string BookingCode,
    DateTime? CheckinDate,
    DateTime? CheckOutDate
) : IRequest<SuccessResponse<Guid>>;
public class CheckInCheckOutCommandHandler
    : IRequestHandler<CheckInCheckOutCommand, SuccessResponse<Guid>>
{
    private readonly IOLMRSApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CheckInCheckOutCommandHandler(
        IOLMRSApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<SuccessResponse<Guid>> Handle(CheckInCheckOutCommand request, CancellationToken ct)
    {
        try
        {
            var groundBooking = await _context.GroundBookings.FirstOrDefaultAsync(x => x.BookingCode == request.BookingCode && x.BookingDate == DateTime.Today);

            if (groundBooking == null)
                throw new KeyNotFoundException("Booking not found.");

            if(request.CheckinDate.HasValue)
            {
                groundBooking.CheckinDate = request.CheckinDate.Value;

                await _context.SaveChangesAsync(ct);

                return new SuccessResponse<Guid>(
                   groundBooking.Id,
                   "Ground Check In successfully."
               );

            }
            else if(request.CheckOutDate.HasValue)
            {
                groundBooking.CheckOutDate = request.CheckOutDate.Value;

                await _context.SaveChangesAsync(ct);

                return new SuccessResponse<Guid>(
                  groundBooking.Id,
                  "Ground Check Out successfully."
              );
            }
            else
            {
                return new SuccessResponse<Guid>(
                  groundBooking.Id,
                  "CheckIn /CheckOut Necessary."
              );
            }

        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Ground CheckIn/CheckOut failed: {ex.Message}", ex);
        }
    }
}

