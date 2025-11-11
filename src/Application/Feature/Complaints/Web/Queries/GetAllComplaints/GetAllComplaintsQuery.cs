using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Enums;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DHAFacilitationAPIs.Application.Feature.Complaints.Web.Queries.GetAllComplaints;
public record GetAllComplaintsQuery(
    ComplaintStatus? Status = null,
    string? CategoryCode = null,
    string? PriorityCode = null
) : IRequest<SuccessResponse<List<ComplaintDto>>>;

// ✅ Query Handler
public class GetAllComplaintsQueryHandler : IRequestHandler<GetAllComplaintsQuery, SuccessResponse<List<ComplaintDto>>>
{
    private readonly IApplicationDbContext _ctx;

    public GetAllComplaintsQueryHandler(IApplicationDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<SuccessResponse<List<ComplaintDto>>> Handle(GetAllComplaintsQuery request, CancellationToken ct)
    {
        var query = _ctx.Complaints
            .Include(c => c.Attachments)
            .Include(c => c.Comments)
            .Include(c => c.History)
            .Where(c => c.IsActive == true && c.IsDeleted == false)
            .AsQueryable();

        // Apply filters if provided
        if (request.Status.HasValue)
            query = query.Where(c => c.Status == request.Status);

        if (!string.IsNullOrEmpty(request.CategoryCode))
            query = query.Where(c => c.CategoryCode == request.CategoryCode);

        if (!string.IsNullOrEmpty(request.PriorityCode))
            query = query.Where(c => c.PriorityCode == request.PriorityCode);


        var complaints = await query
            .OrderByDescending(c => c.Created)
            .Select(c => new ComplaintDto
            {
                Id = c.Id,
                ComplaintNo = c.ComplaintNo,
                Title = c.Title,
                Notes = c.Notes,
                CategoryCode = c.CategoryCode,
                PriorityCode = c.PriorityCode,
                Status = c.Status.ToString(),
                CreatedByUserId = c.CreatedByUserId,
                AssignedToUserId = c.AssignedToUserId,
                Lat = c.Lat,
                Lng = c.Lng,
                AcknowledgedAt = c.AcknowledgedAt,
                ResolvedAt = c.ResolvedAt,
                ClosedAt = c.ClosedAt,
                Created = c.Created,
                Attachments = c.Attachments.Select(a => new ComplaintAttachmentDto
                {
                    Id = a.Id,
                    ImageURL = a.ImageURL,
                    ImageExtension = a.ImageExtension,
                    ImageName = a.ImageName,
                    Description = a.Description
                }).ToList(),
                Comments = c.Comments.Select(cm => new ComplaintCommentDto
                {
                    Id = cm.Id,
                    Text = cm.Text,
                    Visibility = cm.Visibility.ToString(),
                    CreatedByUserId = cm.CreatedByUserId,
                    Created = cm.Created
                }).ToList(),
                History = c.History.Select(h => new ComplaintHistoryDto
                {
                    Id = h.Id,
                    Action = h.Action,
                    FromValue = h.FromValue,
                    ToValue = h.ToValue,
                    ActorUserId = h.ActorUserId,
                    Created = h.Created
                }).ToList()
            })
            .ToListAsync(ct);

        return Success.Ok(complaints);
    }
}
