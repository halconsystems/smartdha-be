using System;
using System.Threading;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.UserFamily.Commands.UpdateUserFamilyCommandHandler;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace DHAFacilitationAPIs.Application.Feature.UserFamily.UserFamilyCommands.UpdateUserFamilyCommandHandler
{
    public class UpdateUserFamilyCommandHandler
        : IRequestHandler<UpdateUserFamilyCommand, Result<UpdateUserFamilyResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ISmartdhaDbContext _smartDhaContext;
        private readonly IFileStorageService _fileStorage;

        public UpdateUserFamilyCommandHandler(
            IApplicationDbContext context,
            ISmartdhaDbContext smartDhaContext,
            IFileStorageService fileStorage)
        {
            _context = context;
            _smartDhaContext = smartDhaContext;
            _fileStorage = fileStorage;
        }

        public async Task<Result<UpdateUserFamilyResponse>> Handle(UpdateUserFamilyCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var response = new UpdateUserFamilyResponse();
                var entity = await _smartDhaContext.UserFamilies
                    .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

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

                    var newImagePath = await _fileStorage.SaveFileAsync(
                        request.ProfilePicture,
                        "uploads/smartdha/userfamily",
                        cancellationToken);

                    entity.ProfilePicture = newImagePath;
                }

                entity.Name = request.Name ?? "";
                entity.Relation = (RelationUserFamily)request.Relation;
                entity.PhoneNumber = request.PhoneNo;
                entity.Cnic = request.CNIC;
                entity.DateOfBirth = request.DOB.Date;
                await _smartDhaContext.SaveChangesAsync(cancellationToken);

                response.Name = entity.Name;
                response.CNIC = entity.Cnic ?? "";
                response.PhoneNo = entity.PhoneNumber ?? "";
                response.DOB = entity.DateOfBirth;
                response.Relation = entity.Relation;
                response.ProfilePicture = entity.ProfilePicture;
                return Result<UpdateUserFamilyResponse>.Success(response);
            }
            catch (Exception ex)
            {
                return Result<UpdateUserFamilyResponse>.Failure(new[] { ex.Message });
            }
        }
    }
}
