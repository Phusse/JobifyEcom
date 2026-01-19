using System.Text.Json;
using Jobify.Ecom.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Jobify.Ecom.Infrastructure.Services;

internal class RedisCacheService : ICacheService
{
    private readonly IDatabase? _db;
    private readonly ILogger<RedisCacheService> _logger;

    public RedisCacheService(IConfiguration configuration, ILogger<RedisCacheService> logger)
    {
        _logger = logger;
        string? connectionString = configuration.GetConnectionString("Redis");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            _logger.LogWarning("Redis connection string is missing. Redis cache will be disabled.");
            return;
        }

        try
        {
            ConnectionMultiplexer redisConnection = ConnectionMultiplexer.Connect(connectionString);
            _db = redisConnection.GetDatabase();
            _logger.LogInformation("Connected to Redis successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to connect to Redis. Redis cache will be disabled.");
        }
    }

    public async Task<bool> SetAsync<T>(string key, T value, TimeSpan? TimeToLive = null)
    {
        if (string.IsNullOrWhiteSpace(key) || _db is null)
            return false;

        try
        {
            string json = JsonSerializer.Serialize(value);
            Expiration expiration = TimeToLive.HasValue ? new Expiration(TimeToLive.Value) : Expiration.Default;

            return await _db.StringSetAsync(key, json, expiration);
        }
        catch (Exception ex)
        {
            if (_logger.IsEnabled(LogLevel.Error))
                _logger.LogError(ex, "Redis SetAsync failed for key '{Key}'", key);

            return false;
        }
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        if (string.IsNullOrWhiteSpace(key) || _db is null)
            return default;

        try
        {
            RedisValue value = await _db.StringGetAsync(key);

            if (value.IsNullOrEmpty) return default;

            return JsonSerializer.Deserialize<T>(value.ToString());
        }
        catch (Exception ex)
        {
            if (_logger.IsEnabled(LogLevel.Error))
                _logger.LogError(ex, "Redis GetAsync failed for key '{Key}'", key);

            return default;
        }
    }

    public async Task<bool> RemoveAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key) || _db is null)
            return false;

        try
        {
            return await _db.KeyDeleteAsync(key);
        }
        catch (Exception ex)
        {
            if (_logger.IsEnabled(LogLevel.Error))
                _logger.LogError(ex, "Redis RemoveAsync failed for key '{Key}'", key);

            return false;
        }
    }

    public async Task<bool> ExistsAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key) || _db is null)
            return false;

        try
        {
            return await _db.KeyExistsAsync(key);
        }
        catch (Exception ex)
        {
            if (_logger.IsEnabled(LogLevel.Error))
                _logger.LogError(ex, "Redis ExistsAsync failed for key '{Key}'", key);

            return false;
        }
    }
}
