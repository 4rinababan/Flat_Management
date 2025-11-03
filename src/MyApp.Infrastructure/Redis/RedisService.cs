using Microsoft.EntityFrameworkCore.Storage;
using MyApp.Infrastructure.Redis;
using StackExchange.Redis;
using System.Text.Json;

public class RedisService : IRedisService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly StackExchange.Redis.IDatabase _db;

    public RedisService(IConnectionMultiplexer redis)
    {
        _redis = redis ?? throw new ArgumentNullException(nameof(redis));
        _db = _redis.GetDatabase();
    }

    /// <summary>
    /// Get value by key and deserialize to type T
    /// </summary>
    public async Task<T> GetAsync<T>(string key)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentNullException(nameof(key));

        var value = await _db.StringGetAsync(key);

        if (!value.HasValue)
            return default;

        return JsonSerializer.Deserialize<T>(value);
    }

    /// <summary>
    /// Get string value by key
    /// </summary>
    public async Task<string> GetAsync(string key)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentNullException(nameof(key));

        var value = await _db.StringGetAsync(key);
        return value.HasValue ? value.ToString() : null;
    }

    /// <summary>
    /// Set value with optional TTL (Time To Live)
    /// </summary>
    public async Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentNullException(nameof(key));

        if (value == null)
            throw new ArgumentNullException(nameof(value));

        var serializedValue = JsonSerializer.Serialize(value);
        return await _db.StringSetAsync(key, serializedValue, expiry);
    }

    /// <summary>
    /// Set string value with optional TTL
    /// </summary>
    public async Task<bool> SetAsync(string key, string value, TimeSpan? expiry = null)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentNullException(nameof(key));

        if (value == null)
            throw new ArgumentNullException(nameof(value));

        return await _db.StringSetAsync(key, value, expiry);
    }

    /// <summary>
    /// Remove key from Redis
    /// </summary>
    public async Task<bool> RemoveAsync(string key)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentNullException(nameof(key));

        return await _db.KeyDeleteAsync(key);
    }

    /// <summary>
    /// Check if key exists
    /// </summary>
    public async Task<bool> ExistsAsync(string key)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentNullException(nameof(key));

        return await _db.KeyExistsAsync(key);
    }

    /// <summary>
    /// Get remaining TTL of a key
    /// </summary>
    public async Task<TimeSpan?> GetTimeToLiveAsync(string key)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentNullException(nameof(key));

        return await _db.KeyTimeToLiveAsync(key);
    }

    /// <summary>
    /// Set or update expiry time for existing key
    /// </summary>
    public async Task<bool> SetExpiryAsync(string key, TimeSpan expiry)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentNullException(nameof(key));

        return await _db.KeyExpireAsync(key, expiry);
    }
}
