using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Common.Models;
public class FirebaseSettings
{
    public string ProjectId { get; set; } = default!;
    public string ServiceAccountJsonPath { get; set; } = default!;

    public string DHAProjectId { get; set; } = default!;
    public string DHAServiceAccountJsonPath { get; set; } = default!;
}

