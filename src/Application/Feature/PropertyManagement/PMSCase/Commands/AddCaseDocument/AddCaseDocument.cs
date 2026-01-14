using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Entities.PMS;
using Microsoft.AspNetCore.Http;

namespace DHAFacilitationAPIs.Application.Feature.PropertyManagement.PMSCase.Commands.AddCaseDocument;
public record AddCaseDocumentCommand(
    Guid CaseId,
    Guid? PrerequisiteDefinitionId,
    IFormFile File
) : IRequest<ApiResult<Guid>>;


public class AddCaseDocumentHandler
    : IRequestHandler<AddCaseDocumentCommand, ApiResult<Guid>>
{
    private readonly IPMSApplicationDbContext _db;
    private readonly IFileStorageService _fileStorage;

    public AddCaseDocumentHandler(
        IPMSApplicationDbContext db,
        IFileStorageService fileStorage)
    {
        _db = db;
        _fileStorage = fileStorage;
    }

    public async Task<ApiResult<Guid>> Handle(
        AddCaseDocumentCommand r,
        CancellationToken ct)
    {
        // 1️⃣ Validate case
        var exists = await _db.Set<PropertyCase>()
            .AnyAsync(x => x.Id == r.CaseId, ct);

        if (!exists)
            return ApiResult<Guid>.Fail("Case not found.");

        if (r.File == null || r.File.Length == 0)
            return ApiResult<Guid>.Fail("File is required.");

        // 2️⃣ Save file (your service)
        var folder = $"pms/cases/{r.CaseId}";
        var savedPath = await _fileStorage.SaveFileAsync(
            r.File,
            folder,
            ct
        );

        // 3️⃣ Save metadata in DB
        var doc = new CaseDocument
        {
            CaseId = r.CaseId,
            PrerequisiteDefinitionId = r.PrerequisiteDefinitionId,
            FileName = r.File.FileName,
            FileUrl = savedPath,
            ContentType = r.File.ContentType,
            FileSize = r.File.Length
        };

        _db.Set<CaseDocument>().Add(doc);
        await _db.SaveChangesAsync(ct);

        return ApiResult<Guid>.Ok(doc.Id, "Document uploaded successfully.");
    }
}

