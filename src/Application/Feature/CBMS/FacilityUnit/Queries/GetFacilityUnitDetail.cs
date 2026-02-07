using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.CBMS.Clubs.Queries.GetClubFacilitiesByCategory;
using DHAFacilitationAPIs.Domain.Enums.CBMS;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.FacilityUnit.Queries.FacilityUnitDetail;
public record GetFacilityUnitDetailQuery(
    Guid FacilityUnitId
) : IRequest<ApiResult<FacilityUnitDetailResponse>>;
public class GetFacilityUnitDetailHandler
    : IRequestHandler<GetFacilityUnitDetailQuery, ApiResult<FacilityUnitDetailResponse>>
{
    private readonly ICBMSApplicationDbContext _db;
    private readonly IFileStorageService _fileStorageService;
    public GetFacilityUnitDetailHandler(ICBMSApplicationDbContext db, IFileStorageService fileStorageService)
    {
        _db = db;
        _fileStorageService = fileStorageService;
    }
        public async Task<ApiResult<FacilityUnitDetailResponse>> Handle(
            GetFacilityUnitDetailQuery request,
            CancellationToken ct)
        {
            // =========================
            // FACILITY UNIT
            // =========================
            var unit = await _db.FacilityUnits
                .Include(u => u.Facility)
                .FirstOrDefaultAsync(u =>
                    u.Id == request.FacilityUnitId &&
                    u.IsActive == true &&
                    u.IsDeleted != true,
                    ct);

            if (unit == null)
                return ApiResult<FacilityUnitDetailResponse>.Fail("Facility unit not found");

            // =========================
            // IMAGES
            // =========================
            var imagesRaw = await _db.FacilityUnitImages
                .Where(x =>
                    x.FacilityUnitId == unit.Id &&
                    x.IsActive == true &&
                    x.IsDeleted != true)
                .Select(x => new
                {
                    x.ImageURL,
                    x.ImageExtension,
                    x.ImageName,
                    x.Description,
                    x.Category
                })
                .ToListAsync(ct);

            var images = imagesRaw
                .Select(x => new ImageDto(
                    _fileStorageService.GetPublicUrl(x.ImageURL),
                    x.ImageExtension,
                    x.ImageName,
                    x.Description,
                    x.Category
                ))
                .ToList();

        // =========================
        // SERVICES
        // =========================
        var services = await _db.FacilityUnitServices
 .Where(us =>
     us.FacilityUnitId == unit.Id &&
     us.IsEnabled &&
     us.IsDeleted != true)
 .Select(us => new FacilityUnitServiceDto(
     us.ServiceDefinitionId,                 // ✅ ServiceDefinitionId
     us.ServiceDefinition.Name,              // ✅ Service name
     us.ServiceDefinition.Code,              // ✅ Service code
     us.IsComplimentary,                     // ✅ unit-level flag
     us.ServiceDefinition.IsQuantityBased,   // ✅ definition-level flag
     us.Price,                               // ✅ unit-level price
     us.IsEnabled
 ))
 .ToListAsync(ct);

        // =========================
        // BASE AMOUNT (PREVIEW)
        // =========================
        var config = await _db.FacilityUnitBookingConfigs
                .FirstOrDefaultAsync(x => x.FacilityUnitId == unit.Id, ct);

            var baseAmount = config?.BasePrice ?? 0;

            var servicesTotal = services
                .Where(x => !x.IsComplimentary)
                .Sum(x => x.Price);

            var subTotal = baseAmount + servicesTotal;

            // =========================
            // DISCOUNTS (UNIT LEVEL)
            // =========================
            var discountEntities = await _db.FacilityUnitDiscounts
                .Include(x => x.Discount)
                .Where(x =>
                    x.FacilityUnitId == unit.Id &&
                    x.IsActive==true &&
                    x.Discount.IsActive==true)
                .Select(x => x.Discount)
                .ToListAsync(ct);

            var discountDtos = discountEntities
                .Select(d =>
                {
                    var amount = CalculateDiscount(subTotal, d.Type, d.Value);
                    return new PriceModifierDto(
                        d.Id,
                        d.Name,
                        d.Code,
                        (DiscountOrTaxType)d.Type,
                        d.Value,
                        amount
                    );
                })
                .ToList();

            var discountTotal = discountDtos.Sum(x => x.CalculatedAmount);

            // =========================
            // TAXES (UNIT LEVEL)
            // =========================
            var taxableAmount = subTotal - discountTotal;

            var taxEntities = await _db.FacilityUnitTax
                .Include(x => x.Tax)
                .Where(x =>
                    x.FacilityUnitId == unit.Id &&
                    x.IsActive==true &&
                    x.Tax.IsActive==true)
                .Select(x => x.Tax)
                .ToListAsync(ct);

            var taxDtos = taxEntities
                .Select(t =>
                {
                    var amount = CalculateTax(taxableAmount, t.Type, t.Value);
                    return new PriceModifierDto(
                        t.Id,
                        t.Name,
                        t.Code,
                        (DiscountOrTaxType)t.Type,
                        t.Value,
                        amount
                    );
                })
                .ToList();

            // =========================
            // FINAL RESPONSE
            // =========================
            var response = new FacilityUnitDetailResponse(
                unit.Id,
                unit.Name,
                unit.UnitType,
                new FacilityDto(
                    unit.Facility.Id,
                    unit.Facility.Name,
                    unit.Facility.DisplayName,
                    unit.Facility.Code,
                    unit.Facility.Description
                ),
                images,
                services,
                discountDtos,
                taxDtos
            );

            return ApiResult<FacilityUnitDetailResponse>.Ok(response);
        }

        // =========================
        // CALCULATION HELPERS
        // =========================
        private static decimal CalculateDiscount(
            decimal baseAmount,
            DiscountType type,
            decimal value)
        {
            return type == DiscountType.Percentage
                ? Math.Round(baseAmount * value / 100, 2)
                : value;
        }

        private static decimal CalculateTax(
            decimal baseAmount,
            TaxType type,
            decimal value)
        {
            return type == TaxType.Percentage
                ? Math.Round(baseAmount * value / 100, 2)
                : value;
        }
    }
