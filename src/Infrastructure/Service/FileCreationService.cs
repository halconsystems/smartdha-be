using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.VisitorPass.Queries.GetVisitorPassbyId;
using DHAFacilitationAPIs.Domain.Entities.Smartdha;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Document = QuestPDF.Fluent.Document;


namespace DHAFacilitationAPIs.Infrastructure.Service;
public class FileCreationService : IFileCreationService
{
    public byte[] GenerateVisitorPassPdf(VisitorPass visitorPass)
    {
        var pdfBytes = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(20);

                page.Header()
                    .Text("Visitor Pass Receipt")
                    .FontSize(20)
                    .Bold()
                    .AlignCenter();

                page.Content().Column(column =>
                {
                    column.Spacing(10);

                    column.Item().Text($"Name: {visitorPass.Name}");
                    column.Item().Text($"CNIC: {visitorPass.CNIC}");
                    column.Item().Text($"Vehicle License Plate: {visitorPass.VehicleLicensePlate}");
                    column.Item().Text($"Vehicle License No: {visitorPass.VehicleLicenseNo}");
                    column.Item().Text($"Pass Type: {visitorPass.VisitorPassType}");
                    column.Item().Text($"Valid From: {visitorPass.ValidFrom:dd MMM yyyy}");
                    column.Item().Text($"Valid To: {visitorPass.ValidTo:dd MMM yyyy}");

                    if (!string.IsNullOrEmpty(visitorPass.QRCode))
                    {
                        column.Item().Text($"QR Code: {visitorPass.QRCode}");
                    }
                });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Generated on ");
                        x.Span(DateTime.Now.ToString("dd MMM yyyy HH:mm"));
                    });
            });
        }).GeneratePdf();

        return pdfBytes;
    }
}
