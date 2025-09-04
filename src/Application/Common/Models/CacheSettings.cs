using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Common.Models;
public class CacheSettings
{
    public int RedisExpiryMinutes { get; set; } = 60;
    public int MemoryExpiryMinutes { get; set; } = 5;
}

