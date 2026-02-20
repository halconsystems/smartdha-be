using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Feature.VisitorPass.Queries.GetVisitorPassbyId;
using DHAFacilitationAPIs.Domain.Entities.Smartdha;
using DHAFacilitationAPIs.Domain.Enums;
using QRCoder;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Document = QuestPDF.Fluent.Document;


namespace DHAFacilitationAPIs.Infrastructure.Service;
public class FileCreationService : IFileCreationService
{
    public byte[] GenerateVisitorPassPdf(VisitorPass visitorPass)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1, Unit.Inch);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Verdana));

                page.Content().Column(column =>
                {
                    // HEADER
                    column.Item().BorderBottom(1).PaddingBottom(5).Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text("VISITOR PASS").FontSize(22).SemiBold().FontColor(Colors.Blue.Medium);
                            c.Item().Text($"DOC ID: {Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}").FontSize(9).Italic();
                        });

                        row.ConstantItem(80).AlignRight().Text("OFFICIAL").FontSize(10).Bold().FontColor(Colors.Grey.Medium);
                    });

                    // MAIN DATA TABLE (Replaces Obsolete Grid)
                    column.Item().PaddingTop(15).PaddingBottom(15).Table(table =>
                    {
                        // Define two columns
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        // Local helper for table cells to avoid "void" return issues
                        void AddCell(string label, string value)
                        {
                            table.Cell().PaddingVertical(5).Column(c =>
                            {
                                c.Item().Text(label).FontSize(8).SemiBold().FontColor(Colors.Grey.Medium);
                                c.Item().Text(value ?? "N/A").FontSize(11);
                            });
                        }

                        AddCell("NAME", visitorPass.Name);
                        AddCell("CNIC", visitorPass.CNIC);
                        AddCell("VEHICLE LICENSE #", visitorPass.VehicleLicensePlate + "-" + visitorPass.VehicleLicenseNo.ToString());
                        AddCell("PASS TYPE", visitorPass.VisitorPassType.ToString());
                        AddCell("VALID FROM", visitorPass.ValidFrom.ToString("dd MMM yyyy"));
                        AddCell("VALID UNTIL", visitorPass.ValidTo.ToString("dd MMM yyyy"));
                    });

                    // QR CODE SECTION
                    if (!string.IsNullOrEmpty(visitorPass.QRCode))
                    {
                        column.Item().PaddingTop(10).Background(Colors.Grey.Lighten4).Padding(15).Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text($"Reference Number: {visitorPass.QRCode}").FontSize(10).Bold();
                                c.Item().PaddingTop(4).Text("Input reference number or Scan this code at entry/exit points for automated logging.").FontSize(8).LineHeight(1.2f).FontColor(Colors.Grey.Darken1);
                            });

                            // QR Image Placeholder
                            // To show actual image: .Image(byte[] yourQRCodeBytes)
                            row.ConstantItem(100).AlignRight().Column(c => {
                                // If the string is a GUID or raw text, we generate the image bytes here
                                byte[]? qrBytes = GetQrCodeImageBytes(visitorPass.QRCode);
                                if (qrBytes != null)
                                {
                                    c.Item().Image(qrBytes);
                                }
                            });
                        });
                    }

                    // FOOTER TERMS
                    column.Item().PaddingTop(30).Column(c => {
                        c.Item().Text("Instructions:").FontSize(9).Bold();
                        c.Item().PaddingTop(2).Text("1. Please keep this pass visible at all times.").FontSize(8);
                        c.Item().Text("2. Return the pass to the reception upon departure.").FontSize(8);
                    });
                });

                page.Footer().PaddingTop(10).Row(row =>
                {
                    row.RelativeItem().Text(x =>
                    {
                        x.Span("Generated on: ").FontSize(8);
                        x.Span(DateTime.Now.ToString("dd MMM yyyy HH:mm")).FontSize(8);
                    });

                    row.RelativeItem().AlignRight().Text(x =>
                    {
                        x.Span("Page ").FontSize(8);
                        x.CurrentPageNumber().FontSize(8);
                    });
                });
            });
        }).GeneratePdf();
    }
    
    private byte[]? GetQrCodeImageBytes(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return null;

        // 1. If it looks like a GUID (contains hyphens) or isn't Base64 image data,
        // we treat the string as the PAYLOAD for a new QR Code.
        if (IsPlainString(input))
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(input, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            return qrCode.GetGraphic(20);
        }

        // 2. Otherwise, try to treat it as an existing Base64 image string
        try
        {
            string cleaned = input.Contains(",") ? input.Split(',')[1] : input;
            return Convert.FromBase64String(cleaned.Trim().Replace(" ", "+"));
        }
        catch
        {
            return null;
        }
    }

    private bool IsPlainString(string input)
    {
        // If it contains hyphens (like your GUID) or is very short, it's likely a ID/Payload
        // Real Base64 images are usually thousands of characters long.
        return input.Contains("-") || input.Length < 100;
    }
}
