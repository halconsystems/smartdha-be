using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Constants;
using DHAFacilitationAPIs.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DHAFacilitationAPIs.Application.Feature.Worker.Commands.AddWorker;
public class AddWorkerCommandHandler : IRequestHandler<AddWorkerCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ISmartdhaDbContext _smartdhaDbContext;
    private readonly IFileStorageService _fileStorage;
    public AddWorkerCommandHandler(ISmartdhaDbContext smartdhaDbContext, IApplicationDbContext context, IFileStorageService fileStorage)
    {
        _smartdhaDbContext = smartdhaDbContext;
        _context = context;
        _fileStorage = fileStorage;
    }

    public async Task<Result<Guid>> Handle(AddWorkerCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var profilePath = request.ProfilePicture != null ? await _fileStorage.SaveFileInternalAsync(
                file: request.ProfilePicture,
                moduleFolder: FileStorageConstants.Modules.SmartDHA,
                subFolder: "worker/profile",
                ct: cancellationToken,
                maxBytes: FileStorageConstants.MaxSize.Image,
                allowedExtensions: FileStorageConstants.Extensions.Images,
                allowedMimeTypes: FileStorageConstants.MimeTypes.Images) : null;

            var cnicFrontPath = request.CnicFront != null ? await _fileStorage.SaveFileInternalAsync(
                file: request.CnicFront,
                moduleFolder: FileStorageConstants.Modules.SmartDHA,
                subFolder: "worker/cnic/front",
                ct: cancellationToken,
                maxBytes: FileStorageConstants.MaxSize.Image,
                allowedExtensions: FileStorageConstants.Extensions.Images,
                allowedMimeTypes: FileStorageConstants.MimeTypes.Images) : null;

            var cnicBackPath = request.CnicBack != null ? await _fileStorage.SaveFileInternalAsync(
                file: request.CnicBack,
                moduleFolder: FileStorageConstants.Modules.SmartDHA,
                subFolder: "worker/cnic/back",
                ct: cancellationToken,
                maxBytes: FileStorageConstants.MaxSize.Image,
                allowedExtensions: FileStorageConstants.Extensions.Images,
                allowedMimeTypes: FileStorageConstants.MimeTypes.Images) : null;

            var policeVerificationDocPath = request.PoliceVerificationAttachment != null ? await _fileStorage.SaveFileInternalAsync(
                file: request.PoliceVerificationAttachment,
                moduleFolder: FileStorageConstants.Modules.SmartDHA,
                subFolder: "worker/policeVerification",
                ct: cancellationToken,
                maxBytes: FileStorageConstants.MaxSize.Image,
                allowedExtensions: FileStorageConstants.Extensions.Images,
                allowedMimeTypes: FileStorageConstants.MimeTypes.Images) : null;

            var entity = new Domain.Entities.Smartdha.Worker
            {
                Name = request.Name,
                JobType = (JobType)request.JobType,
                DateOfBirth = request.DOB.Date,
                CNIC = request.CNIC,
                FatherOrHusbandName = request.FatherHusbandName,
                ProfilePicture = profilePath,
                CnicBack = cnicBackPath,
                CnicFront = cnicFrontPath,
                PoliceVerificationAttachment = policeVerificationDocPath,
                PhoneNumber = request.PhoneNo,
                WorkerCardDeliveryType = (WorkerCardDeliveryType)request.WorkerCardDeliveryType,
                Created = DateTime.UtcNow
            };

            await _smartdhaDbContext.Workers.AddAsync(entity, cancellationToken);
            await _smartdhaDbContext.SaveChangesAsync(cancellationToken);
            return Result<Guid>.Success(entity.Id);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure(new[] { ex.Message});
        }
    }
}
