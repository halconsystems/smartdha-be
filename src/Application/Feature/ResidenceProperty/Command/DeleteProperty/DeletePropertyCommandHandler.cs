using System.Threading;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Property.Command;
using DHAFacilitationAPIs.Application.Feature.ResidenceProperty.Command.DeleteProperty;
using DHAFacilitationAPIs.Application.Feature.Vehicles.Command;
using DHAFacilitationAPIs.Application.Feature.Vehicles.Commands.DeleteVehicle;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DHAFacilitationAPIs.Application.Feature.Property.CommandHandler
{
    public class DeletePropertyCommandHandler
     : IRequestHandler<DeletePropertyCommand, DeletepropertyResponse>
    {
        private readonly ISmartdhaDbContext _smartdhaDbContext;

        public DeletePropertyCommandHandler(ISmartdhaDbContext smartdhaDbContext)
        {
            _smartdhaDbContext = smartdhaDbContext;
        }

        public async Task<DeletepropertyResponse> Handle(
            DeletePropertyCommand request,
            CancellationToken cancellationToken)
        {
            var entity = await _smartdhaDbContext.ResidentProperties
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity == null)
            {
                return new DeletepropertyResponse
                {
                    Success = false,
                    Message = "Property Not Found"
                };
            }

            entity.IsDeleted = true;
            entity.IsActive = false;

            await _smartdhaDbContext.SaveChangesAsync(cancellationToken);

            return new DeletepropertyResponse
            {
                Success = true,
                Message = "Property Deleted Successfully"
            };
        }
    }

}
