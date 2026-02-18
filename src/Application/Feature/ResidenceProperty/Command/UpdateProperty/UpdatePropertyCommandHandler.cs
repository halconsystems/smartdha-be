using System.Threading;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.Property.Command;
using DHAFacilitationAPIs.Application.Feature.ResidenceProperty.Command.UpdateProperty;
using DHAFacilitationAPIs.Domain.Constants;
using DHAFacilitationAPIs.Domain.Entities.Smartdha;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DHAFacilitationAPIs.Application.Feature.Property.CommandHandler
{
    public class UpdatePropertyCommandHandler
        : IRequestHandler<UpdatePropertyCommand, Result<UpdatePropertyResponse>>
    {
        private readonly ISmartdhaDbContext _smartdhaDbContext;
        private readonly IFileStorageService _fileStorage;

        public UpdatePropertyCommandHandler(ISmartdhaDbContext smartdhaDbContext, IFileStorageService fileStorage)
        {
            _smartdhaDbContext = smartdhaDbContext;
            _fileStorage = fileStorage;
        }

        public async Task<Result<UpdatePropertyResponse>> Handle(UpdatePropertyCommand request, CancellationToken cancellationToken)
        {
            var entity = await _smartdhaDbContext.ResidentProperties
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity == null)
                return Result<UpdatePropertyResponse>.Failure(
     new[] { "Error updating property" });


            if (request.ProofOfPossessionImage != null && request.ProofOfPossessionImage.Length > 0)
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(entity.ProofOfPossession))
                        await _fileStorage.DeleteFileAsync(entity.ProofOfPossession, cancellationToken);
                }
                catch { }

                    entity.ProofOfPossession = await _fileStorage.SaveFileInternalAsync(
                    file: request.ProofOfPossessionImage,
                    moduleFolder: FileStorageConstants.Modules.SmartDHA,
                    subFolder: "proof_of_possession",
                    ct: cancellationToken,
                    maxBytes: FileStorageConstants.MaxSize.Image,
                    allowedExtensions: FileStorageConstants.Extensions.Images,
                    allowedMimeTypes: FileStorageConstants.MimeTypes.Images);               
            }
                if (request.UtilityBillAttachment != null && request.UtilityBillAttachment.Length > 0)
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(entity.UtilityBillAttachment))
                            await _fileStorage.DeleteFileAsync(entity.UtilityBillAttachment, cancellationToken);
                    }
                    catch { }

                    entity.UtilityBillAttachment = await _fileStorage.SaveFileInternalAsync(
                    file: request.UtilityBillAttachment,
                    moduleFolder: FileStorageConstants.Modules.SmartDHA,
                    subFolder: "utility_bill",
                    ct: cancellationToken,
                    maxBytes: FileStorageConstants.MaxSize.Image,
                    allowedExtensions: FileStorageConstants.Extensions.Images,
                    allowedMimeTypes: FileStorageConstants.MimeTypes.Images);
                }

                entity.Category = request.Category;
                entity.Type = request.Type;
                entity.Phase = request.Phase;
                entity.Zone = request.Zone;
                entity.Khayaban = request.Khayaban;
                entity.Floor = request.Floor;
                entity.StreetNo = request.StreetNo;
                entity.PlotNo = request.PlotNo;
                entity.Plot = request.Plot;
                entity.PossessionType = request.PossessionType;

                await _smartdhaDbContext.SaveChangesAsync(cancellationToken);

            return Result<UpdatePropertyResponse>.Success(
 new UpdatePropertyResponse
 {
     Success = true,
     Message = "Property Updated Successfully"
 });

        }
    }
    }

