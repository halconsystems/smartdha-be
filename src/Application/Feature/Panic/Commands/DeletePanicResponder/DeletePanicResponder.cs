using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Commands.DeletePanicResponder;
public record DeletePanicResponderCommand(Guid Id) : IRequest<SuccessResponse<string>>;

public class DeletePanicResponderCommandHandler : IRequestHandler<DeletePanicResponderCommand, SuccessResponse<string>>
{
    private readonly IApplicationDbContext _ctx;

    public DeletePanicResponderCommandHandler(IApplicationDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<SuccessResponse<string>> Handle(DeletePanicResponderCommand request, CancellationToken ct)
    {
        var entity = await _ctx.PanicResponders.FindAsync(new object?[] { request.Id }, ct);
        if (entity is null)
            throw new KeyNotFoundException("Panic Responder not found.");

        entity.IsDeleted = true;
        entity.IsActive = false;
        entity.LastModified = DateTime.Now;

        await _ctx.SaveChangesAsync(ct);

        return Success.Delete(entity.Id.ToString());
    }
}
