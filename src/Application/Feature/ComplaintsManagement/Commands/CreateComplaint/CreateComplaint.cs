using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.ComplaintsManagement.Commands.CreateComplaint;
public record CreateComplaintCommand(CreateComplaintRequest Request) : IRequest<string>;



public class CreateComplaintCommandHandler(
    IApplicationDbContext _context,
    IFileStorageService _fileStorage,
    ICurrentUserService _currentUser
) : IRequestHandler<CreateComplaintCommand, string>
{
    public async Task<string> Handle(CreateComplaintCommand command, CancellationToken cancellationToken)
    {
        var dto = command.Request;
        var userId = _currentUser.UserId.ToString() ?? "anonymous";

        // 🔹 Generate ComplaintNo (CMP-YYYY-xxxxx)
        var count = await _context.Complaints.CountAsync(cancellationToken) + 1;
        var complaintNo = $"CMP-{DateTime.Now:yyyy}-{count:D5}";

        var entity = new Complaint
        {
            ComplaintNo = complaintNo,
            Title = dto.Title,
            Notes = dto.Notes,
            CategoryCode = dto.CategoryCode,
            PriorityCode = dto.PriorityCode,
            CreatedByUserId = userId,
            Created = DateTime.Now,
            IsActive = true,
            IsDeleted = false,
            Lat = dto.Lat,
            Lng = dto.Lng,
            Status = ComplaintStatus.New
        };

        _context.Complaints.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        // 🔹 Use your existing SaveFilesAsync()
        if (dto.Images != null && dto.Images.Any())
        {
            var allowedExt = new[] { ".jpg", ".jpeg", ".png", ".webp" };

            // SaveFilesAsync returns list of image URLs
            var savedPaths = await _fileStorage.SaveFilesAsync(
                dto.Images,
                $"complaints/{entity.Id}",
                cancellationToken,
                allowedExtensions: allowedExt
            );

            // Map saved paths to ComplaintAttachment records
            foreach (var path in savedPaths)
            {
                var ext = Path.GetExtension(path);
                var name = Path.GetFileNameWithoutExtension(path);

                _context.ComplaintAttachments.Add(new ComplaintAttachment
                {
                    ComplaintId = entity.Id, // ✅ Guid directly
                    ImageURL = path,
                    ImageExtension = ext,
                    ImageName = name,
                    Created = DateTime.UtcNow,
                    CreatedBy = userId,
                    IsActive = true,
                    IsDeleted = false
                });

            }
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch(Exception ex)
            {
                // Log the exception (you can use a logging framework here)
                Console.WriteLine($"Error saving complaint attachments: {ex.Message}");
            }
        }


        return entity.ComplaintNo;
    }
}

