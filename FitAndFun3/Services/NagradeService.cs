using FitAndFun.Common;
using FitAndFun.Models;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace FitAndFun.Services
{
    public class NagradeService : INagradeService
    {
        private readonly IDistributedCache _cache;
        private readonly IRedisDatabase _redisDatabase; 

        public NagradeService(IDistributedCache cache, IRedisDatabase redisDatabase) 
        {
            _cache = cache;
            _redisDatabase = redisDatabase; 
        }

        public async Task<Nagrade> GetNagradeById(int nagradeId)
        {
            var cacheKey = $"Nagrade_{nagradeId}";

            try
            {
                var cachedNagrade = await GetFromCacheAsync(cacheKey);

                if (cachedNagrade == null)
                {
                    var nagradeFromDatabase = await GetNagradeFromDatabaseAsync(nagradeId);

                    if (nagradeFromDatabase != null)
                    {
                        await AddToCacheAsync(cacheKey, nagradeFromDatabase);
                    }

                    return nagradeFromDatabase;
                }

                return JsonSerializer.Deserialize<Nagrade>(cachedNagrade);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom dobavljanja nagrada po ID iz keša: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<Nagrade>> GetCachedNagradeByUserId(int userId)
        {
            var cacheKey = $"Nagrade_{userId}";

            try
            {
                var cachedNagrade = await GetFromCacheAsync(cacheKey);

                if (cachedNagrade == null)
                {
                    var nagradeFromDatabase = await GetNagradeFromDatabaseByUserIdAsync(userId);

                    if (nagradeFromDatabase != null)
                    {
                        await AddToCacheAsync(cacheKey, nagradeFromDatabase);
                    }

                    return nagradeFromDatabase;
                }

                return JsonSerializer.Deserialize<IEnumerable<Nagrade>>(cachedNagrade);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom dobavljanja nagrada po ID iz keša: {ex.Message}");
                throw;
            }
        }

        public async Task AddNagrade(Nagrade nagrade)
        {
            try
            {
                await AddNagradeToDatabaseAsync(nagrade);
                await InvalidateCacheAsync($"Nagrade_{nagrade.UserId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom dodavanja nagrada: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateNagrade(Nagrade nagrade)
        {
            try
            {
                await UpdateNagradeInDatabaseAsync(nagrade);
                await InvalidateCacheAsync($"Nagrade_{nagrade.UserId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom ažuriranja nagrada: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteNagrade(int nagradeId)
        {
            try
            {
                await DeleteNagradeFromDatabaseAsync(nagradeId);
                await InvalidateCacheAsync($"Nagrade_{nagradeId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom brisanja nagrada sa ID {nagradeId}: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<Nagrade>> GetNagradeByUserId(int userId)
        {
            try
            {
                return await _redisDatabase.GetAsync<IEnumerable<Nagrade>>($"Nagrade_{userId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom dobavljanja nagrada iz Redis baze po ID korisnika: {ex.Message}");
                throw;
            }
        }

        private async Task AddToCacheAsync(string key, object value)
        {
            try
            {
                var jsonValue = JsonSerializer.Serialize(value);
                var cacheEntryOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                };

                await _cache.SetStringAsync(key, jsonValue, cacheEntryOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom dodavanja u keš: {ex.Message}");
                throw;
            }
        }

        private async Task InvalidateCacheAsync(string key)
        {
            try
            {
                await _cache.RemoveAsync(key);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom brisanja iz keša: {ex.Message}");
                throw;
            }
        }

        private async Task<Nagrade> GetNagradeFromDatabaseAsync(int nagradeId)
        {
            try
            {
                return await _redisDatabase.GetAsync<Nagrade>($"Nagrade_{nagradeId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom dobavljanja nagrada iz Redis baze po ID: {ex.Message}");
                throw;
            }
        }

        private async Task<IEnumerable<Nagrade>> GetNagradeFromDatabaseByUserIdAsync(int userId)
        {
            try
            {
                return await _redisDatabase.GetAsync<IEnumerable<Nagrade>>($"Nagrade_{userId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom dobavljanja nagrada iz Redis baze po ID korisnika: {ex.Message}");
                throw;
            }
        }

        private async Task AddNagradeToDatabaseAsync(Nagrade nagrade)
        {
            try
            {
                await _redisDatabase.SetAsync($"Nagrade_{nagrade.NagradeId}", nagrade);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom dodavanja nagrada u Redis bazu: {ex.Message}");
                throw;
            }
        }

        private async Task UpdateNagradeInDatabaseAsync(Nagrade nagrade)
        {
            try
            {
                await _redisDatabase.SetAsync($"Nagrade_{nagrade.NagradeId}", nagrade);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom ažuriranja nagrada u Redis bazi: {ex.Message}");
                throw;
            }
        }

        private async Task DeleteNagradeFromDatabaseAsync(int nagradeId)
        {
            try
            {
                await _redisDatabase.RemoveAsync($"Nagrade_{nagradeId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom brisanja nagrada iz Redis baze sa ID {nagradeId}: {ex.Message}");
                throw;
            }
        }

        private async Task<string> GetFromCacheAsync(string key)
        {
            try
            {
                return await _cache.GetStringAsync(key);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom dobijanja iz keša: {ex.Message}");
                throw;
            }
        }
    }
}
