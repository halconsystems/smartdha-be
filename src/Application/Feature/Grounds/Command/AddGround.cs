using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Entities.GBMS;
using DHAFacilitationAPIs.Domain.Enums;
using DHAFacilitationAPIs.Domain.Enums.GBMS;
using Microsoft.AspNetCore.Identity;
using static Dapper.SqlMapper;

namespace DHAFacilitationAPIs.Application.Feature.Grounds.Command;

public record RegisterGroundCommand(
    string Name,
    string Description,
    string Location,
    string ContactNumber,
    string AccountNo,
    string AccountNoAccronym,
    GroundType GroundType,
    GroundCategory GroundCategory,
    Guid? ClubId,
    List<Domain.Enums.Settings>? discounts,
    List<Domain.Enums.Settings>? taxes
) : IRequest<SuccessResponse<Guid>>;
public class RegisterGroundCommandHandler
    : IRequestHandler<RegisterGroundCommand, SuccessResponse<Guid>>
{
    private readonly IOLMRSApplicationDbContext _context;

    public RegisterGroundCommandHandler(
        IOLMRSApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<Guid>> Handle(RegisterGroundCommand request, CancellationToken ct)
    {

        var existingGround = await _context.Grounds
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u =>
                    u.Name == request.Name &&
                    u.GroundType == request.GroundType &&
                    u.GroundCategory == request.GroundCategory,
                    ct);

        if (existingGround != null)
            throw new RecordAlreadyExistException($"Ground '{request.Name}' is already registered. in this '{request.GroundType}' and '{request.GroundCategory}'");

        try
        {

            var gorunds = new Domain.Entities.GBMS.Grounds
            {
                Name = request.Name,
                Description = request.Description,
                Location = request.Location,
                ContactNumber = request.ContactNumber,
                AccountNo = request.AccountNo,
                AccountNoAccronym = request.AccountNoAccronym,
                GroundType = request.GroundType,
                GroundCategory = request.GroundCategory,
                ClubId = request.ClubId ,
            };

            _context.Grounds.Add(gorunds);
            await _context.SaveChangesAsync(ct);

            if(request.discounts != null)
            {
                var discounts = _context.DiscountSettings.Where(x => request.discounts.Contains(x.Name) && x.IsDiscount == true)
                .Select(x => new
                {
                    x.Code,
                    x.Id
                })
                .ToList();

                foreach (var discount in discounts)
                {
                    var clubSettingEntity = new GroundSetting
                    {
                        GroundId = gorunds.Id,
                        DTCode = discount.Code,
                        IsDiscount = true,
                        SettingId = discount.Id
                    };
                    _context.GroundSettings.Add(clubSettingEntity);
                    await _context.SaveChangesAsync(ct);
                }
            }
            if(request.taxes != null)
            {

                var taxes = _context.DiscountSettings.Where(x => request.taxes.Contains(x.Name) && x.IsDiscount == false)
                    .Select(x => new
                    {
                        x.Code,
                        x.Id
                    })
                    .ToList();

                foreach (var tax in taxes)
                {
                    var clubSettingEntity = new GroundSetting
                    {
                        GroundId = gorunds.Id,
                        DTCode = tax.Code,
                        IsDiscount = false,
                        SettingId = tax.Id
                    };
                    _context.GroundSettings.Add(clubSettingEntity);
                    await _context.SaveChangesAsync(ct);
                }
            }
           

            

            


            // Return GUID safely
            return new SuccessResponse<Guid>(
                gorunds.Id,
                "Owner registered successfully."
            );
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Ground Creation failed: {ex.Message}", ex);
        }
    }
}

