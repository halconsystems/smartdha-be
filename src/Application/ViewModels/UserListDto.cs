using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.ViewModels;
public class UserListDto
{
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Email { get; set; } = default!;
    public string MobileNo { get; set; } = default!;
    public string CNIC { get; set; } = default!;
    public string? AppType { get; set; }
    public string? UserType { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    public string Role { get; set; } = default!;
}
