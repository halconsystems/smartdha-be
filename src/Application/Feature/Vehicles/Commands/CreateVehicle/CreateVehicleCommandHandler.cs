using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.UserFamily.Commands.AddUserFamilyCommandHandler;
using DHAFacilitationAPIs.Application.Feature.Vehicles.Command;
using DHAFacilitationAPIs.Application.Feature.Vehicles.Commands.CreateVehicleCommandHandler;
using DHAFacilitationAPIs.Domain.Constants;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;


namespace DHAFacilitationAPIs.Application.Feature.Vehicles.Commands.CreateVehicleCommandHandler;

public class CreateVehicleCommandHandler : IRequestHandler<CreateVehicleCommand, Result<CreateVehicleResponse>>
{
    private readonly ISmartdhaDbContext _smartdhaDbContext;
    private readonly IFileStorageService _fileStorage;

    public CreateVehicleCommandHandler(
        ISmartdhaDbContext smartdhaDbContext,
        IFileStorageService fileStorage)
    {
        _smartdhaDbContext = smartdhaDbContext;
        _fileStorage = fileStorage;
    }

    public async Task<Result<CreateVehicleResponse>> Handle(CreateVehicleCommand request, CancellationToken cancellationToken)
    {
        var response = new CreateVehicleResponse();

        string? attachmentPath = null;
        if (request.Attachment != null)
        {
            //attachmentPath = await _fileStorage.SaveFileAsync(
            //    request.Attachment,
            //    "uploads/vehicles",
            //    cancellationToken);
            attachmentPath = await _fileStorage.SaveFileInternalAsync(
            file: request.Attachment,
            moduleFolder: FileStorageConstants.Modules.SmartDHA,
            subFolder: "vehicles",
            ct: cancellationToken,
            maxBytes: FileStorageConstants.MaxSize.Image,
            allowedExtensions: FileStorageConstants.Extensions.Images,
            allowedMimeTypes: FileStorageConstants.MimeTypes.Images);
        }

        var entity = new Domain.Entities.Smartdha.Vehicle
        {
            LicenseNo = request.LicenseNo,
            License = request.License,
            Year = request.Year,
            Color = request.Color,
            Make = request.Make,
            Model = request.Model,
            Attachment = attachmentPath,
            ETagId = request.ETagId,
            ValidTo = request.ValidTo,
        };

        await _smartdhaDbContext.Vehicles.AddAsync(entity, cancellationToken);
        await _smartdhaDbContext.SaveChangesAsync(cancellationToken);
        response.Id = entity.Id;

        return Result<CreateVehicleResponse>.Success(response);
    }
}



