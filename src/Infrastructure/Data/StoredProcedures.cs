using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DHAFacilitationAPIs.Application.Common.Dto;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DHAFacilitationAPIs.Infrastructure.Data;

public class StoredProcedures : IProcedureService
{
    private readonly DapperConnectionFactory _connectionFactory;

    public StoredProcedures(DapperConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<DynamicParameters> ExecuteAsync(
        string name,
        DynamicParameters parameters,
        CancellationToken cancellationToken,
        string connectionName = "DefaultConnection")
    {
        using var conn = _connectionFactory.GetConnection(connectionName);

        var cmd = new CommandDefinition(name, parameters, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken);

        await conn.ExecuteAsync(cmd);
        return parameters;
    }

    public async Task<(DynamicParameters, T?)> ExecuteWithSingleRowAsync<T>(
        string name,
        DynamicParameters parameters,
        CancellationToken cancellationToken,
        string connectionName = "DefaultConnection")
    {
        using var conn = _connectionFactory.GetConnection(connectionName);

        var cmd = new CommandDefinition(name, parameters, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken);

        using var multi = await conn.QueryMultipleAsync(cmd);
        var row = await multi.ReadFirstOrDefaultAsync<T>();

        return (parameters, row);
    }

    public async Task<(DynamicParameters, List<T>)> ExecuteWithListAsync<T>(
        string name,
        DynamicParameters parameters,
        CancellationToken cancellationToken,
        string connectionName = "DefaultConnection")
    {
        using var conn = _connectionFactory.GetConnection(connectionName);

        var cmd = new CommandDefinition(name, parameters, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken);

        var rows = (await conn.QueryAsync<T>(cmd)).ToList();

        return (parameters, rows);
    }

    public async Task<List<T>> ExecuteWithoutParamsAsync<T>(
        string name,
        CancellationToken cancellationToken,
        string connectionName = "DefaultConnection")
    {
        using var conn = _connectionFactory.GetConnection(connectionName);

        var cmd = new CommandDefinition(name, null, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken);

        var rows = (await conn.QueryAsync<T>(cmd)).ToList();

        return rows;
    }

    public async Task<(MemberLookupResult Output, List<T> List)>
    ExecuteWithOutputAndListAsync<T>(
        string name,
        DynamicParameters parameters,
        CancellationToken cancellationToken,
        string connectionName = "DefaultConnection")
    {
        using var conn = _connectionFactory.GetConnection(connectionName);

        var cmd = new CommandDefinition(
            name,
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        using var multi = await conn.QueryMultipleAsync(cmd);

        // 🔹 First (and only) result-set = club list
        var list = (await multi.ReadAsync<T>()).ToList();

        // 🔹 Map OUTPUT parameters
        var output = new MemberLookupResult
        {
            MemId = parameters.Get<string?>("@MEMID"),
            MemNo = parameters.Get<string?>("@MemNo"),
            StaffNo = parameters.Get<string?>("@STAFFNO"),
            Category = parameters.Get<string?>("@Cat"),
            Name = parameters.Get<string?>("@Name"),
            ApplicationDate = parameters.Get<string?>("@ApplicationDate"),
            NIC = parameters.Get<string?>("@NIC"),
            CellNo = parameters.Get<string?>("@CellNo"),
            AllReplot = parameters.Get<string?>("@ALLREPLOT"),
            MemPk = parameters.Get<string?>("@MEMPK"),
            Email = parameters.Get<string?>("@Email"),
            DOB = parameters.Get<string?>("@DOB"),
            Message = parameters.Get<string?>("@msg")
        };

        return (output, list);
    }

}

