using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.UserFamily.Commands.UpdateUserFamilyCommandHandler;
using DHAFacilitationAPIs.Domain.Constants;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Mvc.DataAnnotations;

namespace DHAFacilitationAPIs.Application.Feature.Worker.Commands.UpdateWorker;
public class UpdateWorkerCommandHandler : IRequestHandler<UpdateWorkerCommand, Result<UpdateWorkerResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ISmartdhaDbContext _smartdhaDbContext;
    private readonly IFileStorageService _fileStorage;
    public UpdateWorkerCommandHandler(IApplicationDbContext context, ISmartdhaDbContext smartdhaDbContext, IFileStorageService fileStorageService)
    {
        _smartdhaDbContext = smartdhaDbContext;
        _fileStorage = fileStorageService;
        _context = context;
    }
    public async Task<Result<UpdateWorkerResponse>> Handle(UpdateWorkerCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var response = new UpdateWorkerResponse();
            var entity = await _smartdhaDbContext.Workers
                        .FirstOrDefaultAsync(x => x.Id == request.WorkerId, cancellationToken);

            if (entity == null)
                throw new Exception("No Record Found!");

            // If a new profile picture is provided, delete the old file (if any) and save the new one
            if (request.ProfilePicture != null && request.ProfilePicture.Length > 0)
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(entity.ProfilePicture))
                    {
                        await _fileStorage.DeleteFileAsync(entity.ProfilePicture, cancellationToken);
                    }
                }
                catch
                {
                }

                var newImagePath = await _fileStorage.SaveFileInternalAsync(
                file: request.ProfilePicture,
                moduleFolder: FileStorageConstants.Modules.SmartDHA,
                subFolder: "worker/profile",
                ct: cancellationToken,
                maxBytes: FileStorageConstants.MaxSize.Image,
                allowedExtensions: FileStorageConstants.Extensions.Images,
                allowedMimeTypes: FileStorageConstants.MimeTypes.Images);

                entity.ProfilePicture = newImagePath;
            }
            if (request.CnicBack != null && request.CnicBack.Length > 0)
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(entity.CnicBack))
                    {
                        await _fileStorage.DeleteFileAsync(entity.CnicBack, cancellationToken);
                    }
                }
                catch
                {
                }

                var newCnicBackImagePath = await _fileStorage.SaveFileInternalAsync(
                file: request.CnicBack,
                moduleFolder: FileStorageConstants.Modules.SmartDHA,
                subFolder: "worker/cnic/back",
                ct: cancellationToken,
                maxBytes: FileStorageConstants.MaxSize.Image,
                allowedExtensions: FileStorageConstants.Extensions.Images,
                allowedMimeTypes: FileStorageConstants.MimeTypes.Images);

                entity.CnicBack = newCnicBackImagePath;
            }
            if (request.CnicFront != null && request.CnicFront.Length > 0)
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(entity.CnicFront))
                    {
                        await _fileStorage.DeleteFileAsync(entity.CnicFront, cancellationToken);
                    }
                }
                catch
                {
                }

                var newCnicFrontImagePath = await _fileStorage.SaveFileInternalAsync(
                file: request.CnicFront,
                moduleFolder: FileStorageConstants.Modules.SmartDHA,
                subFolder: "worker/cnic/front",
                ct: cancellationToken,
                maxBytes: FileStorageConstants.MaxSize.Image,
                allowedExtensions: FileStorageConstants.Extensions.Images,
                allowedMimeTypes: FileStorageConstants.MimeTypes.Images);

                entity.CnicBack = newCnicFrontImagePath;
            }
            entity.Name = request.Name ?? entity.Name;
            entity.FatherOrHusbandName = request.FatherHusbandName ?? entity.FatherOrHusbandName;
            entity.JobType = request.JobType.HasValue ? (JobType)request.JobType.Value : entity.JobType;
            entity.PhoneNumber = request.PhoneNo ?? entity.PhoneNumber;
            entity.CNIC = request.CNIC ?? entity.CNIC;
            entity.DateOfBirth = request.DOB?.Date ?? entity.DateOfBirth;
            entity.PoliceVerification = request.PoliceVerification;
            try
            {

                await _smartdhaDbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving changes to the database: " + ex.Message);
            }   

            response.WorkerId = entity.Id;
            response.Name = entity.Name;
            response.CNIC = entity.CNIC ?? "";
            response.PhoneNo = entity.PhoneNumber ?? "";
            response.DOB = entity.DateOfBirth;
            response.JobType = entity.JobType;
            response.ProfilePicture = entity.ProfilePicture;
            response.CnicFront = entity.CnicFront;
            response.CnicBack = entity.CnicBack;
            return Result<UpdateWorkerResponse>.Success(response);
        }

        catch (Exception ex)
        {
            return Result<UpdateWorkerResponse>.Failure(new[] { ex.Message});
        }
    }

}
