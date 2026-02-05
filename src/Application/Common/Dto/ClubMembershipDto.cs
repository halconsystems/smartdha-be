using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Common.Dto;
public class ClubMembershipDto
{
    public string MembershipNo { get; set; } = default!;
    public string Rank { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string CNIC { get; set; } = default!;
    public string MobilNumber { get; set; } = default!;
    public string OneInKid { get; set; } = default!;
    public string BillStatus { get; set; } = default!;
    public string Clube { get; set; } = default!;
}

