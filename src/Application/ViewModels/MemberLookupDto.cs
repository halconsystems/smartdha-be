using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.ViewModels;
public class MemberLookupDto
{
    public string MEMID { get; set; } = default!;
    public string MemNo { get; set; } = default!;
    public string STAFFNO { get; set; } = default!;
    public string Cat { get; set; } = default!;
    public string Name { get; set; } = default!;
    public DateTime? ApplicationDate { get; set; }
    public string NIC { get; set; } = default!;
    public string CellNo { get; set; } = default!;
    public string ALLREPLOT { get; set; } = default!;
    public string MEMPK { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string DOB { get; set; } = default!;
}

