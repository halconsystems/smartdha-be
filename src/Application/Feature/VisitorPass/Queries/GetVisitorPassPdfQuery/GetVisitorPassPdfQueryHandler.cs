using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Domain.Constants;
using DHAFacilitationAPIs.Domain.Entities.Smartdha;
using Microsoft.AspNetCore.Http;

namespace DHAFacilitationAPIs.Application.Feature.VisitorPass.Queries.GetVisitorPassPdfQuery;
public class GetVisitorPassPdfUrlQueryHandler : IRequestHandler<GetVisitorPassPdfQuery, Result<string>>
{
    private readonly ISmartdhaDbContext _context;
    private readonly IFileStorageService _fileStorage;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IFileCreationService _fileCreation;

    public GetVisitorPassPdfUrlQueryHandler(
        ISmartdhaDbContext context,
        IFileStorageService fileStorage,
        IHttpContextAccessor httpContextAccessor,
        IFileCreationService fileCreation)
    {
        _context = context;
        _fileStorage = fileStorage;
        _httpContextAccessor = httpContextAccessor;
        _fileCreation = fileCreation;
    }

    public async Task<Result<string>> Handle(
        GetVisitorPassPdfQuery request,
        CancellationToken cancellationToken)
    {
        var entity = await _context.VisitorPasses
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
            return Result<string>.Failure(new[] { "Visitor pass not found" });

        // ALWAYS generate new PDF
        var pdfBytes = _fileCreation.GenerateVisitorPassPdf(entity);

        var fileName = $"visitorpass_receipt_{entity.Id}_{DateTime.UtcNow:yyyyMMddHHmmss}.pdf";

        var savedPath = await _fileStorage.SaveFileByteInternalAsync(
            fileBytes: pdfBytes,
            fileName: fileName,
            moduleFolder: FileStorageConstants.Modules.SmartDHA,
            subFolder: "visitorpass/receipt",
            ct: cancellationToken,
            maxBytes: FileStorageConstants.MaxSize.Document,
            allowedExtensions: FileStorageConstants.Extensions.Documents,
            allowedMimeTypes: FileStorageConstants.MimeTypes.Documents
        );

        // Update DB with NEW path
        entity.PdfFilePath = savedPath;

        await _context.SaveChangesAsync(cancellationToken);

        var httpContext = _httpContextAccessor.HttpContext;

        var baseUrl = $"{httpContext?.Request.Scheme}://{httpContext?.Request.Host}";

        var pdfUrl = _fileStorage.GetPublicUrl(savedPath, baseUrl);

        return Result<string>.Success(pdfUrl);
    }
}

