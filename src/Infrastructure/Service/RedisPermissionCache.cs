using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Application.Common.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DHAFacilitationAPIs.Infrastructure.Service;

public class RedisPermissionCache : IPermissionCache
{
    private readonly IDistributedCache _redis;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<RedisPermissionCache> _logger;

    private readonly TimeSpan _redisExpiry = TimeSpan.FromHours(1);
    private readonly TimeSpan _memoryExpiry = TimeSpan.FromMinutes(5);

    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

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
        var redisKey = $"RBAC:permissions:{userId}";

        // 1️⃣ Try Redis first
        try
        {
            var data = await _redis.GetStringAsync(redisKey);
            if (!string.IsNullOrEmpty(data))
            {
                var list = JsonSerializer.Deserialize<List<string>>(data, _jsonOptions);
                if (list != null && list.Count > 0)
                {
                    return list;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, $"Redis unavailable for key {redisKey}. Using fallback.");
        }

        // 2️⃣ Try in-memory cache
        if (_memoryCache.TryGetValue(redisKey, out List<string>? cachedPerms) && cachedPerms != null)
        {
            return cachedPerms;
        }

        // 3️⃣ Fallback → DB query
        var permissionsFromDb = await dbFallback() ?? new List<string>();

        // 4️⃣ Save to Redis
        try
        {
            await SetPermissionsAsync(userId, permissionsFromDb);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, $"Redis unavailable when setting key {redisKey}.");
        }

        // 5️⃣ Save to MemoryCache (short TTL)
        _memoryCache.Set(redisKey, permissionsFromDb, _memoryExpiry);

        return permissionsFromDb;
    }

    public async Task SetPermissionsAsync(string userId, List<string> permissions)
    {
        var redisKey = $"RBAC:permissions:{userId}";
        var json = JsonSerializer.Serialize(permissions, _jsonOptions);

        try
        {
            await _redis.SetStringAsync(redisKey, json, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _redisExpiry
            });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, $"Failed to set Redis key {redisKey}. Falling back to in-memory only.");
        }

        // Always update memory cache too
        _memoryCache.Set(redisKey, permissions ?? new List<string>(), _memoryExpiry);
    }

    public async Task InvalidateAsync(string userId)
    {
        var redisKey = $"RBAC:permissions:{userId}";
        try
        {
            await _redis.RemoveAsync(redisKey);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, $"Failed to remove Redis key {redisKey}. Will clear in-memory anyway.");
        }

        _memoryCache.Remove(redisKey);
    }
}




