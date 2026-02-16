using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DHAFacilitationAPIs.Application.Feature.Worker.Commands.DeleteWorker;
public class DeleteWorkerCommandHandler : IRequestHandler<DeleteWorkerCommand, Result<Guid>>
{
    private readonly ISmartdhaDbContext _smartdhaContext;
    public  DeleteWorkerCommandHandler(ISmartdhaDbContext smartdhaContext)
    {
        _smartdhaContext = smartdhaContext;
    }
    public async Task<Result<Guid>> Handle(DeleteWorkerCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _smartdhaContext.Workers
                .FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

            if (entity == null)
                return Result<Guid>
                .Failure(new[] { "Worker data not found" });

            entity.IsDeleted = true;
            entity.IsActive = false;

            await _smartdhaContext.SaveChangesAsync(cancellationToken);
            return Result<Guid>.Success(entity.Id);
        }
        catch (Exception ex) {
            return Result<Guid>.Failure(new[] { ex.Message });
        }
    }
}
