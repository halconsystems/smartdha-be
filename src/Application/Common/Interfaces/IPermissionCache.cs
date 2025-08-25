using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Common.Interfaces;
public interface IPermissionCache
{
    Task<List<string>> GetPermissionsAsync(string userId, Func<Task<List<string>>> dbFallback);
    Task SetPermissionsAsync(string userId, List<string> permissions);
    Task InvalidateAsync(string userId);
}

