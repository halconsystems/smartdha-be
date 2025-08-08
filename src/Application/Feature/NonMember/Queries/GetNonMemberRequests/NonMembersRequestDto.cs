using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

    public string CNIC { get; set; } = default!;

    public string Remarks { get; set; } = default!;
    public VerificationStatus Status { get; set; } 
    public DateTime? ApprovedAt { get; set; } = default!;
    public string? ApprovedBy { get; set; } = default!;// Admin UserId
    public Guid PurposeId { get; set; } = default!;

    public string PurposeTitles { get; set; } = default!;

    public List<NonMemberVerificationDocsDto> VerificationDocs { get; set; }= default!;
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

