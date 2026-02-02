using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Common.Interfaces;
public interface IMerchantResolver
{
    Task<string> ResolveAsync(
        string sourceSystem,
        string entityType,
        Guid? entityId,
        CancellationToken ct);
}

