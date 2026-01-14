using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.MemberShipCategory.Command;

public record CreateMemberCategoryShipDto(string Name, string DisplayName, Guid MemberShipID);
public record CreateMemberShipCategoryCommand(
    List<CreateMemberCategoryShipDto> MemberShips
) : IRequest<SuccessResponse<List<string>>>;

public class CreateMemberShipCategoryCommandHandler : IRequestHandler<CreateMemberShipCategoryCommand, SuccessResponse<List<string>>>
{
    private readonly IApplicationDbContext _context;

    public CreateMemberShipCategoryCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<List<string>>> Handle(CreateMemberShipCategoryCommand request, CancellationToken ct)
    {
        try
        {
            var entities = request.MemberShips.Select(x => new MemberShipCatergories
            {
                name = x.Name,
                displayname = x.DisplayName,
                Code = x.DisplayName.Substring(0, x.DisplayName.Length / 2).ToUpper(),
                MemberShipId = x.MemberShipID,
            }).ToList();
            await _context.MemberShipCatergories.AddRangeAsync(entities, ct);
            await _context.SaveChangesAsync(ct);

            var ids = entities.Select(e => e.Id.ToString()).ToList();
            return Success.Created(ids);

        }
        catch (Exception)
        {

            throw;
        }
    }
}

