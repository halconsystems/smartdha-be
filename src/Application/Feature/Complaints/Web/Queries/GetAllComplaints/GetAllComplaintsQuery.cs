using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DHAFacilitationAPIs.Application.Feature.Complaints.Web.Queries.GetAllComplaints;
public record GetAllComplaintsQuery() : IRequest<SuccessResponse<List<ComplaintDto>>>;

public class GetAllComplaintsQueryHandler : IRequestHandler<GetAllComplaintsQuery, SuccessResponse<List<ComplaintDto>>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IApplicationDbContext _ctx;
    private readonly IFileStorageService _fileStorageService;

    public GetAllComplaintsQueryHandler(IApplicationDbContext ctx, UserManager<ApplicationUser> userManager, IFileStorageService fileStorageService)
    {
        _ctx = ctx;
        _userManager = userManager;
        _fileStorageService = fileStorageService;
    }

    public async Task<SuccessResponse<List<ComplaintDto>>> Handle(GetAllComplaintsQuery request, CancellationToken ct)
    {
        var complaints = await _ctx.Complaints
                .Include(c => c.Attachments)
                .Include(c => c.History)
                .Where(c => c.IsActive == true && c.IsDeleted == false)
                .OrderByDescending(c => c.Created)
                .Select(c => new
                {
                    Complaint = c,
                    c.CreatedByUserId
                })
                .ToListAsync(ct);

        var userIds = complaints
            .Select(c => c.CreatedByUserId)
            .Where(id => id != null)
            .Distinct()
            .ToList();

        var users = await _userManager.Users
            .Where(u => userIds.Contains(u.Id) && u.IsActive == true && u.IsDeleted == false)
            .Select(u => new { u.Id, u.Name, u.CNIC, u.MobileNo})
            .ToListAsync(ct);

        var result = complaints.Select(c =>
        {
            var user = users.FirstOrDefault(u => u.Id == c.CreatedByUserId);
            return new ComplaintDto
            {
                Id = c.Complaint.Id,
                ComplaintNo = c.Complaint.ComplaintNo,
                Title = c.Complaint.Title,
                Notes = c.Complaint.Notes,
                CategoryCode = c.Complaint.CategoryCode,
                PriorityCode = c.Complaint.PriorityCode,
                Status = c.Complaint.Status.ToString(),
                CreatedByUserId = c.Complaint.CreatedByUserId,
                CreatedByUserName = user?.Name ?? "N/A",
                CreatedByUserCNIC = user?.CNIC ?? "N/A",
                CreatedByUserMobileNo = user?.MobileNo ?? "N/A",
                AssignedToUserId = c.Complaint.AssignedToUserId,
                Lat = c.Complaint.Lat,
                Lng = c.Complaint.Lng,
                AdminRemarks = c.Complaint.AdminRemarks,
                AcknowledgedAt = c.Complaint.AcknowledgedAt,
                ResolvedAt = c.Complaint.ResolvedAt,
                ClosedAt = c.Complaint.ClosedAt,
                Created = c.Complaint.Created,
                Attachments = c.Complaint.Attachments.Select(a => new ComplaintAttachmentDto
                {
                    Id = a.Id,
                    ImageURL = _fileStorageService.GetPublicUrlOfComplaint(a.ImageURL),
                    ImageExtension = a.ImageExtension,
                    ImageName = a.ImageName,
                    Description = a.Description
                }).ToList(),
                History = c.Complaint.History.Select(h => new ComplaintHistoryDto
                {
                    Id = h.Id,
                    Action = h.Action,
                    FromValue = h.FromValue,
                    ToValue = h.ToValue,
                    ActorUserId = h.ActorUserId,
                    Created = h.Created
                }).ToList()
            };
        }).ToList();

        return Success.Ok(result);
    }
}
