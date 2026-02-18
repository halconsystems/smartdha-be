using System.Threading;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.Property.Command;
using DHAFacilitationAPIs.Application.Feature.ResidenceProperty.Command.DeleteProperty;
using DHAFacilitationAPIs.Application.Feature.Vehicles.Command;
using DHAFacilitationAPIs.Application.Feature.Vehicles.Commands.DeleteVehicle;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DHAFacilitationAPIs.Application.Feature.Property.CommandHandler
{
    public class DeletePropertyCommandHandler
     : IRequestHandler<DeletePropertyCommand, Result<DeletepropertyResponse>>
    {
        private readonly ISmartdhaDbContext _smartdhaDbContext;

        public DeletePropertyCommandHandler(ISmartdhaDbContext smartdhaDbContext)
        {
            _smartdhaDbContext = smartdhaDbContext;
        }

        public async Task<Result<DeletepropertyResponse>> Handle(
            DeletePropertyCommand request,
            CancellationToken cancellationToken)
        {
            var entity = await _smartdhaDbContext.ResidentProperties
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity == null)
            {
                return Result<DeletepropertyResponse>.Failure(
     new[] { "Error deleting property" });

            }

            entity.IsDeleted = true;
            entity.IsActive = false;

            await _smartdhaDbContext.SaveChangesAsync(cancellationToken);

            return Result<DeletepropertyResponse>.Success(
    new DeletepropertyResponse
    {
        Success = true,
        Message = "Property Deleted Successfully"
    });

        }
    }

}
