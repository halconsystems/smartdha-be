using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.Vehicles.Command;
using DHAFacilitationAPIs.Application.Feature.Vehicles.Commands.CreateVehicleCommandHandler;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;


namespace DHAFacilitationAPIs.Application.Feature.Vehicles.Commands.CreateVehicleCommandHandler;

public class CreateVehicleCommandHandler : IRequestHandler<CreateVehicleCommand, CreateVehicleResponse>
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

    public async Task<CreateVehicleResponse> Handle(CreateVehicleCommand request, CancellationToken cancellationToken)
    {
        var response = new CreateVehicleResponse();

        string? attachmentPath = null;
        if (request.Attachment != null)
        {
            attachmentPath = await _fileStorage.SaveFileAsync(
                request.Attachment,
                "uploads/vehicles",
                cancellationToken);
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
            ValidUpTo = request.ValidUpTo,
        };

        await _smartdhaDbContext.Vehicles.AddAsync(entity, cancellationToken);
        await _smartdhaDbContext.SaveChangesAsync(cancellationToken);

        response.Success = true;
        response.Message = "Vehicle added successfully.";
        response.Id = entity.Id;

        return response;
    }
}



