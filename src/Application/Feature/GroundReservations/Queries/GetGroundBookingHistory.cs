using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.GroundReservations.Queries;

public record GetGroundBookingHistoryQuery(Guid BookingId) : IRequest<BookingDTO>;
public class GetGroundBookingHistoryQueryHandler : IRequestHandler<GetGroundBookingHistoryQuery, BookingDTO>
{
    private readonly IOLMRSApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetGroundBookingHistoryQueryHandler(IOLMRSApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<BookingDTO> Handle(GetGroundBookingHistoryQuery request, CancellationToken ct)
    {

        var booking = await _context.GroundBookings.Where(x => x.GroundId == request.BookingId)
            .Include(x => x.Grounds)
            .AsNoTracking()
            .FirstOrDefaultAsync(ct);

        if (booking == null)
            throw new KeyNotFoundException("Booking not found.");

        var Slotsumarries = await _context.GroundBookingSlots.Where(x => x.BookingId == booking.Id).AsNoTracking().ToListAsync(ct);

        var grounds = await _context.Grounds.Where(x => x.Id == booking.GroundId).AsNoTracking().ToListAsync(ct);

        var groundImage = await _context.GroundImages.Where(x => x.GroundId == booking.GroundId).ToListAsync(ct);

        var grounddtSetting = await _context.GroundSettings.Where(x => x.GroundId == booking.GroundId).ToListAsync(ct);


        if (booking == null)
            throw new KeyNotFoundException("Booking Not Found.");


        var result = new BookingDTO
        {
            Id = booking.Id,
            GroundId = booking.GroundId,
            GroundName = grounds?.FirstOrDefault(g => g.Id == booking.GroundId)?.Name,
            BookingDate = booking.BookingDate,
            BookingCode = booking.BookingCode,
            BookingDescription = booking.BookingDescription,
            GroundType = grounds?.FirstOrDefault(d => d.Id == booking.GroundId)?.GroundType.ToString(),
            GroundCategory = grounds?.FirstOrDefault(d => d.Id == booking.GroundId)?.GroundCategory.ToString(),
            SlotsCount = Slotsumarries.Count().ToString(),
            PaymentMethod = booking.PaymentMethod,
            TotalPrice = booking.TotalAmount,
            MainImage = groundImage.FirstOrDefault(i => i.GroundId == booking.GroundId && i.Category == ImageCategory.Main)?.ImageURL,
            GroundImages = groundImage,
            Slots = Slotsumarries,
            GroundSettings = grounddtSetting
        }; ;

        return result;
    }
}






