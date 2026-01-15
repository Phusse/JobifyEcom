namespace Jobify.Ecom.Application.Services;

public interface ICacheService
{
    Task<bool> SetAsync<T>(string key, T value, TimeSpan? timeToLive = null);
    Task<T?> GetAsync<T>(string key);
    Task<bool> RemoveAsync(string key);
    Task<bool> ExistsAsync(string key);
}
