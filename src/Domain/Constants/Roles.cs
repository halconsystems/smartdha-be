namespace DHAFacilitationAPIs.Domain.Constants;

public abstract class Roles
{
    // Web Roles (used in admin panel, system access)
    public const string SuperAdministrator = nameof(SuperAdministrator);
    public const string Administrator = nameof(Administrator);
    public const string Admin = nameof(Admin);

    // Mobile App Roles (for mobile API access)
    public const string Member = nameof(Member);
    public const string NonMember = nameof(NonMember);

    public static IEnumerable<string> GetWebRoles() => new[]
    {
        SuperAdministrator,
        Administrator,
        Admin
    };

    public static IEnumerable<string> GetMobileRoles() => new[]
    {
        Member,
        NonMember
    };

    public static IEnumerable<string> GetAllRoles() =>
        GetWebRoles().Concat(GetMobileRoles());
}

