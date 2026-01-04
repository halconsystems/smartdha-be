using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Common.Settings;
public class FirebaseOptions
{
    public const string SectionName = "Firebase";

    public string ServerKey { get; set; } = default!;
}
