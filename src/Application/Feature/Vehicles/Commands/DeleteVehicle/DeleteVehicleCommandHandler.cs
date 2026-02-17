using System.Threading;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Vehicles.Command;
using DHAFacilitationAPIs.Domain.Entities;
using MediatR;

namespace DHAFacilitationAPIs.Application.Feature.Vehicles.Commands.DeleteVehicle
{
    public class DeleteVehicleCommandHandler
      : IRequestHandler<DeleteVehicleCommand, DeleteVehicleResponse>
    {
        private readonly ISmartdhaDbContext _smartdhaDbContext;

        public DeleteVehicleCommandHandler(ISmartdhaDbContext smartdhaDbContext)
        {
            _smartdhaDbContext = smartdhaDbContext;
        }

        public async Task<DeleteVehicleResponse> Handle(
            DeleteVehicleCommand request,
            CancellationToken cancellationToken)
        {
            var entity = await _smartdhaDbContext.Vehicles
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity == null)
            {
                return new DeleteVehicleResponse
                {
                    Success = false,
                    Message = "Vehicle Not Found"
                };
            }

            entity.IsDeleted = true;
            entity.IsActive = false;

            await _smartdhaDbContext.SaveChangesAsync(cancellationToken);

            return new DeleteVehicleResponse
            {
                Success = true,
                Message = "Vehicle Deleted Successfully"
            };
        }
    }
}
