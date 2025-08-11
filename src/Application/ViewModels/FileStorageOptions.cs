using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.ViewModels;
public class FileStorageOptions
{
    public string RootPath { get; set; } = default!;     // absolute server path
    public string RequestPath { get; set; } = "/uploads"; // public prefix
    public string? PublicBaseUrl { get; set; }           // optional absolute URL base
}
