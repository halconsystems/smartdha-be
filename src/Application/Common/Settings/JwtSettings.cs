using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Common.Settings;
public class JwtSettings
{
    public const string SectionName = "JwtSettings";
    public static JwtSettings Bind = new JwtSettings();
    public string Key { get; set; } = default!;
    public string Issuer { get; set; } = default!;
    public string Audience { get; set; } = default!;
    public double DurationInMinutes { get; set; }
    public string Secret { get; set; } = default!;
}
