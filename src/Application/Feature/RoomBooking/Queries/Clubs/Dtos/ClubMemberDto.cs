using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.Clubs.Dtos;
public class ClubMemberDto
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = default!;
    public string CNIC { get; set; } = default!;
    public string MEMPK { get; set; } = default!;
    public string MobileNo { get; set; } = default!;
    public string? RegisteredMobileNo { get; set; }
    public string? RegisteredEmail { get; set; }
    public string? RegistrationNo { get; set; }
}
