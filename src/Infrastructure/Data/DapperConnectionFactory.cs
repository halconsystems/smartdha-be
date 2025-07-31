using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace DHAFacilitationAPIs.Infrastructure.Data;
public class DapperConnectionFactory
{
    private readonly IConfiguration _configuration;

    public DapperConnectionFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IDbConnection GetConnection(string connectionName = "DefaultConnection")
    {
        var connStr = _configuration.GetConnectionString(connectionName);
        var conn = new SqlConnection(connStr);
        conn.Open();
        return conn;
    }
}

