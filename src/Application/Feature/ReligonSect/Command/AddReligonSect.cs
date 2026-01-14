using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.ReligonSect.Command;

public record CreateRelgionSectDto(string Name, string DisplayName, Guid Religon);
public record CreateReligonSectCommand(
    List<CreateRelgionSectDto> Religons
) : IRequest<SuccessResponse<List<string>>>;

public class CreateReligonSectCommandHandler : IRequestHandler<CreateReligonSectCommand, SuccessResponse<List<string>>>
{
    private readonly IApplicationDbContext _context;

    public CreateReligonSectCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SuccessResponse<List<string>>> Handle(CreateReligonSectCommand request, CancellationToken ct)
    {
        try
        {
            var entities = request.Religons.Select(x => new Domain.Entities.ReligonSect
            {
                Name = x.Name,
                DisplayName = x.DisplayName,
                Code = x.DisplayName.Substring(0, x.DisplayName.Length / 2).ToUpper(),
                ReligonId = x.Religon,
            }).ToList();
            await _context.ReligonSects.AddRangeAsync(entities, ct);
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

