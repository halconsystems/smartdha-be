using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;

namespace DHAFacilitationAPIs.Infrastructure.Service;
public class BasketIdGenerator : IBasketIdGenerator
{
    public string Generate()
        => $"DHA{DateTime.UtcNow:yyMMddHHmmss}{Random.Shared.Next(1000, 9999)}";
}
