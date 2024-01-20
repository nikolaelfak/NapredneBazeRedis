using FitAndFun.Common;
using FitAndFun.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace FitAndFun.Services
{
    public class RoutineService : IRoutineService
    {
        private readonly IRedisDatabase _redisDatabase;
        private readonly IDistributedCache _cache;

        public RoutineService(IRedisDatabase redisDatabase, IDistributedCache cache)
        {
            _redisDatabase = redisDatabase;
            _cache = cache;
        }

        public async Task<Routine> GetRoutineById(int routineId)
        {
            try
            {
                return await _redisDatabase.GetAsync<Routine>($"Routine_{routineId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom dobavljanja rutine sa ID {routineId} iz Redis baze: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<Routine>> GetRoutinesByUserId(int userId)
        {
            try
            {
                return await _redisDatabase.GetAsync<IEnumerable<Routine>>($"Routines_{userId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom dobavljanja rutina za korisnika sa ID {userId} iz Redis baze: {ex.Message}");
                throw;
            }
        }

        public async Task AddRoutine(Routine routine)
        {
            try
            {
                await _redisDatabase.SetAsync($"Routine_{routine.RoutineId}", routine);
                await InvalidateCacheAsync($"Routines_{routine.UserId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom dodavanja rutine u Redis bazu: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateRoutine(Routine routine)
        {
            try
            {
                await _redisDatabase.SetAsync($"Routine_{routine.RoutineId}", routine);
                await InvalidateCacheAsync($"Routines_{routine.UserId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom ažuriranja rutine u Redis bazi: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteRoutine(int routineId)
        {
            try
            {
                await _redisDatabase.RemoveAsync($"Routine_{routineId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom brisanja rutine sa ID {routineId} iz Redis baze: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<Routine>> GetCachedRoutinesByUserId(int userId)
        {
            var cacheKey = $"Routines_{userId}";

            try
            {
                var cachedRoutines = await GetFromCacheAsync(cacheKey);

                if (cachedRoutines == null)
                {
                    var routinesFromDatabase = await _redisDatabase.GetAsync<IEnumerable<Routine>>($"Routines_{userId}");

                    if (routinesFromDatabase != null)
                    {
                        await AddToCacheAsync(cacheKey, routinesFromDatabase);
                    }

                    return routinesFromDatabase;
                }

                return JsonSerializer.Deserialize<IEnumerable<Routine>>(cachedRoutines);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom dobavljanja rutina iz keša: {ex.Message}");
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
                Console.WriteLine($"Greška prilikom dobavljanja iz keša: {ex.Message}");
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
    }
}
