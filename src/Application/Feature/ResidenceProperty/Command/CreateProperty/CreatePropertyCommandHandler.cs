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
                string? proofOfPossessionPath = null;
                string? utilityBillPath = null;

                // Save Proof of Possession
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

                // Save Utility Bill
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

                _smartdhaDbContext.ResidentProperties.Add(entity);
                await _smartdhaDbContext.SaveChangesAsync(cancellationToken);

                return Result<CreatePropertyResponse>.Success(
                    new CreatePropertyResponse
                    {
                        Id = entity.Id
                    });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Result<CreatePropertyResponse>.Failure(
                new[] { "Error saving property" });

            }
        }
    }
}
