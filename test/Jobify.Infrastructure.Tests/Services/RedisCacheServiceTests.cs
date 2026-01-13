using Jobify.Infrastructure.Services;
using Jobify.Infrastructure.Tests.Utilities.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;

namespace Jobify.Infrastructure.Tests.Services;

[Collection("Redis collection")]
public class RedisCacheServiceTests
{
    private readonly RedisCacheService _cache;

    private class NonSerializable
    {
        public NonSerializable Self => this;
    }

    public RedisCacheServiceTests(RedisTestFixture fixture)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Redis"] = fixture.ConnectionString
            })
            .Build();

        _cache = new RedisCacheService(
            configuration,
            NullLogger<RedisCacheService>.Instance
        );
    }

    [Fact]
    public async Task SetAndGet_ShouldReturnStoredValue()
    {
        string key = "test:key:1";
        TestObject value = new() { Id = 1, Name = "Jane" };

        bool setResult = await _cache.SetAsync(key, value, TimeSpan.FromMinutes(1));
        TestObject? result = await _cache.GetAsync<TestObject>(key);

        Assert.True(setResult);
        Assert.NotNull(result);
        Assert.Equal(value.Id, result.Id);
        Assert.Equal(value.Name, result.Name);
    }

    [Fact]
    public async Task Exists_ShouldReturnTrue_WhenKeyExists()
    {
        string key = "test:key:exists";

        await _cache.SetAsync(key, "hello");

        bool exists = await _cache.ExistsAsync(key);

        Assert.True(exists);
    }

    [Fact]
    public async Task Remove_ShouldDeleteKey()
    {
        string key = "test:key:remove";

        await _cache.SetAsync(key, "value");
        bool removed = await _cache.RemoveAsync(key);
        bool exists = await _cache.ExistsAsync(key);

        Assert.True(removed);
        Assert.False(exists);
    }

    [Fact]
    public async Task Get_ShouldReturnNull_WhenKeyDoesNotExist()
    {
        string? result = await _cache.GetAsync<string>("non-existent-key");

        Assert.Null(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task SetAsync_ShouldReturnFalse_WhenKeyIsInvalid(string key)
    {
        bool result = await _cache.SetAsync(key, "value");

        Assert.False(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetAsync_ShouldReturnDefault_WhenKeyIsInvalid(string key)
    {
        string? result = await _cache.GetAsync<string>(key);

        Assert.Null(result);
    }

    [Fact]
    public async Task SetAsync_ShouldExpireKey_WhenTTLIsProvided()
    {
        string key = $"ttl:{Guid.NewGuid()}";

        await _cache.SetAsync(key, "temp", TimeSpan.FromMilliseconds(300));

        await Task.Delay(500);

        bool exists = await _cache.ExistsAsync(key);

        Assert.False(exists);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_WhenKeyDoesNotExist()
    {
        string key = $"missing:{Guid.NewGuid()}";

        bool exists = await _cache.ExistsAsync(key);

        Assert.False(exists);
    }

    [Fact]
    public async Task RemoveAsync_ShouldReturnFalse_WhenKeyDoesNotExist()
    {
        string key = $"missing:{Guid.NewGuid()}";

        bool removed = await _cache.RemoveAsync(key);

        Assert.False(removed);
    }

    [Fact]
    public async Task SetAsync_ShouldReturnFalse_WhenSerializationFails()
    {
        string key = $"bad:{Guid.NewGuid()}";
        NonSerializable value = new();

        bool result = await _cache.SetAsync(key, value);

        Assert.False(result);
    }

    [Fact]
    public async Task Service_ShouldGracefullyDisable_WhenRedisConnectionStringIsMissing()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder().Build();

        RedisCacheService service = new(
            configuration,
            NullLogger<RedisCacheService>.Instance
        );

        bool result = await service.SetAsync("key", "value");

        Assert.False(result);
    }

    [Fact]
    public async Task Service_ShouldNotThrow_WhenRedisIsUnreachable()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Redis"] = "127.0.0.1:1"
            })
            .Build();

        RedisCacheService service = new(
            configuration,
            NullLogger<RedisCacheService>.Instance
        );

        string? result = await service.GetAsync<string>("key");

        Assert.Null(result);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_WhenKeyIsNullOrEmpty()
    {
        RedisCacheService service = new(
            new ConfigurationBuilder().Build(),
            NullLogger<RedisCacheService>.Instance
        );

        Assert.False(await service.ExistsAsync(""));
        Assert.False(await service.ExistsAsync("   "));
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_WhenDbIsNull()
    {
        RedisCacheService service = new(
            new ConfigurationBuilder().Build(),
            NullLogger<RedisCacheService>.Instance
        );

        Assert.False(await service.ExistsAsync("any-key"));
    }

    [Fact]
    public async Task RemoveAsync_ShouldReturnFalse_WhenKeyIsNullOrEmpty()
    {
        RedisCacheService service = new(
            new ConfigurationBuilder().Build(),
            NullLogger<RedisCacheService>.Instance
        );

        Assert.False(await service.RemoveAsync(""));
        Assert.False(await service.RemoveAsync("   "));
    }

    [Fact]
    public async Task RemoveAsync_ShouldReturnFalse_WhenDbIsNull()
    {
        RedisCacheService service = new(
            new ConfigurationBuilder().Build(),
            NullLogger<RedisCacheService>.Instance
        );

        Assert.False(await service.RemoveAsync("any-key"));
    }
}
