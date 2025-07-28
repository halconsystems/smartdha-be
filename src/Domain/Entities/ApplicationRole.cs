using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace DHAFacilitationAPIs.Domain.Entities;
public class ApplicationRole : IdentityRole
{
    public string? Description { get; set; } = default!;
    public AppType AppType { get; set; } = default!; // Optional
}
