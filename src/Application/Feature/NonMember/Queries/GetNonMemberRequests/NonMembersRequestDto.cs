using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.NonMember.Queries.GetNonMemberRequests;
public class NonMemberRequestsDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string CNIC { get; set; } = default!;

    public string Remarks { get; set; } = default!;
    public VerificationStatus Status { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? ApprovedBy { get; set; }

    // 🔄 Updated for multiple purposes
    public List<PurposeDto> Purposes { get; set; } = new();

    // Optional: display purpose titles as comma-separated string
    [NotMapped]
    public string PurposeTitlesDisplay => string.Join(", ", Purposes.Select(p => p.Title));

    public List<NonMemberVerificationDocsDto> VerificationDocs { get; set; } = new();
}

public class PurposeDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
}


public class NonMemberVerificationDocsDto
{

    public Guid VerificationId { get; set; }
    public NonMemberVerification Verification { get; set; } = default!;

    public string CNICFrontImagePath { get; set; } = default!;

    public string CNICBackImagePath { get; set; } = default!;

    public string? SupportingDocumentPath { get; set; } // optional 

    public string? DocumentType { get; set; } // optional, e.g., "Utility Bill", "Rental Proof"

    public string? OriginalFileName { get; set; } // optional: store uploaded file name
}

