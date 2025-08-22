using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.User.Queries.GetUserDashboard;
public class UserDashboardDto
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int InactiveUsers { get; set; }
    public int VerifiedUsers { get; set; }
    public int NonVerifiedUsers { get; set; }
    public int DeletedUsers { get; set; }

    public Dictionary<string, int> UserTypeCounts { get; set; } = new();
    public Dictionary<string, int> AppTypeCounts { get; set; } = new();

    // Day-wise trend for current month
    public List<DailyUserTrendDto> DailyTrends { get; set; } = new();

    // Role-based bar chart data
    public List<RoleUserCountDto> RoleWiseUsers { get; set; } = new();
}

public class DailyUserTrendDto
{
    public DateTime Date { get; set; }
    public int RegisteredCount { get; set; }
    public int VerifiedCount { get; set; }
    public int MemberCount { get; set; }
    public int NonMemberCount { get; set; }
}

public class RoleUserCountDto
{
    public string RoleName { get; set; } = string.Empty;
    public int UserCount { get; set; }
}

