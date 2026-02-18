using System.Threading;
using System.Threading.Tasks;
using MediatR;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.UserFamily.UserFamilyCommands.DeleteUserFamilyCommand;

public class DeleteUserFamilyCommandHandler : IRequestHandler<DeleteUserFamilyCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ISmartdhaDbContext _smartDhaContext;

    public DeleteUserFamilyCommandHandler(IApplicationDbContext context, ISmartdhaDbContext smartDhaContext)
    {
        _context = context;
        _smartDhaContext = smartDhaContext;
    }

    public async Task<Result<Guid>> Handle(DeleteUserFamilyCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _smartDhaContext.UserFamilies
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.IsDeleted == false, cancellationToken);

            if (entity == null)
                return Result<Guid>
                .Failure(new[] { "user family data not found" });

            entity.IsDeleted = true;
            entity.IsActive = false;

            await _context.SaveChangesAsync(cancellationToken);
            return Result<Guid>.Success(entity.Id);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure(new[] { ex.Message });
        }
    }
}


