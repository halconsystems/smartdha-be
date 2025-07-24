using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Settings;
using DHAFacilitationAPIs.Application.Interface.Repository;
using DHAFacilitationAPIs.Infrastructure.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace DHAFacilitationAPIs.Infrastructure.Repository;
public class TodoListRepository : ITodoListRepository
{
    private readonly IDbConnectionSettings _databaseSettings;

    public TodoListRepository(IOptions<DatabaseSettings> databaseSettings)
    {
        _databaseSettings = new DbConnectionSettings(new SqlConnection(databaseSettings.Value.DefaultConnection)); ;
    }
    public async Task<IEnumerable<int>> GetTodoList()
    {
        var parameters = new DynamicParameters();
        parameters.Add("@Id", "2");
        return await _databaseSettings
            .SqlConnection
            .QueryAsync<int>(
            StoredProcedures.GetList,
            parameters, commandType: CommandType.StoredProcedure);
    }
}
