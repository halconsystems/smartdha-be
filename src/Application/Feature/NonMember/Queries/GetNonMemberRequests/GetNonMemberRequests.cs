using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.User.Queries.GetAllNonMemberPurposes;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;

namespace DHAFacilitationAPIs.Application.Feature.NonMember.Queries.GetNonMemberRequests;
public record NonMemberRequestsQuery : IRequest<SuccessResponse<List<NonMemberRequestsDto>>>;

public class NonMemberRequestQueryHandler : IRequestHandler<NonMemberRequestsQuery, SuccessResponse<List<NonMemberRequestsDto>>>
{
    private readonly IApplicationDbContext _context;

    public NonMemberRequestQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<SuccessResponse<List<NonMemberRequestsDto>>> Handle(NonMemberRequestsQuery request, CancellationToken cancellationToken)
    {
        // 1. Get all pending verifications (minimal projection with Purpose)
        var pendingRequests = await _context.NonMemberVerifications
            .Where(p => p.Status == Domain.Enums.VerificationStatus.Pending)
            .Select(p => new
            {
                p.Id,
                p.UserId,
                p.CNIC,
                p.Remarks,
                p.Status,
                p.ApprovedAt,
                p.ApprovedBy
            })
            .ToListAsync(cancellationToken);

        // 2. Get related documents in bulk
        var requestIds = pendingRequests.Select(x => x.Id).ToList();

        var allDocs = await _context.NonMemberVerificationDocuments
            .Where(doc => requestIds.Contains(doc.VerificationId))
            .Select(doc => new NonMemberVerificationDocsDto
            {
                VerificationId = doc.VerificationId,
                CNICFrontImagePath = doc.CNICFrontImagePath,
                CNICBackImagePath = doc.CNICBackImagePath,
                SupportingDocumentPath = doc.SupportingDocumentPath,
                DocumentType = doc.DocumentType,
                OriginalFileName = doc.OriginalFileName
            })
            .ToListAsync(cancellationToken);

        // 3. Map DTOs and group related documents
        var result = pendingRequests.Select(p => new NonMemberRequestsDto
        {
            Id = p.Id,
            UserId = p.UserId,
            CNIC = p.CNIC,
            Remarks = p.Remarks,
            Status = p.Status,
            ApprovedAt = p.ApprovedAt,
            ApprovedBy = p.ApprovedBy,
            VerificationDocs = allDocs.Where(d => d.VerificationId == p.Id).ToList()
        }).ToList();

        return new SuccessResponse<List<NonMemberRequestsDto>>(
            result,
            "Non-member pending requests with documents and purpose details.",
            "NonMember-Requests"
        );
    }


  
}
