using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Feature.CBMS.ClubServices.Queries;
using DHAFacilitationAPIs.Domain.Entities.CBMS;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.Clubs.Queries;

public class ClubDTO
{
    public Guid Id {  get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? imageUrl { get; set; }
    public List<string>? bannerIamges { get; set; }
    public string? Location { get; set; }
    public string? ContactNumber { get; set; }
    public string? AccountNo { get; set; }
    [MaxLength(4)] public string? AccountNoAccronym { get; set; }
    public ClubType ClubType { get; set; } = ClubType.GuestRoom;

    public List<string>? highlights { get; set; }
    public List<HighlightDTO>? Categories { get; set; }

    public List<ClubServiceProcessDTO>? ClubServicesDTOs { get; set; }
}
public class HighlightDTO
{
    public Guid Id { get; set; }   // or int, whatever your Id type is
    public string Name { get; set; } = string.Empty;
}
