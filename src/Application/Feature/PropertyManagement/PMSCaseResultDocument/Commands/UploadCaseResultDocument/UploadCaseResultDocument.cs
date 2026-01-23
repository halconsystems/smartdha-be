using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCase.Queries.GetCaseWorkflowHierarchy;
using DHAFacilitationAPIs.Domain.Entities.PMS;
using Microsoft.AspNetCore.Http;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCaseResultDocument.Commands.UploadCaseResultDocument;

public class CaseResultDocumentDto
{
    public Guid Id { get; set; }
    public string DocumentType { get; set; } = default!;
    public string FileName { get; set; } = default!;
    public string DownloadUrl { get; set; } = default!;
}

public record UploadCaseResultDocumentCommand(
    Guid CaseId,
    IFormFile File
) : IRequest<ApiResult<CaseResultDocumentDto>>;
public class UploadCaseResultDocumentHandler
    : IRequestHandler<UploadCaseResultDocumentCommand, ApiResult<CaseResultDocumentDto>>
{
    private readonly IPMSApplicationDbContext _db;
    private readonly IFileStorageService _fileStorage;

    public UploadCaseResultDocumentHandler(
        IPMSApplicationDbContext db,
        IFileStorageService fileStorage)
    {
        _db = db;
        _fileStorage = fileStorage;
    }

    public async Task<ApiResult<CaseResultDocumentDto>> Handle(
        UploadCaseResultDocumentCommand r,
        CancellationToken ct)
    {
        // 1️⃣ Validate Case
        var c = await _db.Set<PropertyCase>()
            .FirstOrDefaultAsync(x => x.Id == r.CaseId && x.IsDeleted != true, ct);

        if (c == null)
            return ApiResult<CaseResultDocumentDto>.Fail("Case not found.");

        // 2️⃣ Upload file
        var fileUrl = await _fileStorage.SavePMSDocumentAsync(
            r.File,
            $"pms/cases/{c.Id}/result",
            ct,
            maxBytes: 20 * 1024 * 1024,
            allowedExtensions: new[] { ".pdf" }
        );

        // 3️⃣ Save document record
        var entity = new CaseResultDocument
        {
            CaseId = c.Id,
            DocumentType = r.File.ContentType,
            FileName = r.File.FileName,
            FilePath = fileUrl,
            ContentType = r.File.ContentType,
            FileSize = r.File.Length,
            IsFinal = true
        };

        _db.Set<CaseResultDocument>().Add(entity);
        await _db.SaveChangesAsync(ct);

        // 4️⃣ Return response
        return ApiResult<CaseResultDocumentDto>.Ok(new CaseResultDocumentDto
        {
            Id = entity.Id,
            DocumentType = entity.DocumentType,
            FileName = entity.FileName,
            DownloadUrl = fileUrl
        }, "Document uploaded successfully");
    }
}

