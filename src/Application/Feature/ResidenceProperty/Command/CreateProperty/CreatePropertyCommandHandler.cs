using System;
using System.Threading;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.Property.Command;
using DHAFacilitationAPIs.Application.Feature.ResidenceProperty.Command.CreateProperty;
using DHAFacilitationAPIs.Domain.Constants;
using DHAFacilitationAPIs.Domain.Entities.Smartdha;
using DHAFacilitationAPIs.Domain.Enums;
using MediatR;

namespace DHAFacilitationAPIs.Application.Feature.Property.CommandHandler
{
    public class CreatePropertyCommandHandler
        : IRequestHandler<CreatePropertyCommand, Result<CreatePropertyResponse>>
    {
        private readonly ISmartdhaDbContext _smartdhaDbContext;
        private readonly IFileStorageService _fileStorage;

        public CreatePropertyCommandHandler(
            ISmartdhaDbContext smartdhaDbContext,
            IFileStorageService fileStorage)
        {
            _smartdhaDbContext = smartdhaDbContext;
            _fileStorage = fileStorage;
        }

        public async Task<Result<CreatePropertyResponse>> Handle(
            CreatePropertyCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                if (!Enum.IsDefined(typeof(CategoryType), request.Category) ||
                    !Enum.IsDefined(typeof(PropertyType), request.Type) ||
                    !Enum.IsDefined(typeof(Phase), request.Phase) ||
                    !Enum.IsDefined(typeof(Zone), request.Zone) ||
                    !Enum.IsDefined(typeof(ResidenceStatusDha), request.PossessionType))
                {
                    return Result<CreatePropertyResponse>.Failure(
                        new[] { "Invalid enum value provided." });
                }

                string? proofOfPossessionPath = null;
                string? utilityBillPath = null;

                if (request.ProofOfPossessionImage != null)
                {
                    proofOfPossessionPath = await _fileStorage.SaveFileInternalAsync(
                        file: request.ProofOfPossessionImage,
                        moduleFolder: FileStorageConstants.Modules.SmartDHA,
                        subFolder: "proof_of_possession",
                        ct: cancellationToken,
                        maxBytes: FileStorageConstants.MaxSize.Image,
                        allowedExtensions: FileStorageConstants.Extensions.Images,
                        allowedMimeTypes: FileStorageConstants.MimeTypes.Images);
                }

                if (request.UtilityBillAttachment != null)
                {
                    utilityBillPath = await _fileStorage.SaveFileInternalAsync(
                        file: request.UtilityBillAttachment,
                        moduleFolder: FileStorageConstants.Modules.SmartDHA,
                        subFolder: "utility_bill",
                        ct: cancellationToken,
                        maxBytes: FileStorageConstants.MaxSize.Image,
                        allowedExtensions: FileStorageConstants.Extensions.Images,
                        allowedMimeTypes: FileStorageConstants.MimeTypes.Images);
                }

                var entity = new ResidentProperty
                {
                    Category = (CategoryType)request.Category,
                    Type = (PropertyType)request.Type,
                    Phase = (Phase)request.Phase,
                    Zone = (Zone)request.Zone,
                    Khayaban = request.Khayaban,
                    Floor = request.Floor,
                    StreetNo = request.StreetNo,
                    PlotNo = request.PlotNo,
                    Plot = request.Plot,
                    PossessionType = (ResidenceStatusDha)request.PossessionType,
                    ProofOfPossession = proofOfPossessionPath,
                    UtilityBillAttachment = utilityBillPath,
                };

                await _smartdhaDbContext.ResidentProperties.AddAsync(entity, cancellationToken);
                await _smartdhaDbContext.SaveChangesAsync(cancellationToken);

                // ✅ Success Response
                return Result<CreatePropertyResponse>.Success(
                    new CreatePropertyResponse
                    {
                        Id = entity.Id,
                        Message = "Property Created Successfully"
                    });
            }
            catch (Exception ex)
            {
                // ❗ Production me logging service use karna chahiye
                Console.WriteLine(ex);

                return Result<CreatePropertyResponse>.Failure(
                    new[] { "An error occurred while saving the property." });
            }
        }
    }
}
