using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace DHAFacilitationAPIs.Application.Common.Interfaces;
public interface IProcedureService
{
    /// <summary>
    /// Executes a stored procedure and returns the same DynamicParameters,
    /// so you can read any OUTPUT or RETURN values.
    /// </summary>
    Task<DynamicParameters> ExecuteAsync(
        string name,
        DynamicParameters parameters,
        CancellationToken cancellationToken
    );

    Task<(DynamicParameters, T?)> ExecuteWithSingleRowAsync<T>(
       string name,
       DynamicParameters parameters,
       CancellationToken cancellationToken
   );

    Task<(DynamicParameters, List<T>)> ExecuteWithListAsync<T>(
    string name,
    DynamicParameters parameters,
    CancellationToken cancellationToken
);

}
