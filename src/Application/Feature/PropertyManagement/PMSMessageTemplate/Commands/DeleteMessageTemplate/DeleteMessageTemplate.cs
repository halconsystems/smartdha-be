using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSMessageTemplate.Commands.DeleteMessageTemplate;
public record DeleteMessageTemplateCommand(Guid Id)
    : IRequest<ApiResult<bool>>;
public class DeleteMessageTemplateHandler
    : IRequestHandler<DeleteMessageTemplateCommand, ApiResult<bool>>
{
    private readonly IPMSApplicationDbContext _db;

    public DeleteMessageTemplateHandler(IPMSApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResult<bool>> Handle(
        DeleteMessageTemplateCommand r,
        CancellationToken ct)
    {
        var template = await _db.Set<MessageTemplate>()
            .FirstOrDefaultAsync(x => x.Id == r.Id && x.IsDeleted !=true, ct);

        if (template == null)
            return ApiResult<bool>.Fail("Template not found.");

        template.IsDeleted = true;
        template.IsActive = false;

        await _db.SaveChangesAsync(ct);
        return ApiResult<bool>.Ok(true, "Template deleted.");
    }
}

