using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.DiscountSetting.Command;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Enums;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DHAFacilitationAPIs.Application.Feature.Fumigation.FemigationDT.Command;

public record AddFemigationDTCommand(
    [Required] Settings Name,
     [Required] Domain.Enums.ValueType ValueType,
     [Required] decimal? Value,
     [Required] string? DisplayName
) : IRequest<SuccessResponse<Guid>>;
public class AddFemigationDTCommandCommandHandler
    : IRequestHandler<AddFemigationDTCommand, SuccessResponse<Guid>>
{
    private readonly IApplicationDbContext _context;

    public AddFemigationDTCommandCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<Guid>> Handle(AddFemigationDTCommand request, CancellationToken ct)
    {
        try
        {

            var FmDt = new Domain.Entities.FMS.FemDTSetting
            {
                Name = request.Name,
                DisplayName = request.DisplayName,
                ValueType = request.ValueType,
                Code = request.Name.ToString().Substring(0, request.Name.ToString().Length / 2).ToUpper(),
                Value = request.Value.ToString(),
                IsDiscount = SettingRule.IsDiscount(request.Name)
            };

            _context.FemDTSettings.Add(FmDt);
            await _context.SaveChangesAsync(ct);

            // Return GUID safely
            return new SuccessResponse<Guid>(
                FmDt.Id,
                "Disount/Tax registered successfully."
            );
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Disount/Tax Creation failed: {ex.Message}", ex);
        }
    }
}




