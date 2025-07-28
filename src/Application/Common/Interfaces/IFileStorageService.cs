using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DHAFacilitationAPIs.Application.Common.Interfaces;
public interface IFileStorageService
{
    Task<string> SaveFileAsync(IFormFile file, string folderName, CancellationToken cancellationToken);
}

