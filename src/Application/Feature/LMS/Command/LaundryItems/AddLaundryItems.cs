using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using MediatR;

namespace DHAFacilitationAPIs.Application.Feature.LMS.Command.LaundryItems;

internal class AddLaundryItems
{
}
public record CreateLaundryItemsDto(string Name, string DisplayName, string itemPrice);
public record CreateLaundryItemsCommand(
    Guid CategoryId,
    List<CreateLaundryItemsDto> laundryItems
) : IRequest<SuccessResponse<List<string>>>;
public class CreateLaundryItemsCommandHandler : IRequestHandler<CreateLaundryItemsCommand, SuccessResponse<List<string>>>
{
    private readonly ILaundrySystemDbContext _context;

    public CreateLaundryItemsCommandHandler(ILaundrySystemDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<List<string>>> Handle(CreateLaundryItemsCommand command, CancellationToken ct)
    {
        try
        {
            var entities = command.laundryItems.Select(x => new Domain.Entities.LaundryItems
            {
                Name = x.Name,
                DisplayName = x.DisplayName,
                Code = x.DisplayName.Substring(0, x.DisplayName.Length / 2).ToUpper(),
                CategoryId = command.CategoryId,
                ItemPrice = x.itemPrice
                
            }).ToList();
            await _context.LaundryItems.AddRangeAsync(entities, ct);
            await _context.SaveChangesAsync(ct);

            var ids = entities.Select(e => e.Id.ToString()).ToList();
            return Success.Created(ids);

        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}




