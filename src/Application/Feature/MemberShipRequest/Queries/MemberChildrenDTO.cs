using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace DHAFacilitationAPIs.Application.Feature.MemberShipRequest.Queries;

public class MemberChildrenDTO
{

    [Required]
    public string FullName { get; set; } = default!;
    public Relation Relation { get; set; }
    public string? MobileNo { get; set; }
    public bool IsAdult { get; set; }
    [Required]
    public IFormFile PicturePath { get; set; } = default!;
    [Required]
    public IFormFile CNICFrontImagePath { get; set; } = default!;

    [Required]
    public IFormFile CNICBackImagePath { get; set; } = default!;

    public IFormFile? NadraBForm { get; set; }
    public string? CnicNo { get; set; }
    public DateTime CnicExpiryDate { get; set; }
}
