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

        if (string.IsNullOrEmpty(entity.PdfFilePath))
        {
            // Generate PDF bytes using full entity, not file path
            var pdfBytes = _fileCreation.GenerateVisitorPassPdf(entity);

            // Generate file name
            var fileName = $"visitorpass_receipt_{entity.Id}.pdf";

            // Save file using storage service
            var savedPath = await _fileStorage.SaveFileByteInternalAsync(
                pdfBytes,
                fileName,
                moduleFolder: FileStorageConstants.Modules.SmartDHA,
                subFolder: "visitorpass/receipt",
                ct: cancellationToken,
                maxBytes: FileStorageConstants.MaxSize.Document,
                allowedExtensions: FileStorageConstants.Extensions.Documents,
                allowedMimeTypes: FileStorageConstants.MimeTypes.Documents
                );

            // Save path in DB
            entity.PdfFilePath = savedPath;

            await _context.SaveChangesAsync(cancellationToken);
        }

        if (string.IsNullOrEmpty(entity.PdfFilePath))
            return Result<string>.Failure(new[] { "PDF not available" });

        var httpContext = _httpContextAccessor.HttpContext;

        var baseUrl = $"{httpContext?.Request.Scheme}://{httpContext?.Request.Host}";

        var pdfUrl = _fileStorage.GetPublicUrl(entity.PdfFilePath, baseUrl);

        return Result<string>.Success(pdfUrl);
    }
}

