using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.DependencyResolver;
using DHAFacilitationAPIs.Application.Common.Settings;
using Microsoft.Extensions.Options;

namespace DHAFacilitationAPIs.Infrastructure.Data;
public interface IDbConnectionSettings
{
    IDbConnection SqlConnection { get; }
}

internal sealed class DbConnectionSettings : IDbConnectionSettings
{
    public IDbConnection SqlConnection { get; }
    public DbConnectionSettings(IDbConnection connection)
    {
        SqlConnection = connection;
    }
}

public interface IConnectionString : IServicesType.IScopedService
{
    DatabaseSettings DatabaseConnection { get; }
}
internal sealed class ConnectionString : IConnectionString
{
    public DatabaseSettings DatabaseConnection { get; }
    public ConnectionString(IOptions<DatabaseSettings> dbSettings)
    {
        DatabaseConnection = dbSettings.Value;
    }
}
