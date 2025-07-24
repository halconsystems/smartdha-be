using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Settings;
using DHAFacilitationAPIs.Application.Interface.Repository;
using Microsoft.Extensions.Options;

namespace DHAFacilitationAPIs.Infrastructure.Repository;
public class UnitOfWork : IUnitOfWork
{
    public UnitOfWork(IOptions<DatabaseSettings> databaseSettings)
    {
        TodoListRepo = new TodoListRepository(databaseSettings);
    }
    public ITodoListRepository TodoListRepo { get; }
}
