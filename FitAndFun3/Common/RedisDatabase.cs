using StackExchange.Redis;
using System.Text.Json;
using System.Threading.Tasks;

namespace FitAndFun.Common
{
    public class RedisDatabase : IRedisDatabase
    {
        private readonly IDatabase _database;
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        public RedisDatabase(IConnectionMultiplexer connectionMultiplexer)
        {
            if (connectionMultiplexer == null)
            {
                throw new ArgumentNullException(nameof(connectionMultiplexer));
            }
            _connectionMultiplexer = connectionMultiplexer;
            _database = connectionMultiplexer.GetDatabase();
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var value = await _database.StringGetAsync(key);
            return value.HasValue ? JsonSerializer.Deserialize<T>(value) : default;
        }

        public async Task SetAsync<T>(string key, T value)
        {
            await _database.StringSetAsync(key, JsonSerializer.Serialize(value));
        }

        public async Task RemoveAsync(string key)
        {
            await _database.KeyDeleteAsync(key);
        }

        public async Task<bool> PublishAsync(string channel, string message)
        {
            var result = await _database.PublishAsync(channel, message);

            return result > 0;
        }

        public async Task SubscribeAsync(string channel, Action<string> handler)
        {
            var subscriber = _connectionMultiplexer.GetSubscriber();
            await subscriber.SubscribeAsync(channel, (redisChannel, value) =>
            {
                handler?.Invoke(value);
            });
        }

        public async Task<IEnumerable<string>> GetAllKeysAsync(string pattern)
{
    var server = _connectionMultiplexer.GetServer(_connectionMultiplexer.GetEndPoints().First());
    var keys = server.Keys(pattern: pattern);

    return keys.Select(k => (string)k);
}

    }
}
