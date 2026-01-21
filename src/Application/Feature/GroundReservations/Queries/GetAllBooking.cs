using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.GroundReservations.Queries;

public record GetAllBookingQuery : IRequest<List<BookingDTO>>;
public class GetAllBookingQueryHandler : IRequestHandler<GetAllBookingQuery, List<BookingDTO>>
{
    private readonly IOLMRSApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetAllBookingQueryHandler(IOLMRSApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<BookingDTO>> Handle(GetAllBookingQuery request, CancellationToken ct)
    {
        var userID = _currentUser.UserId.ToString();
        var UserID = _currentUser.UserId;
        if (string.IsNullOrWhiteSpace(userID))
            throw new UnauthorizedAccessException("User not authenticated.");

        var booking = await _context.GroundBookings
            .Include(x => x.Grounds)
            .AsNoTracking()
            .ToListAsync(ct);


        var Slotsumarries = await _context.GroundBookingSlots.Where(x => booking.Select(x => x.Id).Contains(x.BookingId)).AsNoTracking().ToListAsync(ct);

        var grounds = await _context.Grounds.Where(x => booking.Select(x => x.GroundId).Contains(x.Id)).AsNoTracking().ToListAsync(ct);

        var groundImage = await _context.GroundImages.Where(x => booking.Select(x => x.GroundId).Contains(x.GroundId)).ToListAsync(ct);


        if (booking == null)
            throw new KeyNotFoundException("Booking Not Found.");


        var result = booking.Select(x => new BookingDTO
        {
            Id = x.Id,
            GroundId = x.GroundId,
            GroundName = grounds?.FirstOrDefault(g => g.Id == x.GroundId)?.Name,
            BookingDate = x.BookingDate,
            BookingCode = x.BookingCode,
            BookingDescription = x.BookingDescription,
            GroundType = grounds?.FirstOrDefault(d => d.Id == x.GroundId)?.GroundType.ToString(),
            GroundCategory = grounds?.FirstOrDefault(d => d.Id == x.GroundId)?.GroundCategory.ToString(),
            SlotsCount = Slotsumarries.Count().ToString(),
            PaymentMethod = x.PaymentMethod,
            TotalPrice = x.TotalAmount,
            MainImage = groundImage.FirstOrDefault(i => i.GroundId == x.GroundId && i.Category == ImageCategory.Main)?.ImageURL,
        }).ToList();

        return result;
    }
}






