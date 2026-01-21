//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using DHAFacilitationAPIs.Application.Common.Interfaces;
//using DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.SearchRooms;
//using DHAFacilitationAPIs.Domain.Enums;
//using DHAFacilitationAPIs.Domain.Enums.GBMS;

//namespace DHAFacilitationAPIs.Application.Feature.Grounds.Queries;
//public record SearchGroundQuery(
//    Guid GroundId,
//    DateOnly CheckInDate,
//    DateOnly CheckOutDate,
//    GroundCategory GroundCategory, // "self" or "guest"
//    TimeOnly? CheckInTime = null,
//    TimeOnly? CheckOutTime = null
//) : IRequest<List<GroundDTO>>;

//public class SearchGroundQueryHandler : IRequestHandler<SearchGroundQuery, List<GroundDTO>>
//{
//    private readonly IOLMRSApplicationDbContext _context;
//    private readonly IFileStorageService _fileStorageService;

//    public SearchGroundQueryHandler(IOLMRSApplicationDbContext context, IFileStorageService fileStorageService)
//    {
//        _context = context;
//        _fileStorageService = fileStorageService;
//    }

//    public async Task<List<GroundDTO>> Handle(SearchGroundQuery request, CancellationToken ct)
//    {
//        var startDate = request.CheckInDate;   // DateOnly
//        var endDate = request.CheckOutDate;  // DateOnly
//        TimeOnly checkIn;
//        TimeOnly checkOut;

//        if (request.CheckInTime.HasValue && request.CheckOutTime.HasValue)
//        {
//            checkIn = request.CheckInTime.Value;
//            checkOut = request.CheckOutTime.Value;
//        }
//        else
//        {
//            var stdTimes = await _context.GroundStandtardTimes.AsNoTracking()
//                            .Where(std => std.GroundId == request.GroundId && std.IsDeleted == false && std.IsActive == true)
//                            .Select(std => new { std.CheckInTime, std.CheckOutTime }).FirstOrDefaultAsync(ct);
//            if (stdTimes == null)
//                throw new Exception($"Standard booking times not set for Ground ID: {request.GroundId}");

//            checkIn = stdTimes.CheckInTime;
//            checkOut = stdTimes.CheckOutTime;
//        }

//        var start = startDate.ToDateTime(checkIn);
//        var end = endDate.ToDateTime(checkOut);

//        var ground = await _context.Grounds.Where(x => x.GroundCategory == request.GroundCategory)
//            .AsNoTracking()
//            .ToListAsync();


//        var ground = await _context.Grounds.Where(x => x.GroundCategory == request.GroundCategory)
//            .AsNoTracking()
//            .ToListAsync();

//        var groundImage = await _context.GroundImages.Where(x => ground.Select(x => x.Id).Contains(x.GroundId)).ToListAsync();

//        var clubs = await _context.Clubs.Where(x => ground.Select(x => x.ClubId).Contains(x.Id)).ToListAsync();

//        var grounds = ground.Select(x => new GroundDTO
//        {
//            Id = x.Id,
//            GroundName = x.Name,
//            MainImageUrl = groundImage?.FirstOrDefault(y => y.GroundId == x.Id)?.ImageURL,
//            GroundDescription = x.Description,
//            GroundType = x.GroundType,
//            GroundCategory = x.GroundCategory,
//            Location = x.Location,
//            ContactNumber = x.ContactNumber,
//            //ClubName = clubs != null ? clubs.FirstOrDefault(c => c.Id == x.ClubId).Name : ""

//        }).ToList();

//        var query =
//            from r in _context.GroundSlots.AsNoTracking()
//            where r.ClubId == request.GroundId
//               && r.IsGloballyAvailable

//            // Check rooms according to their availibility (subset allowed)
//            from a in r.Availabilities
//                .Where(a => a.Action == AvailabilityAction.Available
//                     && a.FromDate <= start   // starts before (or on) the search end
//                     && a.ToDate >= end
//                     && a.IsDeleted == false) // ends after (or on) the search start
//            where !_context.ReservationRooms.Any(res =>     // Exclude rooms already reserved/booked in overlapping period
//                res.RoomId == r.Id &&
//                res.FromDate < end &&
//                start < res.ToDate &&
//                //   (res.Reservation.Status == Domain.Enums.ReservationStatus.AwaitingPayment
//                //    || res.Reservation.Status == Domain.Enums.ReservationStatus.Converted))
//                (res.Reservation.ExpiresAt > DateTime.Now))
//            && !_context.RoomBookings.Any(bk =>
//                bk.RoomId == r.Id &&
//                bk.Status == BookingStatus.Confirmed &&
//                bk.CheckInDate < end &&
//                start < bk.CheckOutDate)

//            select new SearchRoomsDto
//            {
//                ResidenceTypeName = r.ResidenceType != null ? r.ResidenceType.Name : string.Empty,
//                CategoryName = r.RoomCategory != null ? r.RoomCategory.Name : string.Empty,

//                RoomId = r.Id,
//                Name = r.Name ?? string.Empty,
//                RoomNo = r.No,

//                // Charges (requested according to BookingType & 1 Occupant)
//                Price = _context.RoomCharges
//                            .AsNoTracking()
//                            .Where(c => c.RoomId == r.Id && c.BookingType == request.BookingType && c.ExtraOccupancy == 0 && c.IsActive == true && c.IsDeleted == false)
//                            .Select(c => (decimal?)c.Charges)
//                            .FirstOrDefault(),


//                Ratings = (decimal?)_context.RoomRatings
//                            .AsNoTracking()
//                            .Where(rr => rr.RoomId == r.Id)
//                            .OrderByDescending(rr => rr.Created) // If not Created then remove/set Id
//                            .Select(rr => (double?)rr.RoomRatings)
//                            .FirstOrDefault(),

//                // Main Image
//                DefaultImage = _context.RoomImages
//                            .AsNoTracking()
//                            .Where(img => img.RoomId == r.Id && img.Category == ImageCategory.Main && r.IsActive == true && r.IsDeleted == false)
//                            .OrderBy(img => img.Id)
//                            .Select(img => img.ImageURL)
//                            .FirstOrDefault() ?? string.Empty,

//                // User requested dates
//                CheckInDate = startDate,
//                CheckInTimeOnly = checkIn,
//                CheckOutDate = endDate,
//                CheckOutTimeOnly = checkOut,

//                // Range of this specific available slot

//                AvailabilityFrom = a.FromDate,

//                AvailabilityTo = a.ToDate
//            };

//        var list = await query.ToListAsync(ct);

//        // Make the image URL public
//        list.ForEach(x =>
//        {
//            x.DefaultImage = string.IsNullOrWhiteSpace(x.DefaultImage)
//                ? string.Empty
//                : _fileStorageService.GetPublicUrl(x.DefaultImage);
//        });

//        return list;
//    }





//}

