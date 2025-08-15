using MediatR;
using Microsoft.EntityFrameworkCore;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.SearchRooms;

public record SearchRoomsQuery(
    Guid ClubId,
    DateOnly CheckInDate,
    DateOnly CheckOutDate,
    string BookingType // "self" or "guest"
) : IRequest<List<SearchRoomsDto>>;

public class SearchRoomsQueryHandler : IRequestHandler<SearchRoomsQuery, List<SearchRoomsDto>>
{
    private readonly IOLMRSApplicationDbContext _context;
    private readonly IFileStorageService _fileStorageService;

    public SearchRoomsQueryHandler(IOLMRSApplicationDbContext context, IFileStorageService fileStorageService)
    {
        _context = context;
        _fileStorageService = fileStorageService;
    }

    public async Task<List<SearchRoomsDto>> Handle(SearchRoomsQuery request, CancellationToken ct)
    {
        var start = request.CheckInDate;   // DateOnly
        var end = request.CheckOutDate;  // DateOnly

        var query =
            from r in _context.Rooms.AsNoTracking()
            where r.ClubId == request.ClubId
               && r.IsGloballyAvailable

            // روم کی ہر وہ available ونڈو جو پورا رینج کور کرتی ہو (subset allowed)
            from a in r.Availabilities
                .Where(a => a.Action == AvailabilityAction.Available
                     && a.FromDateOnly <= end   // starts before (or on) the search end
                     && a.ToDateOnly >= start) // ends after (or on) the search start




            select new SearchRoomsDto
            {
                // Navigations سے سیدھا
                ResidenceTypeName = r.ResidenceType != null ? r.ResidenceType.Name : string.Empty,
                CategoryName = r.RoomCategory != null ? r.RoomCategory.Name : string.Empty,

                RoomId = r.Id,
                Name = r.Name ?? string.Empty,
                RoomNo = r.No,

                // Charges (requested BookingType کے مطابق)
                Price = _context.RoomCharges
                            .AsNoTracking()
                            .Where(c => c.RoomId == r.Id && c.BookingType == request.BookingType)
                            .Select(c => (decimal?)c.Charges)
                            .FirstOrDefault(),

                // ریٹنگ (اوسط یا تازہ ترین میں سے ایک چنیں)
                // تازہ ترین:
                Ratings = (decimal?)_context.RoomRatings
                            .AsNoTracking()
                            .Where(rr => rr.RoomId == r.Id)
                            .OrderByDescending(rr => rr.Created) // اگر Created نہیں تو ہٹا دیں/Id پر کریں
                            .Select(rr => (double?)rr.RoomRatings)
                            .FirstOrDefault(),

                // مین امیج
                DefaultImage = _context.RoomImages
                            .AsNoTracking()
                            .Where(img => img.RoomId == r.Id && img.Category == ImageCategory.Main)
                            .OrderBy(img => img.Id)
                            .Select(img => img.ImageURL)
                            .FirstOrDefault() ?? string.Empty,

                // صارف کی ریکوئسٹڈ تاریخیں
                CheckInDate = start,
                CheckInTimeOnly=a.FromTimeOnly,
                CheckOutDate = end,
                CheckOutTimeOnly=a.ToTimeOnly,

                // ✅ اس مخصوص available سلوٹ کی رینج

                AvailabilityFrom = a.FromDate,

                AvailabilityTo = a.ToDate
            };

        var list = await query.ToListAsync(ct);

        // امیج URL کو public بنا دیں
        list.ForEach(x =>
        {
            x.DefaultImage = string.IsNullOrWhiteSpace(x.DefaultImage)
                ? string.Empty
                : _fileStorageService.GetPublicUrl(x.DefaultImage);
        });

        return list;
    }




    //public async Task<List<SearchRoomsDto>> Handle(SearchRoomsQuery request, CancellationToken cancellationToken)
    //{
    //    var availableRooms = await (
    //        from r in _context.Rooms
    //        join ra in _context.RoomAvailabilities
    //            on r.Id equals ra.RoomId
    //        where r.ClubId == request.ClubId
    //            && r.IsGloballyAvailable
    //            && ra.FromDate <= request.CheckInDate
    //            && ra.ToDate >= request.CheckOutDate
    //            && ra.Action == AvailabilityAction.Available
    //        select new SearchRoomsDto
    //        {
    //            ResidenceTypeName = _context.ResidenceTypes
    //                .Where(rt => rt.Id == r.ResidenceTypeId)
    //                .Select(rt => rt.Name)
    //                .FirstOrDefault() ?? string.Empty,

    //            CategoryName = _context.RoomCategories
    //                .Where(rc => rc.Id == r.RoomCategoryId)
    //                .Select(rc => rc.Name)
    //                .FirstOrDefault() ?? string.Empty,

    //            RoomId = r.Id,
    //            Name = r.Name ?? string.Empty,
    //            RoomNo = r.No,

    //            Price = _context.RoomCharges
    //                .Where(rc => rc.RoomId == r.Id && rc.BookingType == request.BookingType)
    //                .Select(rc => rc.Charges)
    //                .FirstOrDefault(),

    //            Ratings = _context.RoomRatings
    //                .Where(rr => rr.RoomId == r.Id)
    //                .Select(rr => rr.RoomRatings)
    //                .FirstOrDefault(),

    //            DefaultImage = _context.RoomImages
    //                .Where(img => img.RoomId == r.Id && img.Category == ImageCategory.Main)
    //                .Select(img => img.ImageURL)
    //                .FirstOrDefault() ?? string.Empty,

    //            CheckInDate = ra.FromDate,
    //            CheckOutDate = ra.ToDate
    //        })
    //        .ToListAsync(cancellationToken); // async version

    //    // Now safely modify in memory
    //    availableRooms.ForEach(r =>
    //    {
    //        r.DefaultImage = string.IsNullOrEmpty(r.DefaultImage)
    //            ? string.Empty
    //            : _fileStorageService.GetPublicUrl(r.DefaultImage);
    //    });

    //    return availableRooms;
    //}
}
