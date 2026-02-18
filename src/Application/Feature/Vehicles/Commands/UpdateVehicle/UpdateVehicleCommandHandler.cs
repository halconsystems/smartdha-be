using System.Threading;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.Vehicles.Commands.UpdateVehicle.UpdateVecleCommandHandler;
using DHAFacilitationAPIs.Domain.Constants;
using DHAFacilitationAPIs.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace DHAFacilitationAPIs.Application.Feature.Vehicles.Commands.UpdateVehicle.UpdateVecleCommandHandler
{
    public class UpdateVehicleCommandHandler
        : IRequestHandler<UpdateVehicleCommand, Result<UpdateVehicleResponse>>
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

        public async Task<Result<UpdateVehicleResponse>> Handle(UpdateVehicleCommand request, CancellationToken cancellationToken)
        {
            var entity = await _smartdhaDbContext.Vehicles.FindAsync(request.Id);

            if (entity == null)
            {
                return Result<UpdateVehicleResponse>.Failure(
     new[] { "Error updating vehicle" });

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

            var newAttachmentPath = await _fileStorage.SaveFileInternalAsync(
            file: request.Attachment,
            moduleFolder: FileStorageConstants.Modules.SmartDHA,
            subFolder: "vehicles",
            ct: cancellationToken,
            maxBytes: FileStorageConstants.MaxSize.Image,
            allowedExtensions: FileStorageConstants.Extensions.Images,
            allowedMimeTypes: FileStorageConstants.MimeTypes.Images);

                entity.Attachment = newAttachmentPath;
            }

            entity.Color = request.Color;
            entity.Make = request.Make;
            entity.Model = request.Model;

            await _smartdhaDbContext.SaveChangesAsync(cancellationToken);

            return Result<UpdateVehicleResponse>.Success(
     new UpdateVehicleResponse
     {
         Success = true,
         Message = "Vehicle Updated Successfully"
     });

        }
    }
}
