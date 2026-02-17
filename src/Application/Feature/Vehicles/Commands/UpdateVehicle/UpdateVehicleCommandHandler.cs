using System.Threading;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Vehicles.Commands.UpdateVehicle.UpdateVecleCommandHandler;
using DHAFacilitationAPIs.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace DHAFacilitationAPIs.Application.Feature.Vehicles.Commands.UpdateVehicle.UpdateVecleCommandHandler
{
    public class UpdateVehicleCommandHandler
        : IRequestHandler<UpdateVehicleCommand, UpdateVehicleResponse>
    {
        private readonly ISmartdhaDbContext _smartdhaDbContext;
        private readonly IFileStorageService _fileStorage;

        public UpdateVehicleCommandHandler(
            ISmartdhaDbContext smartdhaDbContext,
            IFileStorageService fileStorage)
        {
            _smartdhaDbContext = smartdhaDbContext;
            _fileStorage = fileStorage;
        }

        public async Task<UpdateVehicleResponse> Handle(UpdateVehicleCommand request, CancellationToken cancellationToken)
        {
            var entity = await _smartdhaDbContext.Vehicles.FindAsync(request.Id);

            if (entity == null)
            {
                return new UpdateVehicleResponse
                {
                    Success = false,
                    Message = "Vehicle Not Found"
                };
            }

            if (request.Attachment != null && request.Attachment.Length > 0)
            {
                if (!string.IsNullOrWhiteSpace(entity.Attachment))
                {
                    try
                    {
                        await _fileStorage.DeleteFileAsync(entity.Attachment, cancellationToken);
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }

                var newAttachmentPath = await _fileStorage.SaveFileAsync(
                    request.Attachment,
                    "uploads/vehicles",
                    cancellationToken);

                entity.Attachment = newAttachmentPath;
            }

            entity.Color = request.Color;
            entity.Make = request.Make;
            entity.Model = request.Model;

            await _smartdhaDbContext.SaveChangesAsync(cancellationToken);

            return new UpdateVehicleResponse
            {
                Success = true,
                Message = "Vehicle Updated Successfully"
            };
        }
    }
}
