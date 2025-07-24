using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.DependencyResolver;

namespace DHAFacilitationAPIs.Application.Interface.Repository;
public interface IUnitOfWork : IServicesType.IScopedService
{
    ITodoListRepository TodoListRepo { get; }
}
