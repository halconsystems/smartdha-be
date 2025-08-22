using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DHAFacilitationAPIs.Infrastructure.Service;
public class RedisPermissionCache : IPermissionCache
{
    private readonly IDistributedCache _redis;
    private readonly IMemoryCache _memoryCache; // backup in-memory cache
    private readonly ILogger<RedisPermissionCache> _logger;
    private readonly TimeSpan _expiry = TimeSpan.FromHours(1);

    public RedisPermissionCache(
        IDistributedCache redis,
        IMemoryCache memoryCache,
        ILogger<RedisPermissionCache> logger)
    {
        _redis = redis;
        _memoryCache = memoryCache;
        _logger = logger;
    }

    public async Task<List<string>> GetPermissionsAsync(string userId, Func<Task<List<string>>> dbFallback)
    {
        var redisKey = $"permissions:{userId}";

        try
        {
            // 1. Try Redis
            var data = await _redis.GetStringAsync(redisKey);
            if (!string.IsNullOrEmpty(data))
            {
                return JsonConvert.DeserializeObject<List<string>>(data)!;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis unavailable. Falling back.");
        }

        // 2. Try In-Memory cache
        if (_memoryCache.TryGetValue(redisKey, out List<string>? cachedPerms) && cachedPerms != null)
        {
            return cachedPerms;
        }

        // 3. Fallback → DB
        var permissionsFromDb = await dbFallback();

        // 4. Store in Redis (if available)
        try
        {
            await SetPermissionsAsync(userId, permissionsFromDb);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis unavailable during Set.");
        }

        // 5. Store in MemoryCache (short TTL)
        _memoryCache.Set(redisKey, permissionsFromDb, TimeSpan.FromMinutes(5));

        return permissionsFromDb;
    }

    public async Task SetPermissionsAsync(string userId, List<string> permissions)
    {
        var redisKey = $"permissions:{userId}";
        var json = JsonConvert.SerializeObject(permissions);

        await _redis.SetStringAsync(redisKey, json, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _expiry
        });

        _memoryCache.Set(redisKey, permissions, TimeSpan.FromMinutes(5));
    }

    public async Task InvalidateAsync(string userId)
    {
        var redisKey = $"permissions:{userId}";
        await _redis.RemoveAsync(redisKey);
        _memoryCache.Remove(redisKey);
    }
}

