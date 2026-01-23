using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DHAFacilitationAPIs.Application.Common.Exceptions;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Enums;
using Newtonsoft.Json.Linq;

namespace DHAFacilitationAPIs.Application.Feature.Fumigation.FemigationDT.Command;

public record UpdateFemDTCommand(
    [Required] Guid Id,
    [Required] Settings Name,
     [Required] Domain.Enums.ValueType ValueType,
     [Required] decimal? Value,
     [Required] string? DisplayName
) : IRequest<SuccessResponse<Guid>>;
public class UpdateFemDTCommandHandler
    : IRequestHandler<UpdateFemDTCommand, SuccessResponse<Guid>>
{
    private readonly IApplicationDbContext _context;

    public UpdateFemDTCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<Guid>> Handle(UpdateFemDTCommand request, CancellationToken ct)
    {

        var existingService = await _context.FemDTSettings
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u =>
                    u.Id == request.Id,
                    ct);

        if (existingService == null)
            throw new RecordAlreadyExistException($"Service '{request.Name}' is Not registered.");

        try
        {

            existingService.Name = request.Name;
            existingService.DisplayName = request.DisplayName;
            existingService.ValueType = request.ValueType;
            existingService.Code = request.Name.ToString().Substring(0, request.Name.ToString().Length / 2).ToUpper();
            existingService.Value = request.Value.ToString();
            existingService.IsDiscount = SettingRule.IsDiscount(request.Name);

            await _context.SaveChangesAsync(ct);

            // Return GUID safely
            return new SuccessResponse<Guid>(
                existingService.Id,
                "Disount/Tax Updated successfully."
            );
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Disount/Tax Updation failed: {ex.Message}", ex);
        }
    }
}



