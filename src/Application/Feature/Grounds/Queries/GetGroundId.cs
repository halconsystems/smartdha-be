using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Grounds.Command.GroundSlots.Queries;
using DHAFacilitationAPIs.Domain.Entities.GBMS;
using DHAFacilitationAPIs.Domain.Enums.GBMS;

using NotFoundException = DHAFacilitationAPIs.Application.Common.Exceptions.NotFoundException;

namespace DHAFacilitationAPIs.Application.Feature.Grounds.Queries;


public record GetGroundQueryById(Guid Id,GroundCategory GroundCategory, DateOnly bookingDate) : IRequest<GroundDTO>;

public class GetGroundQueryByIdHandler : IRequestHandler<GetGroundQueryById, GroundDTO>
{
    private readonly IOLMRSApplicationDbContext _context;
    private readonly IFileStorageService _fileStorageService;

    public GetGroundQueryByIdHandler(
         IOLMRSApplicationDbContext context, IFileStorageService fileStorageService)
    {
        _context = context;
        _fileStorageService = fileStorageService;
    }

    public async Task<GroundDTO> Handle(GetGroundQueryById request, CancellationToken cancellationToken)
    {
        var ground = await _context.Grounds.Where(x => x.GroundCategory == request.GroundCategory && x.Id == request.Id)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if(ground == null)
            throw new NotFoundException("Ground not found.");
        var groundImage = await _context.GroundImages.Where(x => x.GroundId == ground.Id).ToListAsync();

        var GroundSlots = await _context.GroundSlots.Where(x => x.GroundId == ground.Id).ToListAsync();
        if(GroundSlots == null)
            throw new NotFoundException("Ground Slots not found.");

        var groundBooked = await _context.GroundBookings.Where(x => x.GroundId == ground.Id && x.BookingDateOnly == request.bookingDate)
           .AsNoTracking()
           .ToListAsync();

        var bookedSlots = await _context.GroundBookingSlots.Where(x => groundBooked.Select(x => x.Id).Contains(x.BookingId) && GroundSlots.Select(x => x.Id).Contains(x.SlotId)).ToListAsync();

        var GroundStandard = await _context.GroundStandtardTimes.Where(x => x.GroundId == ground.Id).ToListAsync();

        var clubs = await _context.Clubs.Where(x => x.Id == ground.ClubId).FirstOrDefaultAsync();

        var groundSlotIds = GroundSlots.Select(s => s.Id).ToHashSet();
        var bookedSlotIds = bookedSlots
            .Where(b => groundSlotIds.Contains(b.SlotId))
            .Select(b => b.SlotId)
            .Distinct()
            .ToHashSet();

        var totalSlots = groundSlotIds.Count;
        var bookedCount = bookedSlotIds.Count;
        var availableCount = totalSlots - bookedCount;



        var grounds = new GroundDTO
        {
            Id = ground.Id,
            GroundName = ground.Name,
            GroundDescription = ground.Description,
            GroundType = ground.GroundType,
            GroundCategory = ground.GroundCategory,
            Location = ground.Location,
            ContactNumber = ground.ContactNumber,
            ClubName = clubs?.Name,
            AccountNo = ground.AccountNo,
            AccountNoAccronym = ground.AccountNoAccronym,
            SlotCount = totalSlots.ToString(),
            BookedCount = bookedCount,
            AvailableCount = availableCount,
            Slots = GroundSlots.Select(x => new GroundSlotsdto
            {
                GroundId = ground.Id,
                Id = x.Id,
                SlotName = x.SlotName,
                SlotPrice = x.SlotPrice,        
                DisplayName = x.DisplayName,
                Code = x.Code,
                SlotDate = x.SlotDate,
                FromTimeOnly = x.FromTimeOnly,
                ToTimeOnly = x.ToTimeOnly,
                Action = bookedSlots.Any(g => g.SlotId == x.Id) ? AvailabilityAction.Booked : AvailabilityAction.Available,
            }).ToList(),
            GroundImages = groundImage?
        .Select(img =>
        {
            var publicUrl = string.IsNullOrWhiteSpace(img.ImageURL)
                ? null
                : _fileStorageService.GetPublicUrl(img.ImageURL);

            if (publicUrl == null)
                publicUrl = string.Empty;

            return new Command.GroundImages.Queries.GroundImagesDTO
            {
                GroudId = ground.Id,
                Id = img.Id,
                ImageExtension = img.ImageExtension,
                ImageName = img.ImageName,
                ImageURL = publicUrl, // ✅ safe
                Description = img.Description,
                Category = img.Category,
            };
        }).ToList()
        ?? new List<Command.GroundImages.Queries.GroundImagesDTO>(),

            GroundStandtardTimes = GroundStandard
        };

        return grounds;
    }
}



