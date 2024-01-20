using System.Threading.Tasks;
using System;

namespace FitAndFun.Common
{
    public interface IRedisDatabase
    {
        Task<T> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value);
        Task RemoveAsync(string key);
        Task<bool> PublishAsync(string channel, string message);
        Task SubscribeAsync(string channel, Action<string> handler);
        Task<IEnumerable<string>> GetAllKeysAsync(string pattern);
    }
}
