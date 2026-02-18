using System.Threading;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.Vehicles.Command;
using DHAFacilitationAPIs.Domain.Entities;
using MediatR;

namespace DHAFacilitationAPIs.Application.Feature.Vehicles.Commands.DeleteVehicle
{
    public class DeleteVehicleCommandHandler
      : IRequestHandler<DeleteVehicleCommand, Result<DeleteVehicleResponse>>
    {
        private readonly ISmartdhaDbContext _smartdhaDbContext;

        public DeleteVehicleCommandHandler(ISmartdhaDbContext smartdhaDbContext)
        {
            _smartdhaDbContext = smartdhaDbContext;
        }

        public async Task<Result<DeleteVehicleResponse>> Handle(
            DeleteVehicleCommand request,
            CancellationToken cancellationToken)
        {
            var entity = await _smartdhaDbContext.Vehicles
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity == null)
            {
                return Result<DeleteVehicleResponse>.Failure(
    new[] { "Error deleting vehicle" });

            }

            entity.IsDeleted = true;
            entity.IsActive = false;

            await _smartdhaDbContext.SaveChangesAsync(cancellationToken);

            return Result<DeleteVehicleResponse>.Success(
    new DeleteVehicleResponse
    {
        Success = true,
        Message = "Vehicle Deleted Successfully"
    });

        }
    }
}
