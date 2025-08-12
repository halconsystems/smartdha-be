using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.User.Queries.GetAllNonMemberPurposes;
using DHAFacilitationAPIs.Application.ViewModels;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Application.Feature.NonMember.Queries.GetNonMemberRequests;
public record NonMemberRequestsQuery : IRequest<SuccessResponse<List<NonMemberRequestsDto>>>
{
    public Domain.Enums.VerificationStatus status { get; set; }
}

public class NonMemberRequestQueryHandler : IRequestHandler<NonMemberRequestsQuery, SuccessResponse<List<NonMemberRequestsDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public NonMemberRequestQueryHandler(IApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }


    public async Task<SuccessResponse<List<NonMemberRequestsDto>>> Handle(
    NonMemberRequestsQuery request,
    CancellationToken cancellationToken)
    {
        // 1) Get the verification rows (minimal shape)
        var query = _context.NonMemberVerifications
    .AsNoTracking()
    .Where(p => p.IsDeleted == false || p.IsDeleted == null);

        if ((int)request.status != -1)
        {
            query = query.Where(p => p.Status == request.status);
        }

        var pendingRequests = await query
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

        if (pendingRequests.Count == 0)
            return new SuccessResponse<List<NonMemberRequestsDto>>(new List<NonMemberRequestsDto>(),
                "No matching requests found.", "NonMember-Requests");

        var userIds = pendingRequests.Select(x => x.UserId).Where(x => x != null).Distinct().ToList();
        var requestIds = pendingRequests.Select(x => x.Id).ToList();

        // 2) Pull user profiles (Identity) for those UserIds
        // If you keep Identity users in the same DbContext:
        var usersDict = await _userManager.Users
    .AsNoTracking()
    .Where(u => userIds.Contains(u.Id))
    .Select(u => new
    {
        u.Id,
        u.Name,
        PhoneNumber = u.PhoneNumber ?? u.MobileNo,
        u.Email
    })
    .ToDictionaryAsync(
        x => x.Id,
        x => new { x.Name, x.PhoneNumber, x.Email },
        cancellationToken
    );


        // If you prefer _userManager.Users instead, replace the query above with:
        // var usersDict = await _userManager.Users
        //     .AsNoTracking()
        //     .Where(u => userIds.Contains(u.Id))
        //     .Select(u => new { u.Id, u.Name, PhoneNumber = u.PhoneNumber ?? u.MobileNo, u.Email })
        //     .ToDictionaryAsync(x => x.Id, x => new { x.Name, x.PhoneNumber, x.Email }, cancellationToken);

        // 3) All documents for those verifications
        var allDocs = await _context.NonMemberVerificationDocuments
            .AsNoTracking()
            .Where(doc => requestIds.Contains(doc.VerificationId) && (doc.IsDeleted == false || doc.IsDeleted == null))
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

        // 4) Purpose titles per user (active + not deleted)
        var userPurposes = await (
            from ump in _context.UserMembershipPurposes.AsNoTracking()
            join mp in _context.MembershipPurposes.AsNoTracking()
                on ump.PurposeId equals mp.Id
            where userIds.Contains(ump.UserId)
                  && (ump.IsDeleted == false) && (ump.IsActive == true)
                  && (mp.IsDeleted == false) && (mp.IsActive == true)
            select new { ump.UserId, mp.Title }
        ).ToListAsync(cancellationToken);

        var purposesByUser = userPurposes
            .GroupBy(x => x.UserId)
            .ToDictionary(
                g => g.Key,
                g => string.Join(", ", g.Select(x => x.Title).Distinct())
            );

        // 5) Build DTOs
        var result = pendingRequests.Select(p =>
        {
            usersDict.TryGetValue(p.UserId, out var u);
            var docs = allDocs.Where(d => d.VerificationId == p.Id).ToList();
            var purposeTitles = purposesByUser.TryGetValue(p.UserId, out var pt) ? pt : string.Empty;

            return new NonMemberRequestsDto
            {
                Id = p.Id,
                UserId = p.UserId,
                Name = u?.Name ?? string.Empty,
                PhoneNumber = u?.PhoneNumber ?? string.Empty,
                Email = u?.Email ?? string.Empty,
                CNIC = p.CNIC,
                Remarks = p.Remarks ?? string.Empty,
                Status = p.Status,
                ApprovedAt = p.ApprovedAt,
                ApprovedBy = p.ApprovedBy,
                // If you only keep titles, PurposeId isn't meaningful when multiple purposes exist.
                // You can remove it from DTO or set Guid.Empty.
                PurposeId = Guid.Empty,
                PurposeTitles = purposeTitles,
                VerificationDocs = docs
            };
        })
        // optional: sort (newest first, or by status)
        .OrderByDescending(x => x.ApprovedAt ?? DateTime.MinValue)
        .ToList();

        return new SuccessResponse<List<NonMemberRequestsDto>>(
            result,
            "Non-member requests with user profile, documents, and purpose details.",
            "NonMember-Requests");
    }



    //public async Task<SuccessResponse<List<NonMemberRequestsDto>>> Handle(NonMemberRequestsQuery request, CancellationToken cancellationToken)
    //{



    //    // 1. Get all pending verifications (minimal projection)
    //    var pendingRequests = await _context.NonMemberVerifications
    //        .Where(p => p.Status == request.status)
    //        .Select(p => new
    //        {
    //            p.Id,
    //            p.UserId,
    //            p.CNIC,
    //            p.Remarks,
    //            p.Status,
    //            p.ApprovedAt,
    //            p.ApprovedBy
    //        })
    //        .ToListAsync(cancellationToken);

    //    var userIds = pendingRequests.Select(x => x.UserId).Distinct().ToList();
    //    var requestIds = pendingRequests.Select(x => x.Id).ToList();

    //    // 2. Get all documents for those verifications
    //    var allDocs = await _context.NonMemberVerificationDocuments
    //        .Where(doc => requestIds.Contains(doc.VerificationId))
    //        .Select(doc => new NonMemberVerificationDocsDto
    //        {
    //            VerificationId = doc.VerificationId,
    //            CNICFrontImagePath = doc.CNICFrontImagePath,
    //            CNICBackImagePath = doc.CNICBackImagePath,
    //            SupportingDocumentPath = doc.SupportingDocumentPath,
    //            DocumentType = doc.DocumentType,
    //            OriginalFileName = doc.OriginalFileName
    //        })
    //        .ToListAsync(cancellationToken);

    //    // 3. Get all purposes associated with each user
    //    var userPurposes = await (
    //        from ump in _context.UserMembershipPurposes
    //        join mp in _context.MembershipPurposes on ump.PurposeId equals mp.Id
    //        where userIds.Contains(ump.UserId) && (ump.IsDeleted==false) && (ump.IsActive==true) && (mp.IsDeleted==false) && (mp.IsActive==true)
    //        select new
    //        {
    //            ump.UserId,
    //            PurposeTitle = mp.Title
    //        })
    //        .ToListAsync(cancellationToken);

    //    var purposesByUser = userPurposes
    //        .GroupBy(x => x.UserId)
    //        .ToDictionary(
    //            g => g.Key,
    //            g => string.Join(", ", g.Select(x => x.PurposeTitle).Distinct())
    //        );

    //    // 4. Build final DTO list
    //    var result = pendingRequests.Select(p => new NonMemberRequestsDto
    //    {
    //        Id = p.Id,
    //        UserId = p.UserId,
    //        CNIC = p.CNIC,
    //        Remarks = p.Remarks,
    //        Status = p.Status,
    //        ApprovedAt = p.ApprovedAt,
    //        ApprovedBy = p.ApprovedBy,
    //        VerificationDocs = allDocs.Where(d => d.VerificationId == p.Id).ToList(),
    //        PurposeTitles = purposesByUser.ContainsKey(p.UserId) ? purposesByUser[p.UserId] : string.Empty
    //    }).ToList();

    //    return new SuccessResponse<List<NonMemberRequestsDto>>(
    //        result,
    //        "Non-member pending requests with documents and purpose details.",
    //        "NonMember-Requests"
    //    );
    //}


    //public async Task<SuccessResponse<List<NonMemberRequestsDto>>> Handle(NonMemberRequestsQuery request, CancellationToken cancellationToken)
    //{
    //    // 1. Get all pending verifications (minimal projection with Purpose)
    //    var pendingRequests = await _context.NonMemberVerifications
    //        .Where(p => p.Status == request.status)
    //        .Select(p => new
    //        {
    //            p.Id,
    //            p.UserId,
    //            p.CNIC,
    //            p.Remarks,
    //            p.Status,
    //            p.ApprovedAt,
    //            p.ApprovedBy
    //        })
    //        .ToListAsync(cancellationToken);

    //    // 2. Get related documents in bulk
    //    var requestIds = pendingRequests.Select(x => x.Id).ToList();

    //    var allDocs = await _context.NonMemberVerificationDocuments
    //        .Where(doc => requestIds.Contains(doc.VerificationId))
    //        .Select(doc => new NonMemberVerificationDocsDto
    //        {
    //            VerificationId = doc.VerificationId,
    //            CNICFrontImagePath = doc.CNICFrontImagePath,
    //            CNICBackImagePath = doc.CNICBackImagePath,
    //            SupportingDocumentPath = doc.SupportingDocumentPath,
    //            DocumentType = doc.DocumentType,
    //            OriginalFileName = doc.OriginalFileName
    //        })
    //        .ToListAsync(cancellationToken);

    //    // 3. Map DTOs and group related documents
    //    var result = pendingRequests.Select(p => new NonMemberRequestsDto
    //    {
    //        Id = p.Id,
    //        UserId = p.UserId,
    //        CNIC = p.CNIC,
    //        Remarks = p.Remarks,
    //        Status = p.Status,
    //        ApprovedAt = p.ApprovedAt,
    //        ApprovedBy = p.ApprovedBy,
    //        VerificationDocs = allDocs.Where(d => d.VerificationId == p.Id).ToList()
    //    }).ToList();

    //    return new SuccessResponse<List<NonMemberRequestsDto>>(
    //        result,
    //        "Non-member pending requests with documents and purpose details.",
    //        "NonMember-Requests"
    //    );
    //}



}
