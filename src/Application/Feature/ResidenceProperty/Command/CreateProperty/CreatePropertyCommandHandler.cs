using System.Threading;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.Property.Command;
using DHAFacilitationAPIs.Application.Feature.ResidenceProperty.Command.CreateProperty;
using DHAFacilitationAPIs.Domain.Entities.Smartdha;
using DHAFacilitationAPIs.Domain.Enums;
using MediatR;

namespace DHAFacilitationAPIs.Application.Feature.Property.CommandHandler
{
    public class CreatePropertyCommandHandler
        : IRequestHandler<CreatePropertyCommand, CreatePropertyResponse>
    {
        private readonly ISmartdhaDbContext _smartdhaDbContext;
        private readonly IFileStorageService _fileStorage;

        public CreatePropertyCommandHandler(ISmartdhaDbContext smartdhaDbContext, IFileStorageService fileStorage)
        {
            _smartdhaDbContext = smartdhaDbContext;
            _fileStorage = fileStorage;
        }

        public async Task<CreatePropertyResponse> Handle(CreatePropertyCommand request, CancellationToken cancellationToken)
        {
            // Save uploaded files and get paths
            string? proofOfPossessionPath = null;
            string? utilityBillPath = null;

            if (request.ProofOfPossessionImage != null)
            {
                proofOfPossessionPath = await _fileStorage.SaveFileAsync(
                    request.ProofOfPossessionImage,
                    "uploads/residence/proof_of_possession",
                    cancellationToken);
            }

            if (request.UtilityBillAttachment != null)
            {
                utilityBillPath = await _fileStorage.SaveFileAsync(
                    request.UtilityBillAttachment,
                    "uploads/residence/utility_bill",
                    cancellationToken);
            }

            // Create entity
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

            try
            {
                await _smartdhaDbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new CreatePropertyResponse
                {
                    Success = false,
                    Message = "Error saving property",
                    Id = Guid.Empty
                };
            }

            return new CreatePropertyResponse
            {
                Success = true,
                Message = "Property created successfully",
                Id = entity.Id
            };
        }
    }
}
