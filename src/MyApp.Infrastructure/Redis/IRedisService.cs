using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Infrastructure.Redis
{
    public interface IRedisService
    {
        Task<T> GetAsync<T>(string key);
        Task<string> GetAsync(string key);
        Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiry = null);
        Task<bool> SetAsync(string key, string value, TimeSpan? expiry = null);
        Task<bool> RemoveAsync(string key);
        Task<bool> ExistsAsync(string key);
        Task<TimeSpan?> GetTimeToLiveAsync(string key);
        Task<bool> SetExpiryAsync(string key, TimeSpan expiry);
    }
}
