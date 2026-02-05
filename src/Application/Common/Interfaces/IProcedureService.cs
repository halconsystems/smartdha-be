using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DHAFacilitationAPIs.Application.Common.Dto;

namespace DHAFacilitationAPIs.Application.Common.Interfaces;
public interface IProcedureService
{
    Task<DynamicParameters> ExecuteAsync(string name, DynamicParameters parameters, CancellationToken cancellationToken, string connectionName = "DefaultConnection");

    Task<(DynamicParameters, T?)> ExecuteWithSingleRowAsync<T>(string name, DynamicParameters parameters, CancellationToken cancellationToken, string connectionName = "DefaultConnection");

    Task<(DynamicParameters, List<T>)> ExecuteWithListAsync<T>(string name, DynamicParameters parameters, CancellationToken cancellationToken, string connectionName = "DefaultConnection");

    Task<List<T>> ExecuteWithoutParamsAsync<T>(string name, CancellationToken cancellationToken, string connectionName = "DefaultConnection");

    Task<(MemberLookupResult Output, List<T> List)>
    ExecuteWithOutputAndListAsync<T>(
        string name,
        DynamicParameters parameters,
        CancellationToken cancellationToken,
        string connectionName = "DefaultConnection");

}
